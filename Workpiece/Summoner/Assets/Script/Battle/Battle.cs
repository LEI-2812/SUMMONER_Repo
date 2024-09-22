using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Battle : MonoBehaviour
{
    /*
      배틀 씬에서 필요한 코드... 대충 생각해도 겁나 많겠지만 일단 알림창을 띄워보자
      메뉴 창에 있는 것들 그대로, esc 클릭 시 띄우기
      전투 승리, 패배 시 띄울 알림창
     */

    public GameObject menu;
    public GameObject toMain;
    public GameObject toQuit;
    public GameObject alertClear;   // 전투 승리 시 띄울 오브젝트
    public GameObject alertFail;    // 전투 패배 시 띄울 오브젝트

    private GameObject setting;
    private SettingMenuController settingController;  // SettingMenuController의 인스턴스를 참조
    private void Start()
    {
        getSettingMenuController(); //SettingMenuController인스턴스 가져오기
    }
    // esc 키를 계속 감지하여 눌릴 때 메뉴창 열기/닫기
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settingController.option.activeSelf)
                openMenu();
        }
    }

    //SettingMenuController 인스턴스 받아오는 메소드
    private void getSettingMenuController()
    {
        settingController = SettingMenuController.instance;
        if (setting == null)
        {
            Debug.LogWarning("SettingMenuController 인스턴스를 찾을 수 없습니다.");
        }
    }

    // 게임 승리 시 알림 창 활성화 - 현재 전투 시스템 미개발로 인해 후에 추가
    public void winBattle()
    {
        Debug.LogWarning("전투에서 승리하였습니다.");
        alertClear.SetActive(true);
    }

    // 게임 패배 시 알림 창 활성화 - 현재 전투 시스템 미개발로 인해 후에 추가
    public void loseBattle()
    {
        Debug.LogWarning("전투에서 패배하였습니다.");
        alertFail.SetActive(true);
    }

    // alertClear와 alertFail에서 "처음부터 다시하기" 버튼 클릭 시
    public void resetBattle()
    {
        Debug.LogWarning("전투를 다시 시작합니다.");
        // 근데 이것도 배틀의 초기 상태를 정해놔야 나중에 넣을 듯
    }

    // alertClear에서 "다음 스테이지로" 버튼 클릭 시
    public void nextStage()
    {
        Debug.LogWarning("다음 스테이지로 이동합니다.");
        // 이것 역시 다음 스테이지를 구현해놔야 다음 배틀 씬으로 이동하는 코드 넣을듯
        // SceneManager.LoadScene("Fight Screen stage2"); 정도..?
    }

    // alertFail에서 "스테이지 선택 화면으로" 버튼 클릭 시
    public void SelectStage()
    {
        Debug.LogWarning("스테이지 선택 화면으로 이동합니다.");
        SceneManager.LoadScene("Stage Select Screen");  // 스테이지 선택 화면으로 씬 이동
    }

    public void openMenu()
    {
        if (menu.activeSelf == true)
            menu.SetActive(false);
        else
            menu.SetActive(true);
    }

    // 메인 화면 가기 전 세이브 알림창
    public void checkSave()
    {
        toMain.SetActive(true);
    }

    // 화면 끄기 전 세이브 알림창
    public void checkSave2()
    {
        toQuit.SetActive(true);
    }

    // 메인 화면으로 이동
    public void gotoMain()
    {
        SceneManager.LoadScene("Start Screen");
        menu.SetActive(false); // 스테이지 선택 화면으로 이동과 동시에 알림창 비활성화
        toMain.SetActive(false);
    }

    // 설정창 가져오기 - 다만 현재는 메인화면에서 한번 설정창을 켜야 가져올 수 있음
    public void OpenOptionCanvas()
    {
        if (SettingMenuController.instance != null)
        {
            SettingMenuController.instance.openOption();
        }
        else
        {
            Debug.LogWarning("SettingMenuController 인스턴스가 존재하지 않습니다.");
        }
    }

    //게임 종료
    public void ExitGame()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit();
    }
}
