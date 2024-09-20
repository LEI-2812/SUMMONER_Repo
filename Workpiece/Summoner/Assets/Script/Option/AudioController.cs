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
        // �����̴� �̺�Ʈ ���
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // �ʱⰪ ����
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    // ������ ���� ����
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolumes();
        PlayerPrefs.SetFloat("MasterVolume", volume); // �� ����
    }

    // ����� ���� ����
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume; //�����̴��� ������ �������� �޾ƿ´�
        ApplyVolumes(); //������ �������� ���� ����޴´�.
        PlayerPrefs.SetFloat("BGMVolume", volume); // �� ����
    }

    // ȿ���� ���� ����
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume; //�����̴��� ������ �������� �޾ƿ´�
        ApplyVolumes(); //������ �������� ���� ����޴´�.
        PlayerPrefs.SetFloat("SFXVolume", volume); // �� ����
    }

    // ���� ������ ���� ���
    private void ApplyVolumes()
    {
        // ������ ���� ������ ���� ���� ���� ����
        float adjustedBGMVolume = bgmVolume * masterVolume;
        float adjustedSFXVolume = sfxVolume * masterVolume;

        // dB�� ��ȯ�Ͽ� ����� �ͼ��� ����
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(adjustedBGMVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(adjustedSFXVolume) * 20);
    }
}
