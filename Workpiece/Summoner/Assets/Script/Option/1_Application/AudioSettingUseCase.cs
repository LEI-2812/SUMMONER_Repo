// 역할: 오디오 설정값의 조회와 저장 흐름을 담당한다.
public class AudioSettingUseCase
{
    private readonly AudioSettingStore audioSettingStore = new AudioSettingStore();

    public float LoadMasterVolume()
    {
        return audioSettingStore.LoadMasterVolume();
    }

    public float LoadBgmVolume()
    {
        return audioSettingStore.LoadBgmVolume();
    }

    public float LoadSfxVolume()
    {
        return audioSettingStore.LoadSfxVolume();
    }

    public void SaveMasterVolume(float volume)
    {
        audioSettingStore.SaveMasterVolume(volume);
    }

    public void SaveBgmVolume(float volume)
    {
        audioSettingStore.SaveBgmVolume(volume);
    }

    public void SaveSfxVolume(float volume)
    {
        audioSettingStore.SaveSfxVolume(volume);
    }
}
