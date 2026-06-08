using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
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

    [SerializeField] private BattleResultController battleResultController;

    private int selectedPlateIndex = -1;
    public int currentTurn;
    public int clearTurn;
    private bool hasSummonedThisTurn;

    private void Awake()
    {
        summonButtonText = summonButton.GetComponentInChildren<TextMeshProUGUI>();
        reSummonButtonText = reSummonButton.GetComponentInChildren<TextMeshProUGUI>();
        EnsureBattleResultController();
        clearTurn = turnController.GetClearTurn();
        ResetPlayerSetting();
    }

    private void Update()
    {
        RedrawButtonStateShow();
    }

    private void RedrawButtonStateShow()
    {
        if (mana < usedMana)
        {
            RedrawButtonDisabledShow();
            return;
        }

        RedrawButtonEnabledShow();
    }

    private void RedrawButtonDisabledShow()
    {
        reSummonButton.image.color = new Color32(174, 174, 174, 255);
        reSummonButtonText.color = new Color32(209, 209, 209, 255);
    }

    private void RedrawButtonEnabledShow()
    {
        reSummonButton.image.color = new Color32(249, 247, 196, 255);
        reSummonButtonText.color = new Color32(249, 247, 196, 255);
    }

    public void PlayerTurnStart()
    {
        Debug.Log("플레이어 턴 시작");
        Debug.Log($"{gameObject.name} 의 마나: {mana}");
        currentTurn = turnController.GetTurnCount();
        hasSummonedThisTurn = false;
        UpdateManaUI();

        BattleFailResultTry();
    }

    public void OnSummonBtnClick()
    {
        if (PlayerActionBlockedCheck()) return;

        if (!SummonTurnCanUseCheck())
        {
            return;
        }

        if (!SummonManaCanUseCheck())
        {
            return;
        }

        if (!PlayerEmptyPlateSummonTry())
        {
            Debug.Log("모든 플레이트에 소환수가 있습니다.");
        }
    }

    private bool SummonTurnCanUseCheck()
    {
        if (!hasSummonedThisTurn)
        {
            return true;
        }

        failSound.Play();
        Debug.Log("이 턴에서는 이미 소환을 했습니다. 다음 턴에 소환할 수 있습니다.");
        return false;
    }

    private bool SummonManaCanUseCheck()
    {
        if (mana > 0)
        {
            return true;
        }

        failSound.Play();
        Debug.Log("마나가 부족하여 소환 불가능");
        return false;
    }

    private bool PlayerEmptyPlateSummonTry()
    {
        for (int i = 0; i < plateController.GetPlayerPlates().Count; i++)
        {
            if (!plateController.GetPlayerPlates()[i].GetIsInSummon())
            {
                PlayerPlateSummonStart(i);
                return true;
            }
        }

        return false;
    }

    private void PlayerPlateSummonStart(int plateIndex)
    {
        Debug.Log(plateIndex + "번째 플레이트에 소환 예정");
        summonController.StartSummon(plateIndex, false);
        mana -= 1;
        hasSummonedThisTurn = true;
        UpdateManaUI();
        clickSound.Play();
        SummonButtonDisabledShow();
    }

    private void SummonButtonDisabledShow()
    {
        summonButton.image.color = new Color32(137, 125, 115, 255);
        summonButtonText.color = new Color32(159, 159, 159, 255);
    }

    public void PlayerTurnOverBtn()
    {
        if (PlayerActionBlockedCheck()) return;

        if (!PlayerTurnCanEndCheck())
        {
            return;
        }

        PlayerTurnEnd();
    }

    private bool PlayerTurnCanEndCheck()
    {
        if (turnController.GetCurrentTurn() == TurnController.Turn.PlayerTurn)
        {
            return true;
        }

        failSound.Play();
        Debug.Log("플레이어 턴이 아닙니다.");
        return false;
    }

    private void PlayerTurnEnd()
    {
        Debug.Log("플레이어 턴 종료");
        turnController.EndTurn();
        clickSound.Play();
    }

    public void OnRedrawButtonClick()
    {
        if (PlayerActionBlockedCheck()) return;

        if (!RedrawManaCanUseCheck())
        {
            return;
        }

        RedrawStartTry();
    }

    private bool RedrawManaCanUseCheck()
    {
        if (mana >= usedMana)
        {
            return true;
        }

        failSound.Play();
        Debug.Log("재소환시 필요한 마나가 모자랍니다.");
        return false;
    }

    private void RedrawStartTry()
    {
        if (!summonController.StartRedraw())
        {
            return;
        }

        RedrawManaUse();
    }

    private void RedrawManaUse()
    {
        mana -= usedMana;
        usedMana += 1;
        UpdateManaUI();
        clickSound.Play();
    }

    public void OnAttackBtnClick()
    {
        if (PlayerActionBlockedCheck()) return;

        Summon attackSummon = NormalAttackSummonGet();
        if (attackSummon == null)
        {
            return;
        }

        NormalAttackExecute(attackSummon);
        PlayerAttackAfterProcess();
    }

    private bool PlayerActionBlockedCheck()
    {
        return summonController.isSummoning || battleController.GetIsAttacking();
    }

    private Summon NormalAttackSummonGet()
    {
        Summon attackSummon = battleController.AttackStart(0);
        if (attackSummon == null)
        {
            failSound.Play();
            return null;
        }

        if (!NormalAttackCanUse(attackSummon))
        {
            Debug.Log("공격할 수 없습니다. ");
            failSound.Play();
            return null;
        }

        return attackSummon;
    }

    private bool NormalAttackCanUse(Summon attackSummon)
    {
        return attackSummon.GetIsAttack() && !attackSummon.IsStun();
    }

    private void NormalAttackExecute(Summon attackSummon)
    {
        attackSummon.NormalAttack(plateController.GetEnermyPlates(), selectedPlateIndex);
        clickSound.Play();
    }

    private void PlayerAttackAfterProcess()
    {
        BattleClearResultTry();
        plateController.CompactEnermyPlates();
        statePanel.gameObject.SetActive(false);
    }

    public void OnSpecialAttackBtnClick()
    {
        if (PlayerActionBlockedCheck()) return;

        Summon attackSummon = SpecialAttackSummonGet();
        if (attackSummon == null)
        {
            return;
        }

        if (!SpecialAttackExecuteOrTargetSelect(attackSummon))
        {
            return;
        }

        PlayerAttackAfterProcess();
    }

    private Summon SpecialAttackSummonGet()
    {
        Summon attackSummon = battleController.AttackStart(0);
        if (attackSummon == null)
        {
            failSound.Play();
            return null;
        }

        if (!battleController.HasCurrentSpecialAttackInfo())
        {
            Debug.Log("사용 가능한 특수 공격이 없습니다.");
            failSound.Play();
            return null;
        }

        if (!SpecialAttackCanUse(attackSummon))
        {
            Debug.Log("공격할 수 없습니다. ");
            failSound.Play();
            return null;
        }

        return attackSummon;
    }

    private bool SpecialAttackCanUse(Summon attackSummon)
    {
        return attackSummon.GetIsAttack() && !attackSummon.IsStun();
    }

    private bool SpecialAttackExecuteOrTargetSelect(Summon attackSummon)
    {
        IAttackStrategy attackStrategy = attackSummon.GetSpecialAttackStrategy()[0];

        if (SpecialAttackCooldownCheck(attackStrategy))
        {
            Debug.Log("특수 스킬이 쿨타임 중입니다. 사용할 수 없습니다.");
            failSound.Play();
            return false;
        }

        if (attackStrategy is TargetedAttackStrategy)
        {
            TargetedSpecialAttackStart(attackSummon);
            return true;
        }

        ImmediateSpecialAttackExecute(attackSummon);
        return true;
    }

    private bool SpecialAttackCooldownCheck(IAttackStrategy attackStrategy)
    {
        return attackStrategy.GetCurrentCooldown() > 0;
    }

    private void TargetedSpecialAttackStart(Summon attackSummon)
    {
        int specialAttackIndex = battleController.GetCurrentSpecialAttackInfoIndex();
        clickSound.Play();

        if (battleController.DoesCurrentSpecialAttackTargetPlayerPlate())
        {
            PlayerPlateTargetSelectionStart(attackSummon, specialAttackIndex);
            return;
        }

        EnermyPlateTargetSelectionStart(attackSummon, specialAttackIndex);
    }

    private void PlayerPlateTargetSelectionStart(Summon attackSummon, int specialAttackIndex)
    {
        Debug.Log("아군의 플레이트를 선택하세요.");
        StartCoroutine(WaitForPlayerPlateSelection(attackSummon, specialAttackIndex));
    }

    private void EnermyPlateTargetSelectionStart(Summon attackSummon, int specialAttackIndex)
    {
        Debug.Log("적의 플레이트를 선택하세요.");
        StartCoroutine(WaitForEnermyPlateSelection(attackSummon, specialAttackIndex));
    }

    private void ImmediateSpecialAttackExecute(Summon attackSummon)
    {
        battleController.SpecialAttackExecute(attackSummon, selectedPlateIndex, 0, true);
        clickSound.Play();
    }

    private IEnumerator WaitForEnermyPlateSelection(Summon attackSummon, int SpecialAttackArrayIndex)
    {
        return WaitForTargetPlateSelection(
            attackSummon,
            SpecialAttackArrayIndex,
            plateController.GetEnermyPlates(),
            true,
            "적의 플레이트를 선택하는 중입니다...",
            "적 플레이트 외부 클릭으로 선택 취소",
            "공격을 준비 중입니다. 선택된 플레이트 인덱스: {0}",
            "공격할 적의 플레이트 인덱스가 유효하지 않습니다.");
    }

    private IEnumerator WaitForPlayerPlateSelection(Summon attackSummon, int SpecialAttackArrayIndex)
    {
        return WaitForTargetPlateSelection(
            attackSummon,
            SpecialAttackArrayIndex,
            plateController.GetPlayerPlates(),
            false,
            "아군의 플레이트를 선택하는 중입니다...",
            "플레이트 외부 클릭으로 선택 취소",
            "아군에게 버프를 준비중입니다. 선택된 플레이트 인덱스: {0}",
            "아군의 플레이트 인덱스가 유효하지 않습니다.");
    }

    private IEnumerator WaitForTargetPlateSelection(
        Summon attackSummon,
        int specialAttackArrayIndex,
        List<Plate> targetPlates,
        bool downTransparencyForPlayerPlate,
        string waitLog,
        string outsideClickLog,
        string executeLogFormat,
        string invalidLog)
    {
        battleController.SetIsAttacking(true);
        summonController.OnDarkBackground(true);
        plateController.DownTransparencyForWhoPlate(downTransparencyForPlayerPlate);
        selectedPlateIndex = -1;
        Debug.Log(waitLog);

        while (selectedPlateIndex < 0)
        {
            if (battleController.GetIsAttacking() &&
                Input.GetMouseButtonDown(0) &&
                !MousePositionInsidePlates(targetPlates))
            {
                Debug.Log(outsideClickLog);
                summonController.OnDarkBackground(false);
                battleController.SetIsAttacking(false);
                yield break;
            }

            yield return null;
        }

        if (selectedPlateIndex >= 0)
        {
            Debug.Log(string.Format(executeLogFormat, selectedPlateIndex));
            battleController.SpecialAttackExecute(attackSummon, selectedPlateIndex, specialAttackArrayIndex, true);
            summonController.OnDarkBackground(false);
            selectedPlateIndex = -1;
        }
        else
        {
            Debug.LogError(invalidLog);
        }
    }

    private bool MousePositionInsidePlates(List<Plate> targetPlates)
    {
        Vector2 mousePosition = Input.mousePosition;

        foreach (var plate in targetPlates)
        {
            RectTransform plateRect = plate.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(plateRect, mousePosition))
            {
                return true;
            }
        }

        return false;
    }

    public void SetHasSummonedThisTurn(bool value)
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

        if (mana > 0 && !hasSummonedThisTurn)
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

    public void SetSelectedPlateIndex(int selectedPlateIndex)
    {
        this.selectedPlateIndex = selectedPlateIndex;
    }

    public PlateController GetPlateController()
    {
        return plateController;
    }

    public void TurnOver()
    {
        BattleFailResultTry();
    }

    private void BattleClearResultTry()
    {
        battleResultController.ClearResultTry(
            plateController.IsEnermyPlateClear(),
            clearTurn,
            currentTurn);
    }

    private void BattleFailResultTry()
    {
        battleResultController.FailResultTry(clearTurn, currentTurn);
    }

    private void EnsureBattleResultController()
    {
        if (battleResultController != null)
        {
            return;
        }

        battleResultController = GetComponent<BattleResultController>();
        if (battleResultController == null)
        {
            battleResultController = gameObject.AddComponent<BattleResultController>();
        }
    }
}
