using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// 역할: AudioSettingView의 책임을 정의한다.
public class AudioSettingView : MonoBehaviour
{
    private const float MutedVolumeDb = -80f;

    [Header("참조")]
    public Slider masterVolumeSlider;
    [Header("참조")]
    public Slider bgmVolumeSlider;
    [Header("참조")]
    public Slider sfxVolumeSlider;

    [Header("참조")]
    public AudioMixer audioMixer;

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;
    private readonly AudioSettingStore audioSettingStore = new AudioSettingStore();

    void Start()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        masterVolumeSlider.value = audioSettingStore.LoadMasterVolume();
        bgmVolumeSlider.value = audioSettingStore.LoadBgmVolume();
        sfxVolumeSlider.value = audioSettingStore.LoadSfxVolume();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolumes();
        audioSettingStore.SaveMasterVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        ApplyVolumes();
        audioSettingStore.SaveBgmVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        ApplyVolumes();
        audioSettingStore.SaveSfxVolume(volume);
    }

    private void ApplyVolumes()
    {
        float adjustedBGMVolume = bgmVolume * masterVolume;
        float adjustedSFXVolume = sfxVolume * masterVolume;

        audioMixer.SetFloat("BGMVolume", VolumeToDecibel(adjustedBGMVolume));
        audioMixer.SetFloat("SFXVolume", VolumeToDecibel(adjustedSFXVolume));
    }

    private static float VolumeToDecibel(float volume)
    {
        if (volume <= 0f)
        {
            return MutedVolumeDb;
        }

        return Mathf.Log10(volume) * 20;
    }
}
