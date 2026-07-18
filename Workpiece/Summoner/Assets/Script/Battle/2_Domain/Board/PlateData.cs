public class PlateData : IPlateState
{
    public PlateData(int plateIndex)
    {
        PlateIndex = plateIndex;
    }

    public int PlateIndex { get; }
    public Summon CurrentSummon { get; private set; }

    public void SetCurrentSummon(Summon summon)
    {
        CurrentSummon = summon;
    }

    public Summon GetCurrentSummon()
    {
        return CurrentSummon;
    }

    public int GetPlateIndex()
    {
        return PlateIndex;
    }
}
