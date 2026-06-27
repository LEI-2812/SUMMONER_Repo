// 역할: Stage Select 화면에서 필요한 저장 조회와 스테이지 선택 흐름을 처리한다.
public class StageSelectUseCase
{
    private readonly GameSaveUseCase gameSaveUseCase;
    private readonly StageTransitionUseCase stageTransitionUseCase;

    public StageSelectUseCase()
        : this(new GameSaveUseCase(new StageProgressSaveStore()), new StageTransitionUseCase())
    {
    }

    public StageSelectUseCase(GameSaveUseCase gameSaveUseCase, StageTransitionUseCase stageTransitionUseCase)
    {
        this.gameSaveUseCase = gameSaveUseCase;
        this.stageTransitionUseCase = stageTransitionUseCase;
    }

    public int GetUnlockedStage()
    {
        return gameSaveUseCase.GetGameSave().savedStage;
    }

    public bool SelectStage(int stage)
    {
        if (!stageTransitionUseCase.TryPrepareStage(stage))
        {
            return false;
        }

        gameSaveUseCase.SavePlayingStage(stage);

        if (stageTransitionUseCase.ShouldOpenStory(stage))
        {
            GameSceneUseCase.LoadStory(stage);
        }
        else
        {
            GameSceneUseCase.LoadFight(stage);
        }

        return true;
    }
}
