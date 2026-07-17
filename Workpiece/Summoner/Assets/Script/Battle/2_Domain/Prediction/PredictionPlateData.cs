// 역할: PredictionPlateData의 책임을 정의한다.
public class PredictionPlateData : IPlateState
{
    private readonly int plateIndex;
    private readonly Summon currentSummon;

    public PredictionPlateData(int plateIndex, Summon currentSummon)
    {
        this.plateIndex = plateIndex;
        this.currentSummon = currentSummon;
    }

    public Summon GetCurrentSummon()
    {
        return currentSummon;
    }

    public int GetPlateIndex()
    {
        return plateIndex;
    }
}
