public class BattleStageData
{
    public int CurrentStage { get; private set; }

    public BattleStageData(int defaultStage)
    {
        SetStage(defaultStage);
    }

    public void SetStage(int stage)
    {
        CurrentStage = stage;
    }

    public double GetSummonStatMultiplier()
    {
        switch (CurrentStage)
        {
            case 3:
            case 4:
                return 1.2;
            case 5:
            case 6:
                return 1.5;
            case 7:
                return 2.5;
            default:
                return 1;
        }
    }
}
