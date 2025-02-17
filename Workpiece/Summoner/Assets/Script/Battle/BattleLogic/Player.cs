using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;
using TMPro;

public class Player : Character
{
    [Header("마나UI")]
    [SerializeField] private List<RawImage> manaList;

    [Header("마나텍스쳐")]
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;

    private int mana;
    private int usedMana;

    [Header("버튼 UI")]
    [SerializeField] private Button summonButton;
     private TextMeshProUGUI summonButtonText;
    [SerializeField] private Button reSummonButton;
     private TextMeshProUGUI reSummonButtonText;

    [Header("상태창 패널")]
    [SerializeField] private Image statePanel;

    [Header("효과음")]
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource failSound;

    [Header("컨트롤러")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private TurnController turnController;
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlateController plateController;

    public BattleAlert battleAlert;

    private int selectedPlateIndex = -1;
    private int stageNum;
    public int currentTurn;
    public int clearTurn;
    private bool hasSummonedThisTurn;

    private void Awake()
    {
        summonButtonText = summonButton.GetComponentInChildren<TextMeshProUGUI>();
        reSummonButtonText = reSummonButton.GetComponentInChildren<TextMeshProUGUI>();
        battleAlert = GetComponent<BattleAlert>();
        stageNum = PlayerPrefs.GetInt("playingStage");
        clearTurn = turnController.GetClearTurn();
        ResetPlayerSetting();
    }

    private void Update()
    {
        if (mana < usedMana)
        {
            reSummonButton.image.color = new Color32(174, 174, 174, 255);
            reSummonButtonText.color = new Color32(209, 209, 209, 255);
        }
        else
        {
            reSummonButton.image.color = new Color32(249, 247, 196, 255);
            reSummonButtonText.color = new Color32(249, 247, 196, 255);
        }
    }

    public void startTurn()
    {
        Debug.Log("플레이어 턴 시작");
        Debug.Log($"{gameObject.name} 의 마나: {mana}");
        currentTurn = turnController.GetTurnCount();
        hasSummonedThisTurn = false;
        UpdateManaUI();
        
        if(clearTurn < currentTurn)
        {
            TurnOver();
        }
    }

    public void OnSummonBtnClick()
    {
        //공격중이거나 소환(재소환포함)중에는 클릭안되게
        if (summonController.isSummoning || battleController.getIsAttaking()) return;

        if (hasSummonedThisTurn)
        {
            failSound.Play();
            Debug.Log("이 턴에서는 이미 소환을 했습니다. 다음 턴에 소환할 수 있습니다.");
            return;
        }

        if (mana > 0)
        {
            for (int i = 0; i < plateController.getPlayerPlates().Count; i++)
            {
                if (!plateController.getPlayerPlates()[i].getIsInSummon())
                {
                    Debug.Log(i + "번째 플레이트에 소환 예정");
                    summonController.StartSummon(i, false);
                    mana -= 1;
                    hasSummonedThisTurn = true;
                    UpdateManaUI();
                    clickSound.Play();
                    summonButton.image.color = new Color32(137, 125, 115, 255); // 회색(#897D73)
                    summonButtonText.color = new Color32(159, 159, 159, 255);  // 회색(#9F9F9F)
                    return;
                }
            }
            Debug.Log("모든 플레이트에 소환수가 있습니다.");
        }
        else
        {
            failSound.Play(); // 효과음 재생
            Debug.Log("마나가 부족하여 소환 불가능");
        }
    }

    //플레이어는 버튼 클릭을 통해서만 턴종료를 시킨다.
    public void PlayerTurnOverBtn() //버튼에 넣을 메소드
    {
        //공격중이거나 소환(재소환포함)중에는 클릭안되게
        if (summonController.isSummoning || battleController.getIsAttaking()) return;


        // 플레이어 턴일 때만 턴 종료 가능
        if (turnController.getCurrentTurn() == TurnController.Turn.PlayerTurn)
        {
            Debug.Log("플레이어 턴 종료");
            turnController.EndTurn();
            clickSound.Play();
        }
        else
        {
            failSound.Play();
            Debug.Log("플레이어 턴이 아닙니다.");
        }
    }

    public void OnReSummonBtnClick() //재소환 버튼 클릭
    {
        //공격중이거나 소환(재소환포함)중에는 클릭안되게
        if (summonController.isSummoning || battleController.getIsAttaking()) return;

        if (mana >= usedMana) {
            if (summonController.StartResummon())
            { //재소환 시작
              //마나 차감
                mana -= usedMana;
                usedMana += 1;
                UpdateManaUI();
                clickSound.Play();
            }           
        }
        else
        {
            failSound.Play();
            Debug.Log("재소환시 필요한 마나가 모자랍니다.");
        }
    }


    public void OnAttackBtnClick() //일반공격
    {
        //공격중이거나 소환(재소환포함)중에는 클릭안되게
        if (summonController.isSummoning || battleController.getIsAttaking()) return;

        Summon attackSummon = battleController.attackStart(0); //공격할 소환수를 받아온다.
        if (!attackSummon.getIsAttack() || attackSummon.IsStun()) {
            Debug.Log("공격할 수 없습니다. ");
            failSound.Play();
            return; 
        }

        if (attackSummon != null)
        {
            // 일반 공격 수행(플레이트, 공격할 인덱스)
            attackSummon.normalAttack(plateController.getEnermyPlates() ,selectedPlateIndex);
            clickSound.Play();
        }
        else
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
        }

        // 승리 조건 1
        if (plateController.IsEnermyPlateClear() && (clearTurn >= currentTurn))
        {
            Debug.Log("승리!");
            battleAlert.clearAlert(stageNum);
        }
        plateController.CompactEnermyPlates();
        statePanel.gameObject.SetActive(false);
    }

