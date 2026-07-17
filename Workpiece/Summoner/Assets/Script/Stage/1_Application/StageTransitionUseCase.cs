public class StageTransitionUseCase
{
    public void SendNextStageAfterBattle(int clearedStage)
    {
        if (ShouldOpenEpilogueAfterBattle(clearedStage))
        {
            SendEpilogue();
            return;
        }

        SendStage(GetNextStageAfterBattle(clearedStage));
    }

    public bool SendStage(int stage)
    {
        if (!CanSendStage(stage))
        {
            return false;
        }

        if (ShouldOpenStory(stage))
        {
            GameSceneUseCase.LoadStory(stage);
        }
        else
        {
            GameSceneUseCase.LoadFight(stage);
        }

        return true;
    }

    public bool CanSendStage(int stage)
    {
        return stage >= 1 && stage <= 7;
    }

    public bool SendFightStage(int stage)
    {
        if (!CanSendStage(stage))
        {
            return false;
        }

        GameSceneUseCase.LoadFight(stage);
        return true;
    }

    public void SendStageSelect()
    {
        GameSceneUseCase.LoadStageSelect();
    }

    public void SendEpilogue()
    {
        GameSceneUseCase.LoadEpilogue();
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

}
