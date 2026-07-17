// 역할: GameplaySettingUseCase의 책임을 정의한다.
public class GameplaySettingUseCase
{
    private readonly GameplaySettingStore gameplaySettingStore = new GameplaySettingStore();

    public bool LoadStorySkipEnabled()
    {
        return gameplaySettingStore.LoadStorySkipEnabled();
    }

    public void SaveStorySkipEnabled(bool isEnabled)
    {
        gameplaySettingStore.SaveStorySkipEnabled(isEnabled);
    }

    public bool LoadOnlyMouseEnabled()
    {
        return gameplaySettingStore.LoadOnlyMouseEnabled();
    }

    public void SaveOnlyMouseEnabled(bool isEnabled)
    {
        gameplaySettingStore.SaveOnlyMouseEnabled(isEnabled);
    }
}
