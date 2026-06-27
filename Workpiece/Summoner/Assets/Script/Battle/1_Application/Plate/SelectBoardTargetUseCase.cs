using UnityEngine;

public class SelectBoardTargetUseCase
{
    private readonly BattleBoardInputController plate;
    private readonly SummonSelectionController summonSelectionController;
    private readonly AttackStateMachine attackStateMachine;
    private readonly PlateBoardView plateBoardController;
    private readonly GameObject statePanel;
    private readonly SummonStatePanelView statePanelView;
    private readonly AudioSource clickSound;

    public SelectBoardTargetUseCase(
        BattleBoardInputController plate,
        SummonSelectionController summonSelectionController,
        AttackStateMachine attackStateMachine,
        PlateBoardView plateBoardController,
        GameObject statePanel,
        SummonStatePanelView statePanelView,
        AudioSource clickSound)
    {
        this.plate = plate;
        this.summonSelectionController = summonSelectionController;
        this.attackStateMachine = attackStateMachine;
        this.plateBoardController = plateBoardController;
        this.statePanel = statePanel;
        this.statePanelView = statePanelView;
        this.clickSound = clickSound;
    }

    public void Execute()
    {
        if (TrySelectResummonPlate())
        {
            PlayClickSound();
            return;
        }

        if (TrySelectAttackTargetPlate())
        {
            PlayClickSound();
            return;
        }

        TryShowSummonStatusPanel();
        PlayClickSound();
    }

    public void ExecutePointerEnter()
    {
        ShowSummonHoverEnter();
        ShowSummonSelectionHoverEnter();
        ShowAttackTargetHoverEnter();
    }

    public void ExecutePointerExit()
    {
        ShowSummonSelectionHoverExit();
        ShowAttackTargetHoverExit();
    }

    private void PlayClickSound()
    {
        if (clickSound != null && clickSound.isActiveAndEnabled && clickSound.gameObject.activeInHierarchy)
        {
            clickSound.Play();
        }
    }

    private bool TrySelectResummonPlate()
    {
        if (!IsSummonSelectionActive() || !plate.GetIsInSummon())
        {
            return false;
        }

        summonSelectionController.SelectPlate(plate);
        plate.Unhighlight();
        plate.SetSummonImageTransparency(1.0f);
        return true;
    }

    private void ShowSummonHoverEnter()
    {
        if (plate.GetCurrentSummon() == null)
        {
            return;
        }

        plate.SetSummonImageTransparency(1.0f);
    }

    private void ShowSummonSelectionHoverEnter()
    {
        if (!plate.GetIsInSummon() || !IsSummonSelectionActive())
        {
            return;
        }

        plate.Highlight();
        plate.SetSummonImageTransparency(1.0f);
    }

    private void ShowAttackTargetHoverEnter()
    {
        if (!BattleAttackActiveOnPlate())
        {
            return;
        }

        if (CanSelectCurrentPlateAsAttackTarget())
        {
            plate.Highlight();
            return;
        }

        plate.SetSummonImageTransparency(0.5f);
    }

    private void ShowSummonSelectionHoverExit()
    {
        if (plate.GetCurrentSummon() == null || !IsSummonSelectionActive())
        {
            return;
        }

        plate.Unhighlight();
        plate.SetSummonImageTransparency(0.5f);
    }

    private void ShowAttackTargetHoverExit()
    {
        if (!BattleAttackActiveOnPlate())
        {
            return;
        }

        plate.Unhighlight();
    }

    private void TryShowSummonStatusPanel()
    {
        Summon currentSummon = plate.GetCurrentSummon();
        if (currentSummon == null || IsSummonSelectionActive() || IsBattleAttacking())
        {
            return;
        }

        Debug.Log("소환수 상태 패널 열기: " + currentSummon.GetSummonName());
        if (statePanel == null || statePanelView == null)
        {
            attackStateMachine?.ClearPlayerAttackSource();
            return;
        }

        int plateIndex = GetPlayerPlateIndex();
        bool isEnemyPlate = IsCurrentEnemyPlate();
        if (attackStateMachine == null)
        {
            return;
        }

        if (isEnemyPlate)
        {
            attackStateMachine.ClearPlayerAttackSource();
        }
        else
        {
            attackStateMachine.SelectPlayerAttackSource(currentSummon, plateIndex);
        }

        statePanel.SetActive(true);
        statePanelView.SetStatePanel(currentSummon, isEnemyPlate);
    }

    private bool TrySelectAttackTargetPlate()
    {
        if (!BattleAttackActiveOnPlate())
        {
            return false;
        }

        if (!TrySelectSpecialAttackTarget(plate, out int plateIndex, out string plateName))
        {
            Debug.Log("유효하지 않은 공격 대상 플레이트입니다.");
            return false;
        }

        Debug.Log($"{plateName} 플레이트 {plateIndex}번을 선택했습니다.");
        plate.Unhighlight();
        return true;
    }

    private bool CanSelectCurrentPlateAsAttackTarget()
    {
        if (plateBoardController == null)
        {
            return false;
        }

        bool targetsPlayerPlate = attackStateMachine != null && attackStateMachine.DoesCurrentSpecialAttackTargetPlayerPlate();
        return plateBoardController.TryGetAttackTargetPlate(plate, targetsPlayerPlate, out _, out _);
    }

    private bool TrySelectSpecialAttackTarget(
        BattleBoardInputController selectedPlate,
        out int plateIndex,
        out string plateName)
    {
        if (!TryGetTargetPlateIndex(selectedPlate, out plateIndex, out plateName))
        {
            return false;
        }

        attackStateMachine.SelectTargetPlate(plateIndex);
        return true;
    }

    private bool TryGetTargetPlateIndex(
        BattleBoardInputController selectedPlate,
        out int plateIndex,
        out string plateName)
    {
        plateIndex = -1;
        plateName = "none";

        if (attackStateMachine == null || plateBoardController == null)
        {
            return false;
        }

        bool targetsPlayerPlate = attackStateMachine.DoesCurrentSpecialAttackTargetPlayerPlate();
        return plateBoardController.TryGetAttackTargetPlate(
            selectedPlate,
            targetsPlayerPlate,
            out plateIndex,
            out plateName);
    }

    private bool BattleAttackActiveOnPlate()
    {
        return plate.GetIsInSummon() && IsBattleAttacking();
    }

    private bool IsSummonSelectionActive()
    {
        return summonSelectionController != null && summonSelectionController.IsSummoning();
    }

    private bool IsBattleAttacking()
    {
        return attackStateMachine != null && attackStateMachine.IsSpecialAttackTargetSelectionActive();
    }

    private int GetPlayerPlateIndex()
    {
        return plateBoardController == null ? -1 : plateBoardController.GetPlayerPlateIndex(plate);
    }

    private bool IsCurrentEnemyPlate()
    {
        return plateBoardController != null && plateBoardController.GetEnemyPlateIndex(plate) >= 0;
    }
}
