public class StageTransitionUseCase
{
    public bool TryPrepareStage(int stage)
    {
        return TrySetStageMultiplier(stage);
    }

    public bool TryPrepareFightStage(int stage)
    {
        return TrySetStageMultiplier(stage);
    }

    public int GetNextStageAfterBattle(int clearedStage)
    {
        return clearedStage + 1;
    }

    public bool ShouldOpenEpilogueAfterBattle(int clearedStage)
    {
        return clearedStage >= 7;
    }

    public bool ShouldOpenStory(int stage)
    {
        return stage == 1 || stage == 2 || stage == 3 || stage == 5 || stage == 7;
    }

    private bool TrySetStageMultiplier(int stage)
    {
        switch (stage)
        {
            case 1:
            case 2:
                Summon.StatMultiplierSet(1);
                return true;
            case 3:
            case 4:
                Summon.StatMultiplierSet(1.2);
                return true;
            case 5:
            case 6:
                Summon.StatMultiplierSet(1.5);
                return true;
            case 7:
                Summon.StatMultiplierSet(2.5);
                return true;
            default:
                return false;
        }
    }
}