    public void OnSpecialAttackBtnClick() //특수공격
    {
        //공격중이거나 소환(재소환포함)중에는 클릭안되게
        if (summonController.isSummoning || battleController.getIsAttaking()) return;

        Summon attackSummon = battleController.attackStart(0); // 공격할 소환수를 가져옴
        if (!attackSummon.getIsAttack() || attackSummon.IsStun())
        {
            Debug.Log("공격할 수 없습니다. ");
            failSound.Play();
            return;
        }

        if (attackSummon != null)
        {

            IAttackStrategy attackStrategy = attackSummon.getSpecialAttackStrategy()[0];

            // 스킬이 쿨타임 중인지 확인
            if (attackStrategy.getCurrentCooldown() > 0)
            {
                Debug.Log("특수 스킬이 쿨타임 중입니다. 사용할 수 없습니다.");
                failSound.Play();
                return;
            }
            
            // TargetedAttackStrategy를 사용하는지 확인
            if (attackStrategy is TargetedAttackStrategy targetedAttack)
            {
                battleController.setIsAttaking(true);
                StatusType attackStatusType = targetedAttack.getStatusType();
                clickSound.Play();
                if (attackStatusType == StatusType.Heal || attackStatusType == StatusType.Upgrade || attackStatusType == StatusType.Shield) //타겟중에 힐일경우
                {
                    Debug.Log("TargetedAttackStrategy의 Heal을 사용합니다. 아군의 플레이트를 선택하세요.");
                    // 아군의 플레이트를 선택하는 코루틴 실행
                    StartCoroutine(WaitForPlayerPlateSelection(attackSummon, battleController.getNowSpecialAttackInfo().getAttackInfoIndex()));
                }
                else
                {
                    Debug.Log("TargetedAttackStrategy를 사용합니다. 적의 플레이트를 선택하세요.");
                    // 적의 플레이트를 선택하는 코루틴 실행
                    StartCoroutine(WaitForEnermyPlateSelection(attackSummon, battleController.getNowSpecialAttackInfo().getAttackInfoIndex()));
                }
            }
            else
            {
                // TargetedAttackStrategy가 아닌 경우 바로 공격 실행
                //공격할 소환수, 공격할 플레이트 인덱스, 특수스킬 배열인덱스, 플레이어 공격
                battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, 0,true);
                clickSound.Play();
            }
        }
        else
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
        }

        // 승리 조건 2
        if (plateController.IsEnermyPlateClear() && (clearTurn >= currentTurn))
        {
            Debug.Log("승리!");
            battleAlert.clearAlert(stageNum);
        }
        plateController.CompactEnermyPlates();
        statePanel.gameObject.SetActive(false);
    }

    private IEnumerator WaitForEnermyPlateSelection(Summon attackSummon, int SpecialAttackArrayIndex)
    {
        battleController.setIsAttaking(true); // 공격 시작
        summonController.OnDarkBackground(true); // 배경 어둡게 처리
        plateController.DownTransparencyForWhoPlate(true); // 아군 소환수 투명화
        selectedPlateIndex = -1; // 선택한 플레이트 초기화

        Debug.Log("적의 플레이트를 선택하는 중입니다...");

        // 선택된 플레이트가 없을 때까지 기다림
        while (selectedPlateIndex < 0)
        {
            // 공격이 활성화된 상태에서 마우스 클릭 감지
            if (battleController.getIsAttaking())
            {
                // 플레이트 선택 시 루프 탈출
                if (selectedPlateIndex >= 0)
                {
                    Debug.Log($"적의 플레이트 {selectedPlateIndex}가 선택되었습니다.");
                    break;
                }

                // 마우스 클릭 위치가 플레이트 외부일 때 선택 취소
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePosition = Input.mousePosition;
                    bool clickedOutside = true;

                    foreach (var plate in plateController.getEnermyPlates())
                    {
                        RectTransform plateRect = plate.GetComponent<RectTransform>();
                        if (RectTransformUtility.RectangleContainsScreenPoint(plateRect, mousePosition))
                        {
                            clickedOutside = false;
                            break;
                        }
                    }

                    if (clickedOutside)
                    {
                        Debug.Log("적 플레이트 외부 클릭으로 선택 취소");
                        summonController.OnDarkBackground(false); // 배경 복원
                        battleController.setIsAttaking(false); // 공격 상태 해제
                        yield break; // 코루틴 종료
                    }
                }
            }

            yield return null; // 한 프레임 대기
        }

        // 선택된 플레이트로 공격 수행
        if (selectedPlateIndex >= 0)
        {
            Debug.Log($"공격을 준비 중입니다. 선택된 플레이트 인덱스: {selectedPlateIndex}");
            battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, SpecialAttackArrayIndex, true);
            summonController.OnDarkBackground(false); // 배경 복원
            selectedPlateIndex = -1; // 선택한 플레이트 초기화
        }
        else
        {
            Debug.LogError("공격할 적의 플레이트 인덱스가 유효하지 않습니다.");
        }
    }

    private IEnumerator WaitForPlayerPlateSelection(Summon attackSummon, int SpecialAttackArrayIndex)
    {
        battleController.setIsAttaking(true); // 공격 시작
        summonController.OnDarkBackground(true); // 배경 어둡게 처리
        plateController.DownTransparencyForWhoPlate(false); // 적 소환수 투명화
        selectedPlateIndex = -1; // 선택한 플레이트 초기화

        Debug.Log("아군의 플레이트를 선택하는 중입니다...");

        // 선택된 플레이트가 없을 때까지 기다림
        while (selectedPlateIndex < 0)
        {
            // 공격이 활성화된 상태에서 마우스 클릭 감지
            if (battleController.getIsAttaking())
            {
                // 플레이트 선택 시 루프 탈출
                if (selectedPlateIndex >= 0)
                {
                    Debug.Log($"아군의 플레이트 {selectedPlateIndex}가 선택되었습니다.");
                    break;
                }

                // 마우스 클릭 위치가 플레이트 외부일 때 선택 취소
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePosition = Input.mousePosition;
                    bool clickedOutside = true;

                    foreach (var plate in plateController.getPlayerPlates())
                    {
                        RectTransform plateRect = plate.GetComponent<RectTransform>();
                        if (RectTransformUtility.RectangleContainsScreenPoint(plateRect, mousePosition))
                        {
                            clickedOutside = false;
                            break;
                        }
                    }

                    if (clickedOutside)
                    {
                        Debug.Log("플레이트 외부 클릭으로 선택 취소");
                        summonController.OnDarkBackground(false); // 배경 복원
                        battleController.setIsAttaking(false); // 공격 상태 해제
                        yield break; // 코루틴 종료
                    }
                }
            }

            yield return null; // 한 프레임 대기
        }

        // 선택한 플레이트로 로직 수행
        if (selectedPlateIndex >= 0)
        {
            Debug.Log($"아군에게 버프를 준비중입니다. 선택된 플레이트 인덱스: {selectedPlateIndex}");
            battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, SpecialAttackArrayIndex, true);
            summonController.OnDarkBackground(false); // 배경 복원
            selectedPlateIndex = -1; // 선택한 플레이트 초기화
        }
        else
        {
            Debug.LogError("아군의 플레이트 인덱스가 유효하지 않습니다.");
        }
    }

    public void SetHasSummonedThisTurn(bool value) //이번턴에 소환했는지 여부
    {
        hasSummonedThisTurn = value;
    }

    public bool HasSummonedThisTurn()
    {
        return hasSummonedThisTurn;
    }

    private void ResetPlayerSetting()
    {
        mana = 10;
        usedMana = 1;
        UpdateManaUI();
    }

    public void UpdateManaUI()
    {
        for (int i = 0; i < manaList.Count; i++)
        {
            manaList[i].texture = (i < mana) ? haveTexture : notHaveTexture;
        }

        // 소환 버튼 색상 초기화
        if (mana > 0 && !hasSummonedThisTurn)   //  소환할 마나가 남아있고 이번 턴 소환을 하지 않았다면
        {
            summonButton.image.color = new Color32(227, 138, 64, 255);
            summonButtonText.color = new Color32(233, 197, 135, 255);
        }
    }

    public void AddMana()
    {
        mana += 1;
        if (mana > 10) {
            mana = 10;
        }
        UpdateManaUI();
    }

    public void setSelectedPlateIndex(int sel)
    {
        this.selectedPlateIndex = sel;
    }

    public PlateController getPlateController()
    {
        return plateController;
    }

    public void TurnOver()
    {
        Debug.Log("패배!");
        battleAlert.failAlert(stageNum);
    }
}
