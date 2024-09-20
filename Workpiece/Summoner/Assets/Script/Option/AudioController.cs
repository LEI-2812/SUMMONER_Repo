using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    public AudioMixer audioMixer;

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    void Start()
    {
        // 슬라이더 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // 초기값 설정
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    // 마스터 볼륨 설정
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolumes();
        PlayerPrefs.SetFloat("MasterVolume", volume); // 값 저장
    }

    // 배경음 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume; //슬라이더로 움직인 볼륨값을 받아온다
        ApplyVolumes(); //마스터 볼륨값에 따라 재계산받는다.
        PlayerPrefs.SetFloat("BGMVolume", volume); // 값 저장
    }

    // 효과음 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume; //슬라이더로 움직인 볼륨값을 받아온다
        ApplyVolumes(); //마스터 볼륨값에 따라 재계산받는다.
        PlayerPrefs.SetFloat("SFXVolume", volume); // 값 저장
    }

    // 실제 적용할 볼륨 계산
    private void ApplyVolumes()
    {
        // 마스터 볼륨 비율에 따라 개별 볼륨 조정
        float adjustedBGMVolume = bgmVolume * masterVolume;
        float adjustedSFXVolume = sfxVolume * masterVolume;

        // dB로 변환하여 오디오 믹서에 적용
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(adjustedBGMVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(adjustedSFXVolume) * 20);
    }
}
