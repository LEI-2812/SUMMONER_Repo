using UnityEngine;

// 역할: 오디오 설정 값을 저장하고 다시 불러오는 저장소 역할을 한다.
public class AudioSettingStore
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string BgmVolumeKey = "BGMVolume";
    private const string SfxVolumeKey = "SFXVolume";

    public float LoadMasterVolume()
    {
        return PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
    }

    public void SaveMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public float LoadBgmVolume()
    {
        return PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
    }

    public void SaveBgmVolume(float volume)
    {
        PlayerPrefs.SetFloat(BgmVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public float LoadSfxVolume()
    {
        return PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
    }

    public void SaveSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, volume);
        PlayerPrefs.Save();
    }
}
