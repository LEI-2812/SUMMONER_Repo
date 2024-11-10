using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    // 해상도 토글 리스트
    [Header("해상도 토글")]
    public List<Toggle> resolutionToggles;

    // 화면 설정 토글 리스트
    [Header("화면크기 토글")]
    public List<Toggle> screenModeToggles;

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    // 해상도 설정 값들
    private readonly List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    // 화면 모드 설정 값들
    private readonly List<FullScreenMode> screenModes = new List<FullScreenMode>
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };

    void Awake()
    {
        // 저장된 해상도와 화면 모드 불러오기
        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        int savedScreenModeIndex = PlayerPrefs.GetInt("screenModeIndex", 0);

        // 해상도 토글 초기화
        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            int index = i;
            resolutionToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(resolutionToggles, index, true); });

            // 저장된 해상도 인덱스를 기준으로 초기화
            resolutionToggles[i].isOn = (i == savedResolutionIndex);
        }

        // 화면 설정 토글 초기화
        for (int i = 0; i < screenModeToggles.Count; i++)
        {
            int index = i;
            screenModeToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(screenModeToggles, index, false); });

            // 저장된 화면 모드 인덱스를 기준으로 초기화
            screenModeToggles[i].isOn = (i == savedScreenModeIndex);
        }
    }

    // 토글이 변경될 때 호출되는 메소드
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

            // 설정 저장 및 적용
            if (isResolution)
            {
                SetResolution(resolutions[index].x, resolutions[index].y);
                PlayerPrefs.SetInt("resolutionIndex", index);  // 해상도 인덱스 저장
            }
            else
            {
                SetScreenMode(screenModes[index]);
                PlayerPrefs.SetInt("screenModeIndex", index);  // 화면 모드 인덱스 저장
            }
            PlayerPrefs.Save();
        }
    }

    void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode);
        Debug.Log($"Resolution set to: {width}x{height}");
    }

    void SetScreenMode(FullScreenMode mode)
    {
        Screen.fullScreenMode = mode;
        Debug.Log($"Screen mode set to: {mode}");
    }
}
