using UnityEngine;

// 역할: 개별 플레이트의 마우스 진입, 이탈, 클릭 입력에 대한 행동을 처리한다.
public class PlateInputActions
{
    private readonly Plate plate;
    private readonly SummonController summonController;
    private readonly BattleController battleController;
    private readonly PlateController plateController;
    private readonly GameObject statePanel;
    private readonly SummonStatePanelView statePanelView;
    private readonly AudioSource clickSound;
    private readonly PlateTargetSelectionActions targetSelectionActions;

    public PlateInputActions(
        Plate plate,
        SummonController summonController,
        BattleController battleController,
        PlateController plateController,
        GameObject statePanel,
        SummonStatePanelView statePanelView,
        AudioSource clickSound)
    {
        this.plate = plate;
        this.summonController = summonController;
        this.battleController = battleController;
        this.plateController = plateController;
        this.statePanel = statePanel;
        this.statePanelView = statePanelView;
        this.clickSound = clickSound;
        targetSelectionActions = new PlateTargetSelectionActions(
            plate,
            battleController,
            plateController);
    }

    public void ShowPointerEnter()
    {
        ShowSummonHoverEnter();
        ShowSummonSelectionHoverEnter();
        ShowAttackTargetHoverEnter();
    }

    public void ShowPointerExit()
    {
        ShowSummonSelectionHoverExit();
        ShowAttackTargetHoverExit();
    }

    public void TryHandleClick()
    {
        if (TrySelectRedrawPlate())
        {
            PlayClickSound();
            return;
        }

        if (TrySelectAttackTargetPlate())
        {
            PlayClickSound();
            return;
        }

        TryOpenSummonStatePanel();
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            clickSound.Play();
        }
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

        if (targetSelectionActions.CanSelectCurrentPlateAsAttackTarget())
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

    private bool TrySelectRedrawPlate()
    {
        if (!IsSummonSelectionActive() || !plate.GetIsInSummon())
        {
            return false;
        }

        summonController.SelectPlate(plate);
        plate.Unhighlight();
        plate.SetSummonImageTransparency(1.0f);
        return true;
    }

    private void TryOpenSummonStatePanel()
    {
        Summon currentSummon = plate.GetCurrentSummon();
        if (currentSummon == null || IsSummonSelectionActive() || IsBattleAttacking())
        {
            return;
        }

        Debug.Log("클릭된 플레이트의 소환수:" + currentSummon.GetSummonName());
        if (statePanel == null || statePanelView == null)
        {
            battleController?.ClearPlayerAttackSource();
            return;
        }

        int plateIndex = GetPlayerPlateIndex();
        bool isEnemyPlate = IsCurrentEnermyPlate();
        if (battleController == null)
        {
            return;
        }

        if (isEnemyPlate)
        {
            battleController.ClearPlayerAttackSource();
        }
        else
        {
            battleController.SelectPlayerAttackSource(currentSummon, plateIndex);
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

        return targetSelectionActions.TrySelectAttackTargetPlate();
    }

    private bool BattleAttackActiveOnPlate()
    {
        return plate.GetIsInSummon() && IsBattleAttacking();
    }

    private bool IsSummonSelectionActive()
    {
        return summonController != null && summonController.IsSummoning();
    }

    private bool IsBattleAttacking()
    {
        return battleController != null && battleController.IsSpecialAttackTargetSelectionActive();
    }

    private int GetPlayerPlateIndex()
    {
        return plateController == null ? -1 : plateController.GetPlayerPlateIndex(plate);
    }

    private bool IsCurrentEnermyPlate()
    {
        return plateController != null && plateController.ContainsEnermyPlate(plate);
    }
}
