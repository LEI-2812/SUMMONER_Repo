using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler instance;

    [Header("�޴� �г� �� ���")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject backGroundPanel;

    [Header("����")]
    [SerializeField] private AudioSource menuClick;
    [SerializeField] private AudioSource alertClick;

    [Header("�˸� �ڵ鷯��")]
    [SerializeField] private ToMainAlertHandler toMainAlertHandler;
    [SerializeField] private ToQuitAlertHandler toQuitAlertHandler;
    [SerializeField] private SkipAlertHandler skipAlertHandler;

    [Header("���� �ڵ鷯")]
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
            Debug.LogError("�޴� �г� �Ǵ� ��� �г��� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.LogError("toMainAlertHandler�� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.LogError("toQuitAlertHandler�� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.LogError("skipAlertHandler�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        skipAlertHandler.ShowAlert(result =>
        {
            if (result)
            {
                Debug.Log("��ŵ�Ǿ����ϴ�.");
            }
        });
    }

    public void OpenSettings()
    {
        if (settingHandler == null)
        {
            Debug.LogError("settingHandler�� �Ҵ���� �ʾҽ��ϴ�.");
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
