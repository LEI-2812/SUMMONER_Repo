public sealed class BattleStageData
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
}
