using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuView : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static MenuView instance; //최상위 오브젝트에 있는 스크립트

    [Header("메뉴 판넬")]
    [SerializeField] private GameObject menuPanel;

    [Space]

    [Header("메인화면이동")]
    [SerializeField] private GameObject toMain; //메인으로 ConfirmAlertView
    [SerializeField] private ConfirmAlertView toMainResult; //메인 ConfirmAlertView 로직

    [Space]

    [Header("설정창")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private SettingPanelView setting;

    [Space]

<<<<<<<< Updated upstream:Workpiece/Summoner/Assets/Script/Menu/안쓰는스크립트/Menu.cs
    [Header("��ŵ�Ͻðڽ��ϱ�? â")]
    public GameObject alertSkip;    // ��ŵ â ��� ������Ʈ
    public Alert alertSkipResult;

    [Space]

    [Header("��������")]
    [SerializeField] private GameObject toQuit; //������ �˸�â
    [SerializeField] private Alert toQuitResult; //������ Alert ����
========
    [Header("게임종료")]
    [SerializeField] private GameObject toQuit; //나가기 알림창
    [SerializeField] private ConfirmAlertView toQuitResult; //나가기 ConfirmAlertView 로직
>>>>>>>> Stashed changes:Workpiece/Summoner/Assets/Script/Menu/MenuView.cs

    [Header("백그라운드 배경")]
    [SerializeField ]private GameObject backGroundPanel;

    [Header("클릭음 사운드")]
    public AudioSource menuClick;
    public AudioSource alertClick;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
<<<<<<<< Updated upstream:Workpiece/Summoner/Assets/Script/Menu/안쓰는스크립트/Menu.cs
========
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬 전환 시 파괴하지 않도록 설정
>>>>>>>> Stashed changes:Workpiece/Summoner/Assets/Script/Menu/MenuView.cs
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면, 새로 생성된 오브젝트 파괴
        }
    }

    // esc 키를 계속 감지하여 눌릴 때 메뉴창 열기/닫기
    void Update()
    {
        //ESC입력은 StartScreen에선 안먹히게 적용
        if (Input.GetKeyDown(KeyCode.Escape) && ! (SceneManager.GetActiveScene().name == "Start Screen"))
        {
            if (!setting.settingPanel.activeSelf)
                openCloseMenu();
        }
    }


    public void openCloseMenu()
    {
        if (menuPanel.activeSelf == true)
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
    public void toMainAlert()
    {
        toMain.SetActive(true); // 알림창 활성화
        menuClick.Play();
        // 알림창 상태를 초기화
        toMainResult.ResetAlert();
        // 코루틴 실행: newAlert에 대한 처리
        StartCoroutine(WaitForAlertResult(toMain, toMainResult, (result) => {
            if (result)
            {
                alertClick.Play();
                SceneManager.LoadScene("Start Screen");
                openCloseMenu();
            }
            else
            {
                // No 버튼 클릭 시 로직
                alertClick.Play();
                toMain.SetActive(false);
            }
        }));
    }

    public void toQuitAlert()
    {
        toQuit.SetActive(true); // 알림창 활성화
        menuClick.Play();
        // 알림창 상태를 초기화
        toQuitResult.ResetAlert();
        // 코루틴 실행: newAlert에 대한 처리
        StartCoroutine(WaitForAlertResult(toQuit, toQuitResult, (result) => {
            if (result)
            {
                alertClick.Play();
                Debug.Log("게임을 종료합니다.");
                Application.Quit();
            }
            else
            {
                // No 버튼 클릭 시 로직
                alertClick.Play();
                toQuit.SetActive(false);
            }
        }));
    }

<<<<<<<< Updated upstream:Workpiece/Summoner/Assets/Script/Menu/안쓰는스크립트/Menu.cs

    // ����â ��������
========
    // 설정창 가져오기 - 다만 현재는 메인화면에서 한번 설정창을 켜야 가져올 수 있음
>>>>>>>> Stashed changes:Workpiece/Summoner/Assets/Script/Menu/MenuView.cs
    public void openSettingCanvas()
    {
        setting.openOption();
        menuClick.Play();
    }

    
    private IEnumerator WaitForAlertResult(GameObject alertObject, ConfirmAlertView alertScript, System.Action<bool> callback)
    {
        // 알림창을 활성화
        alertObject.SetActive(true);

        // 사용자가 버튼을 클릭할 때까지 대기
        while (!alertScript.getIsClicked())
        {
            yield return null;  // 한 프레임 대기
        }

        // 알림창 비활성화
        alertObject.SetActive(false);

        // 버튼 클릭 후 결과 콜백 호출 (true: Yes, false: No)
        callback(alertScript.getResult());
    }
}