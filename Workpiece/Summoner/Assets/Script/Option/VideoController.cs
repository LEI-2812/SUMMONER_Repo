using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    // �ػ� ��� ����Ʈ
    [Header("�ػ� ���")]
    public List<Toggle> resolutionToggles;

    // ȭ�� ���� ��� ����Ʈ
    [Header("ȭ��ũ�� ���")]
    public List<Toggle> screenModeToggles;

    [Header("��ư Ŭ����")]
    [SerializeField] private AudioSource audioSource;

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

    void Awake()
    {
        // ����� �ػ󵵿� ȭ�� ��� �ҷ�����
        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        int savedScreenModeIndex = PlayerPrefs.GetInt("screenModeIndex", 0);

        // �ػ� ��� �ʱ�ȭ
        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            int index = i;
            resolutionToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(resolutionToggles, index, true); });

            // ����� �ػ� �ε����� �������� �ʱ�ȭ
            resolutionToggles[i].isOn = (i == savedResolutionIndex);
        }

        // ȭ�� ���� ��� �ʱ�ȭ
        for (int i = 0; i < screenModeToggles.Count; i++)
        {
            int index = i;
            screenModeToggles[i].onValueChanged.AddListener(delegate { OnToggleChanged(screenModeToggles, index, false); });

            // ����� ȭ�� ��� �ε����� �������� �ʱ�ȭ
            screenModeToggles[i].isOn = (i == savedScreenModeIndex);
        }
    }

    // ����� ����� �� ȣ��Ǵ� �޼ҵ�
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

            // ���� ���� �� ����
            if (isResolution)
            {
                SetResolution(resolutions[index].x, resolutions[index].y);
                PlayerPrefs.SetInt("resolutionIndex", index);  // �ػ� �ε��� ����
            }
            else
            {
                SetScreenMode(screenModes[index]);
                PlayerPrefs.SetInt("screenModeIndex", index);  // ȭ�� ��� �ε��� ����
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
