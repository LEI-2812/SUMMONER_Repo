using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuView : MonoBehaviour
{
    public static MenuView instance;

    [Header("메뉴 패널")]
    [SerializeField] private GameObject menuPanel;

    [Header("메인 화면 이동")]
    [SerializeField] private GameObject toMain;
    [SerializeField] private ConfirmAlertView toMainResult;

    [Header("설정창")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private SettingPanelView setting;

    [Header("게임종료")]
    [SerializeField] private GameObject toQuit;
    [SerializeField] private ConfirmAlertView toQuitResult;

    [Header("백그라운드 배경")]
    [SerializeField] private GameObject backGroundPanel;

    [Header("클릭 사운드")]
    public AudioSource menuClick;
    public AudioSource alertClick;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Start Screen")
        {
            if (setting == null || !setting.settingPanel.activeSelf)
            {
                OpenCloseMenu();
            }
        }
    }

    public void OpenCloseMenu()
    {
        if (menuPanel.activeSelf)
        {
            backGroundPanel.SetActive(false);
            menuPanel.SetActive(false);
        }
        else
        {
            menuPanel.SetActive(true);
            backGroundPanel.SetActive(true);
        }
    }

    public void ToMainAlert()
    {
        toMain.SetActive(true);
        menuClick.Play();
        toMainResult.ResetAlert();

        StartCoroutine(WaitForAlertResult(toMain, toMainResult, result =>
        {
            if (result)
            {
                alertClick.Play();
                SceneManager.LoadScene("Start Screen");
                OpenCloseMenu();
            }
            else
            {
                alertClick.Play();
                toMain.SetActive(false);
            }
        }));
    }

    public void ToQuitAlert()
    {
        toQuit.SetActive(true);
        menuClick.Play();
        toQuitResult.ResetAlert();

        StartCoroutine(WaitForAlertResult(toQuit, toQuitResult, result =>
        {
            if (result)
            {
                alertClick.Play();
                Debug.Log("게임을 종료합니다.");
                Application.Quit();
            }
            else
            {
                alertClick.Play();
                toQuit.SetActive(false);
            }
        }));
    }

    public void OpenSettingCanvas()
    {
        setting.OpenOption();
        menuClick.Play();
    }

    private IEnumerator WaitForAlertResult(GameObject alertObject, ConfirmAlertView alertScript, System.Action<bool> callback)
    {
        alertObject.SetActive(true);

        while (!alertScript.GetIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);
        callback(alertScript.GetResult());
    }
}
