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
    private readonly AudioSettingUseCase audioSettingUseCase = new AudioSettingUseCase();

    void Start()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        masterVolumeSlider.value = audioSettingUseCase.LoadMasterVolume();
        bgmVolumeSlider.value = audioSettingUseCase.LoadBgmVolume();
        sfxVolumeSlider.value = audioSettingUseCase.LoadSfxVolume();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolumes();
        audioSettingUseCase.SaveMasterVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        ApplyVolumes();
        audioSettingUseCase.SaveBgmVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        ApplyVolumes();
        audioSettingUseCase.SaveSfxVolume(volume);
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
