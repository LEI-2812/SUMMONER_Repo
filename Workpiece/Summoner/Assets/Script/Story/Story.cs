using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    /* 스토리 씬에서 필요한 코드
      - skip 버튼 클릭 시 메뉴창 뜨게 하기
      - 알림창에서 스토리 스킵 yes 시 바로 전투 화면 출력
      - no 클릭 시 알림창 끄기
      - 여기서도 esc 클릭 시 메뉴 창 띄우기 */

    public GameObject menu;
    public GameObject toMain;
    public GameObject toQuit;
    public GameObject alertSkip;    // 스킵 창 띄울 오브젝트

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

    // 스킵 버튼 클릭시 스킵 알림창 띄우기
    public void openSkip()
    {
        alertSkip.SetActive(true);
    }

    // 스토리 스킵 yes 클릭 시 전투 씬으로 보내기
    public void gotoBattle()
    {
        Debug.Log("전투 씬으로 이동");
        // 이거 일단 지금 있는 파이트 씬으로 이동을 해놔야 하나..?
        SceneManager.LoadScene("Fight Screen");
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
