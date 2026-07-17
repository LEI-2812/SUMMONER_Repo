// 역할: 화면 설정값의 조회와 저장 흐름을 담당한다.
public class VideoSettingUseCase
{
    private readonly VideoSettingStore videoSettingStore = new VideoSettingStore();

    public int LoadResolutionIndex(int itemCount)
    {
        return videoSettingStore.LoadValidResolutionIndex(itemCount);
    }

    public int LoadScreenModeIndex(int itemCount)
    {
        return videoSettingStore.LoadValidScreenModeIndex(itemCount);
    }

    public void SaveResolutionIndex(int index)
    {
        videoSettingStore.SaveResolutionIndex(index);
    }

    public void SaveScreenModeIndex(int index)
    {
        videoSettingStore.SaveScreenModeIndex(index);
    }
}
