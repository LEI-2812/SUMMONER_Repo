using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler instance;

    [Header("메뉴 패널 및 배경")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject backGroundPanel;

    [Header("사운드")]
    [SerializeField] private AudioSource menuClick;
    [SerializeField] private AudioSource alertClick;

    [Header("알림 핸들러들")]
    [SerializeField] private ToMainAlertHandler toMainAlertHandler;
    [SerializeField] private ToQuitAlertHandler toQuitAlertHandler;
    [SerializeField] private SkipAlertHandler skipAlertHandler;

    [Header("설정 핸들러")]
    [SerializeField] private SettingHandler settingHandler;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && settingHandler.settingPanel.activeSelf)
        {
            settingHandler.settingPanel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Start Screen" && !settingHandler.settingPanel.activeSelf)
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
        if (menuClick != null)
        {
            menuClick.Play();
        }

        if (toMainAlertHandler == null)
        {
            Debug.LogError("toMainAlertHandler가 할당되지 않았습니다.");
            return;
        }

        toMainAlertHandler.ShowAlert(result =>
        {
            if (result)
            {
                SceneManager.LoadScene("Start Screen");
                ToggleMenu();
            }
        });
    }

    public void ShowToQuitAlert()
    {
        if (menuClick != null)
        {
            menuClick.Play();
        }

        if (toQuitAlertHandler == null)
        {
            Debug.LogError("toQuitAlertHandler가 할당되지 않았습니다.");
            return;
        }

        toQuitAlertHandler.ShowAlert(result =>
        {
            if (result)
            {
                Application.Quit();
            }
        });
    }

    public void ShowSkipAlert()
    {
        if (menuClick != null)
        {
            menuClick.Play();
        }

        if (skipAlertHandler == null)
        {
            Debug.LogError("skipAlertHandler가 할당되지 않았습니다.");
            return;
        }

        skipAlertHandler.ShowAlert(result =>
        {
            if (result)
            {
                Debug.Log("스킵되었습니다.");
            }
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

}
