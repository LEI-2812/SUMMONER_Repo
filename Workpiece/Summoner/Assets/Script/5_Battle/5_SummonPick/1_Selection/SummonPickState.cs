// 역할: 소환 선택 중인 소환수, 플레이트, 진행 여부를 보관한다.
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
