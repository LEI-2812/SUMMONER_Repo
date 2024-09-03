using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    // �ػ� ��� ����Ʈ
    public List<Toggle> resolutionToggles;

    // ȭ�� ���� ��� ����Ʈ
    public List<Toggle> screenModeToggles;

    // �ػ� ���� ����
    private readonly List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    // ȭ�� ��� ���� ����
    private readonly List<FullScreenMode> screenModes = new List<FullScreenMode>
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };

    void Start()
    {
        // �ػ� ��� �ʱ�ȭ
        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            int index = i; // ���� ������ �ε��� ����
            resolutionToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(resolutionToggles, index, true); });
        }

        // ȭ�� ���� ��� �ʱ�ȭ
        for (int i = 0; i < screenModeToggles.Count; i++)
        {
            int index = i; // ���� ������ �ε��� ����
            screenModeToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(screenModeToggles, index, false); });
        }

        // �ʱⰪ ���� (��: ù ��° ��� Ȱ��ȭ)
        if (resolutionToggles.Count > 0) resolutionToggles[0].isOn = true;
        if (screenModeToggles.Count > 0) screenModeToggles[0].isOn = true;
    }

    // ����� ����� �� ȣ��Ǵ� �޼ҵ�
    void OnToggleChanged(List<Toggle> toggles, int index, bool isResolution)
    {
        if (toggles[index].isOn)
        {
            // �ش� �ε��� ������ ������ ����� ��Ȱ��ȭ
            for (int i = 0; i < toggles.Count; i++)
            {
                if (i != index)
                {
                    toggles[i].isOn = false;
                }
            }

            // ���õ� ���� ����
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
