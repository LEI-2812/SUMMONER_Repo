using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Player : Character
{
    [Header("플레이어 플레이트")]
    [SerializeField] private List<Plate> playerPlates; // 플레이어가 사용할 플레이트 목록

    //마나
    private int mana; // 플레이어의 기본마나
    private int usedMana; //재소환시 필요한 마나
    [Header("마나UI")]
    [SerializeField] private List<RawImage> manaList; //마나UI
    [Header("마나텍스쳐")]
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;


    //컨트롤러들
    [Header("컨트롤러")]
    [SerializeField] private TurnController turnController; // TurnController 참조
    [SerializeField] private SummonController summonController; //소환버튼으로 소환수를 가져오기 위해 필요

    // 마지막으로 소환했던 플레이트 번호 저장
    private int lastSummonedPlateIndex = -1; // 소환했던 플레이트의 인덱스를 저장 (-1은 초기값)

    // 턴당 소환 여부 확인
    private bool hasSummonedThisTurn;

    private void Start()
    {
        ResetPlayerSetting();
    }

  
    //턴시작
    public override void startTurn()
    {
        base.startTurn();
        Debug.Log($"{gameObject.name} 의 마나: {mana}");
        hasSummonedThisTurn = false; // 매 턴 소환여부 초기화
    }


    //플레이어의 활동 로직
    public override void takeAction()
    {
        Debug.Log("플레이어 takeAction 실행");
    }


    // 소환수를 뽑아 해당 플레이트에 배치
    public void TakeSummon(int plateIndex, bool reSummon)
    {
        summonController.randomTakeSummon();
        summonController.TakeSummonPanel.SetActive(true);

        if (reSummon) //재소환일경우
            StartCoroutine(ReSummonSelection(plateIndex));
        else //일반소환
            StartCoroutine(TakeSummonSelection(plateIndex));
    }

    // 소환 코루틴 (기존 소환 로직)
    private IEnumerator TakeSummonSelection(int plateIndex)
    {
        while (summonController.GetSelectedSummon() == null)
        {
            yield return null;
        }

        Summon selectedSummon = summonController.GetSelectedSummon();
        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon);
            lastSummonedPlateIndex = plateIndex; // 소환된 플레이트 번호 저장
            hasSummonedThisTurn = true;
            Debug.Log($"플레이트 {plateIndex}에 소환 완료.");
        }
        else
        {
            Debug.Log("선택된 소환수가 없습니다.");
        }
    }

    // 재소환 코루틴
    private IEnumerator ReSummonSelection(int plateIndex)
    {
        while (summonController.GetSelectedSummon() == null)
        {
            yield return null;
        }

        Summon selectedSummon = summonController.GetSelectedSummon();
        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
            Debug.Log($"플레이트 {plateIndex}에 재소환 완료.");
        }
        else
        {
            Debug.Log("선택된 소환수가 없습니다.");
        }
    }



    //플레이어는 버튼 클릭을 통해서만 턴종료를 시킨다.
    public void PlayerTurnOverBtn() //버튼에 넣을 메소드
    {
        // 플레이어 턴일 때만 턴 종료 가능
        if (turnController.currentTurn == TurnController.Turn.PlayerTurn)
        {
            Debug.Log("플레이어 턴 종료");
            turnController.EndTurn();
        }
        else
        {
            Debug.Log("플레이어 턴이 아닙니다.");
        }
    }

    ///여기부턴 버튼들 메소드 <summary>
    /// 
    /// 
    public void OnSummonBtnClick() //소환 버튼 클릭 메소드
    {
        Debug.Log($"{gameObject.name} 의 마나: {mana}");
        if (hasSummonedThisTurn)
        {
            Debug.Log("이 턴에서는 이미 소환을 했습니다. 다음 턴에 소환할 수 있습니다.");
            return; // 소환은 턴당 1회만 가능
        }

        if (mana > 0) //재소환에 필요한 마나보다 많고 0보다 많아야함.
        {
            for (int i = 0; i < playerPlates.Count; i++) //비어있는 플레이트를 찾으러 순차적으로 순회
            {
                if (playerPlates[i].isInSummon == false) //가장 가까운 비어있는 곳이 있으면 선택.
                {
                    Debug.Log(i + "번째 플레이트에 소환 예정");
                    TakeSummon(i, false); // 소환
                    mana -= 1; // 일반 소환 시 마나 1 소모
                    hasSummonedThisTurn = true; //이번턴 소환했음을 알림.
                    UpdateManaUI();
                    return; //메소드 종료해버리기
                }
            }
            Debug.Log("모든 플레이트에 소환수가 있습니다."); //소환은 가능했는데 비어있는곳이 없어서 출력
            return;
        }
        else
        {
            takeAction();
            Debug.Log("마나가 부족하여 소환 불가능");
        }
    }

    //재소환 버튼 클릭 이벤트
    public void OnReSummonBtnClick()
    {
        if (!hasSummonedThisTurn)
        {
            Debug.Log("이 턴에 소환을 하지 않았습니다.");
            return; // 소환은 턴당 1회만 가능
        }

        Debug.Log($"{gameObject.name} 의 마나: {mana}");
        if (mana > usedMana) // 재소환에 필요한 마나보다 현재 보유 마나가 더 많은지 확인
        {
            if (mana >= usedMana && lastSummonedPlateIndex != -1) // 재소환 가능한 마나가 있고, 소환한 플레이트가 있다면
            {
                TakeSummon(lastSummonedPlateIndex, true); // 기록된 플레이트 번호로 재소환
                mana -= usedMana; // 재소환 마나 소모
                usedMana += 1; // 재소환 필요마나 축적
                UpdateManaUI();
                Debug.Log("기록된 플레이트에 재소환 완료");
            }
            else
            {
                Debug.Log("해당 턴에 소환된 소환수가 없습니다.");
            }
        }
        else
        {
            Debug.Log("재소환에 필요한 마나가 부족합니다.");
        }
    }

    //플레이어 설정 초기화
    private void ResetPlayerSetting()
    {
        mana = 10; //첫 게임 시작시 마나 10으로 시작
        usedMana = 1; //사용될 마나
        lastSummonedPlateIndex = -1; // 소환 기록 초기화
        UpdateManaUI();
    }

    // 마나 UI 업데이트 메소드 추가
    public void UpdateManaUI()
    {
        for (int i = 0; i < manaList.Count; i++)
        {
            if (i < mana) // 현재 남아 있는 마나만큼 채워줌
            {
                // 마나가 있으면 "Have" 텍스처를 설정
                manaList[i].texture = haveTexture;
            }
            else
            {
                // 마나가 없으면 "Not Have" 텍스처를 설정
                manaList[i].texture = notHaveTexture;
            }
        }
    }

}

