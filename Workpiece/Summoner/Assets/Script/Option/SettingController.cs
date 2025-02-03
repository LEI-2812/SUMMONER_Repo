using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    [Header("오디오 설정")]
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioMixer audioMixer;

    [Header("비디오 설정")]
    public List<Toggle> resolutionToggles;
    public List<Toggle> screenModeToggles;

    [Header("게임 플레이 설정")]
    public Toggle isStorySkip;
    public Toggle isOnlyMouse;

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private readonly List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    private readonly List<FullScreenMode> screenModes = new List<FullScreenMode>
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };


    private void Awake()
    {
        LoadSettings();

        // 슬라이더 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // 토글 이벤트 등록
        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            int index = i;
            resolutionToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(resolutionToggles, index, true); });
        }

        for (int i = 0; i < screenModeToggles.Count; i++)
        {
            int index = i;
            screenModeToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(screenModeToggles, index, false); });
        }

        isStorySkip.onValueChanged.AddListener(delegate { SaveGamePlaySettings(); });
        isOnlyMouse.onValueChanged.AddListener(delegate { SaveGamePlaySettings(); });
    }

    public void LoadSettings()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        int savedScreenModeIndex = PlayerPrefs.GetInt("screenModeIndex", 0);

        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            resolutionToggles[i].isOn = (i == savedResolutionIndex);
        }

        for (int i = 0; i < screenModeToggles.Count; i++)
        {
            screenModeToggles[i].isOn = (i == savedScreenModeIndex);
        }

        isStorySkip.isOn = PlayerPrefs.GetInt("IsStorySkip", 0) == 1;
        isOnlyMouse.isOn = PlayerPrefs.GetInt("IsOnlyMouse", 0) == 1;

        ApplyVolumes();
    }

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        ApplyVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("BGMVolume", volume);
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        ApplyVolumes();
    }

    public void ApplyVolumes()
    {
        float masterVolume = masterVolumeSlider.value;
        float adjustedBGMVolume = bgmVolumeSlider.value * masterVolume;
        float adjustedSFXVolume = sfxVolumeSlider.value * masterVolume;

        audioMixer.SetFloat("BGMVolume", Mathf.Log10(adjustedBGMVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(adjustedSFXVolume) * 20);
    }

    void OnToggleChanged(List<Toggle> toggles, int index, bool isResolution)
    {
        audioSource.Play();
        if (toggles[index].isOn)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (i != index)
                {
                    toggles[i].isOn = false;
                }
            }

            if (isResolution)
            {
                SetResolution(resolutions[index].x, resolutions[index].y);
                PlayerPrefs.SetInt("resolutionIndex", index);
            }
            else
            {
                SetScreenMode(screenModes[index]);
                PlayerPrefs.SetInt("screenModeIndex", index);
            }
            PlayerPrefs.Save();
        }
    }

    void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }

    void SetScreenMode(FullScreenMode mode)
    {
        Screen.fullScreenMode = mode;
    }

    public void SaveGamePlaySettings()
    {
        PlayerPrefs.SetInt("IsStorySkip", isStorySkip.isOn ? 1 : 0);
        PlayerPrefs.SetInt("IsOnlyMouse", isOnlyMouse.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
