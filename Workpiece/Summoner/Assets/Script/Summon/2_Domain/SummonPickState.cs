// 역할: SummonPickState의 책임을 정의한다.
public class SummonPickState
{
    public bool IsSummoning { get; private set; }
    public int SelectedPlateIndex { get; private set; } = -1;
    public Summon SelectedSummon { get; private set; }

    public void StartSummoning()
    {
        IsSummoning = true;
    }

    public void FinishSummoning()
    {
        IsSummoning = false;
    }

    public void SetSelectedPlateIndex(int plateIndex)
    {
        SelectedPlateIndex = plateIndex;
    }

    public void SetSelectedSummon(Summon summon)
    {
        SelectedSummon = summon;
    }

    public void ClearSelectedSummon()
    {
        SelectedSummon = null;
    }
}
