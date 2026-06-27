public sealed class BattleAttackData
{
    public bool IsSpecialAttackTargetSelectionActive { get; private set; }
    public Summon AttackingSummon { get; private set; }
    public SpecialAttackInfo CurrentSpecialAttackInfo { get; private set; }
    public int AttackingPlateIndex { get; private set; } = -1;
    public int SelectedSpecialAttackTargetPlateIndex { get; private set; } = -1;

    private Summon selectedPlayerAttackSource;
    private int selectedPlayerAttackSourcePlateIndex = -1;

    public void SelectPlayerAttackSource(Summon summon, int plateIndex)
    {
        selectedPlayerAttackSource = summon;
        selectedPlayerAttackSourcePlateIndex = plateIndex;
    }

    public void ClearPlayerAttackSource()
    {
        selectedPlayerAttackSource = null;
        selectedPlayerAttackSourcePlateIndex = -1;
    }

    public void StartAttackFromSelectedSource()
    {
        AttackingSummon = selectedPlayerAttackSource;
        AttackingPlateIndex = selectedPlayerAttackSourcePlateIndex;
        CurrentSpecialAttackInfo = null;
    }

    public void SetCurrentSpecialAttackInfo(SpecialAttackInfo specialAttackInfo)
    {
        CurrentSpecialAttackInfo = specialAttackInfo;
    }

    public void StartSpecialAttackTargetSelection()
    {
        IsSpecialAttackTargetSelectionActive = true;
        ClearSpecialAttackTargetSelection();
    }

    public void CancelSpecialAttackTargetSelection()
    {
        IsSpecialAttackTargetSelectionActive = false;
    }

    public void SelectSpecialAttackTargetPlate(int plateIndex)
    {
        SelectedSpecialAttackTargetPlateIndex = plateIndex;
    }

    public void ClearSpecialAttackTargetSelection()
    {
        SelectedSpecialAttackTargetPlateIndex = -1;
    }

    public bool HasCurrentSpecialAttackInfo()
    {
        return CurrentSpecialAttackInfo != null;
    }

    public int GetCurrentSpecialAttackInfoIndex()
    {
        return CurrentSpecialAttackInfo == null ? -1 : CurrentSpecialAttackInfo.GetAttackInfoIndex();
    }

    public bool DoesCurrentSpecialAttackTargetPlayerPlate()
    {
        if (CurrentSpecialAttackInfo == null || CurrentSpecialAttackInfo.GetAttackInfoStrategy() == null)
        {
            return false;
        }

        return CurrentSpecialAttackInfo.GetAttackInfoStrategy().TargetsOwnPlates();
    }

    public void ResetAttackSelection()
    {
        IsSpecialAttackTargetSelectionActive = false;
        AttackingSummon = null;
        CurrentSpecialAttackInfo = null;
        AttackingPlateIndex = -1;
        ClearPlayerAttackSource();
        ClearSpecialAttackTargetSelection();
    }
}
