// 역할: 스토리 스킵 가능 여부와 스킵 후 전투씬 이동을 처리한다.
public class StorySkipUseCase
{
    private readonly GameplaySettingUseCase gameplaySettingUseCase = new GameplaySettingUseCase();
    private readonly GameSaveUseCase gameSaveUseCase = new GameSaveUseCase();
    private readonly StageTransitionUseCase stageTransitionUseCase = new StageTransitionUseCase();

    public bool IsSkipEnabled()
    {
        return gameplaySettingUseCase.LoadStorySkipEnabled();
    }

    public void SkipToCurrentPlayingFight()
    {
        stageTransitionUseCase.SendFightStage(gameSaveUseCase.GetGameSave().playingStage);
    }
}
