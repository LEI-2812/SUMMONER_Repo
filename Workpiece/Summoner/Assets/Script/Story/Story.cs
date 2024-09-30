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

    [Header("메뉴 창")]
    public GameObject menu;

    [Header("메인으로 버튼")]
    public GameObject toMain;
    public Alert toMainResult;

    [Header("나가기 버튼")]
    public GameObject toQuit;
    public Alert toQuitResult;

    [Header("스킵 버튼")]
    public GameObject alertSkip;    // 스킵 창 띄울 오브젝트
    public Alert alertSkipResult;

    private Setting setting;  // SettingMenuController의 인스턴스를 참조



    public void toMainAlert()
    {
        toMain.SetActive(true);

        toMainResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(toMain, toMainResult, (result) => {
            if (result)
            {
                SceneManager.LoadScene("Start Screen");
            }
            else
            {
                // No 버튼 클릭 시 로직
                toMain.SetActive(false);
            }
        }));
    }

    public void toQuitAlert()
    {
        toQuit.SetActive(true);

        toQuitResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(toQuit, toQuitResult, (result) => {
            if (result)
            {
                Debug.Log("게임을 종료합니다.");
                Application.Quit();
            }
            else
            {
                // No 버튼 클릭 시 로직
                toQuit.SetActive(false);
            }
        }));
    }

    public void skipAlert()
    {
        alertSkip.SetActive(true);

        alertSkipResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertSkip, alertSkipResult, (result) => {
            if (result)
            {
                Debug.Log("전투 씬으로 이동");
                SceneManager.LoadScene("Fight Screen");
            }
            else
            {
                toMain.SetActive(false);
            }
        }));
    }
    /*
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
    */
    // 설정창 가져오기 - 다만 현재는 메인화면에서 한번 설정창을 켜야 가져올 수 있음
    public void OpenOptionCanvas()
    {
        if (Setting.instance != null)
        {
            Setting.instance.openOption();
        }
        else
        {
            Debug.LogWarning("SettingMenuController 인스턴스가 존재하지 않습니다.");
        }
    }

    private IEnumerator WaitForAlertResult(GameObject alertObject, Alert alertScript, System.Action<bool> callback)
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
