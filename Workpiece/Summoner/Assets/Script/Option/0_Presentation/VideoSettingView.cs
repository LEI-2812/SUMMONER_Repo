using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: VideoSettingView의 책임을 정의한다.
public class VideoSettingView : MonoBehaviour
{
    [Header("참조")]
    public List<Toggle> resolutionToggles;

    [Header("참조")]
    public List<Toggle> screenModeToggles;

    [Header("참조")]
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

    private readonly VideoSettingStore videoSettingStore = new VideoSettingStore();

    void Awake()
    {
        int savedResolutionIndex = videoSettingStore.LoadValidResolutionIndex(resolutionToggles.Count);
        int savedScreenModeIndex = videoSettingStore.LoadValidScreenModeIndex(screenModeToggles.Count);

        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            int index = i;
            resolutionToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(resolutionToggles, index, true); });

            resolutionToggles[i].isOn = (i == savedResolutionIndex);
        }

        for (int i = 0; i < screenModeToggles.Count; i++)
        {
            int index = i;
            screenModeToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(screenModeToggles, index, false); });

            screenModeToggles[i].isOn = (i == savedScreenModeIndex);
        }
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
                videoSettingStore.SaveResolutionIndex(index);
            }
            else
            {
                SetScreenMode(screenModes[index]);
                videoSettingStore.SaveScreenModeIndex(index);
            }
        }
    }

    void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode);
        Debug.Log($"해상도를 설정했습니다: {width}x{height}");
    }

    void SetScreenMode(FullScreenMode mode)
    {
        Screen.fullScreenMode = mode;
        Debug.Log($"화면 모드를 설정했습니다: {mode}");
    }

}
