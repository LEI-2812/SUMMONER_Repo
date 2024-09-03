using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    // 해상도 토글 리스트
    public List<Toggle> resolutionToggles;

    // 화면 설정 토글 리스트
    public List<Toggle> screenModeToggles;

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

    void Start()
    {
        // 해상도 토글 초기화
        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스 저장
            resolutionToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(resolutionToggles, index, true); });
        }

        // 화면 설정 토글 초기화
        for (int i = 0; i < screenModeToggles.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스 저장
            screenModeToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(screenModeToggles, index, false); });
        }

        // 초기값 설정 (예: 첫 번째 토글 활성화)
        if (resolutionToggles.Count > 0) resolutionToggles[0].isOn = true;
        if (screenModeToggles.Count > 0) screenModeToggles[0].isOn = true;
    }

    // 토글이 변경될 때 호출되는 메소드
    void OnToggleChanged(List<Toggle> toggles, int index, bool isResolution)
    {
        if (toggles[index].isOn)
        {
            // 해당 인덱스 제외한 나머지 토글을 비활성화
            for (int i = 0; i < toggles.Count; i++)
            {
                if (i != index)
                {
                    toggles[i].isOn = false;
                }
            }

            // 선택된 설정 적용
            if (isResolution)
            {
                SetResolution(resolutions[index].x, resolutions[index].y);
            }
            else
            {
                SetScreenMode(screenModes[index]);
            }
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
