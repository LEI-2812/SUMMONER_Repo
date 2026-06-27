// 역할: 스토리 스킵 가능 여부와 스킵 후 전투씬 이동을 처리한다.
public class StorySkipUseCase
{
    private readonly GameplaySettingStore gameplaySettingStore;
    private readonly GameSaveUseCase gameSaveUseCase;

    public StorySkipUseCase()
        : this(new GameplaySettingStore(), new GameSaveUseCase(new StageProgressSaveStore()))
    {
    }

    public StorySkipUseCase(GameplaySettingStore gameplaySettingStore, GameSaveUseCase gameSaveUseCase)
    {
        this.gameplaySettingStore = gameplaySettingStore;
        this.gameSaveUseCase = gameSaveUseCase;
    }

    public bool IsSkipEnabled()
    {
        return gameplaySettingStore.LoadStorySkipEnabled();
    }

    public void SkipToCurrentPlayingFight()
    {
        GameSceneUseCase.LoadFight(gameSaveUseCase.GetGameSave().playingStage);
    }
}
