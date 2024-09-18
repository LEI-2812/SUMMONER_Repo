using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{ 
    public GameObject menu;
    public GameObject toMain;
    public GameObject toQuit;

    // esc 키를 계속 감지하여 눌릴 때 메뉴창 열기/닫기
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            openMenu();
        }
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
