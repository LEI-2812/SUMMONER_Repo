using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// 역할: 오디오 설정 UI 값을 표시하고 변경 입력을 저장소에 반영한다.
public class AudioSettingView : MonoBehaviour
{
    private const float MutedVolumeDb = -80f;

    [Header("마스터 볼륨 슬라이더")]
    public Slider masterVolumeSlider;
    [Header("BGM 볼륨 슬라이더")]
    public Slider bgmVolumeSlider;
    [Header("SFX 볼륨 슬라이더")]
    public Slider sfxVolumeSlider;

    [Header("오디오 믹서")]
    public AudioMixer audioMixer;

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;
    private readonly AudioSettingStore audioSettingStore = new AudioSettingStore();

    void Start()
    {
        // 슬라이더 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // 초기값 설정
        masterVolumeSlider.value = audioSettingStore.LoadMasterVolume();
        bgmVolumeSlider.value = audioSettingStore.LoadBgmVolume();
        sfxVolumeSlider.value = audioSettingStore.LoadSfxVolume();
    }

    // 마스터 볼륨 설정
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolumes();
        audioSettingStore.SaveMasterVolume(volume);
    }

    // 배경음 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume; //슬라이더로 움직인 볼륨값을 받아온다
        ApplyVolumes(); //마스터 볼륨값에 따라 재계산받는다.
        audioSettingStore.SaveBgmVolume(volume);
    }

    // 효과음 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume; //슬라이더로 움직인 볼륨값을 받아온다
        ApplyVolumes(); //마스터 볼륨값에 따라 재계산받는다.
        audioSettingStore.SaveSfxVolume(volume);
    }

    // 실제 적용할 볼륨 계산
    private void ApplyVolumes()
    {
        // 마스터 볼륨 비율에 따라 개별 볼륨 조정
        float adjustedBGMVolume = bgmVolume * masterVolume;
        float adjustedSFXVolume = sfxVolume * masterVolume;

        // dB로 변환하여 오디오 믹서에 적용
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
