using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: MenuHandler의 책임을 정의한다.
public class MenuHandler : MonoBehaviour
{
    public static MenuHandler instance;

    private readonly ExitGameUseCase exitGameUseCase = new ExitGameUseCase();

    [Header("참조")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject backGroundPanel;

    [Header("참조")]
    [SerializeField] private AudioSource menuClick;
    [SerializeField] private AudioSource alertClick;

    [Header("참조")]
    [SerializeField] private ToMainAlertHandler toMainAlertHandler;
    [SerializeField] private ToQuitAlertHandler toQuitAlertHandler;
    [SerializeField] private SkipAlertHandler skipAlertHandler;

    [Header("참조")]
    [SerializeField] private SettingHandler settingHandler;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (settingHandler != null && settingHandler.IsSettingsOpen())
        {
            settingHandler.CloseSettings();
            return;
        }

        if (!GameSceneUseCase.IsStartScreenActive())
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (menuPanel == null || backGroundPanel == null)
        {
            Debug.LogError("메뉴 패널 또는 배경 패널이 할당되지 않았습니다.");
            return;
        }

        bool isActive = menuPanel.activeSelf;
        menuPanel.SetActive(!isActive);
        backGroundPanel.SetActive(!isActive);
    }

    public void ShowToMainAlert()
    {
        ShowMenuAlert(toMainAlertHandler, "toMainAlertHandler가 할당되지 않았습니다.", () =>
        {
            GameSceneUseCase.LoadStartScreen();
            ToggleMenu();
        });
    }

    public void ShowToQuitAlert()
    {
        ShowMenuAlert(toQuitAlertHandler, "toQuitAlertHandler가 할당되지 않았습니다.", exitGameUseCase.ExitGame);
    }

    public void ShowSkipAlert()
    {
        ShowMenuAlert(skipAlertHandler, "skipAlertHandler가 할당되지 않았습니다.", () =>
        {
            Debug.Log("스킵되었습니다.");
        });
    }

    public void OpenSettings()
    {
        if (settingHandler == null)
        {
            Debug.LogError("settingHandler가 할당되지 않았습니다.");
            return;
        }

        settingHandler.OpenSettings();
    }

    public void CloseSettings()
    {
        if (settingHandler != null)
        {
            settingHandler.CloseSettings();
        }
    }

    private void ShowMenuAlert(BaseAlertHandler alertHandler, string missingMessage, System.Action onConfirmed)
    {
        PlayMenuClick();

        if (alertHandler == null)
        {
            Debug.LogError(missingMessage);
            return;
        }

        alertHandler.ShowAlert(result =>
        {
            if (result)
            {
                onConfirmed();
            }
        });
    }

    private void PlayMenuClick()
    {
        if (menuClick != null)
        {
            menuClick.Play();
        }
    }

}



