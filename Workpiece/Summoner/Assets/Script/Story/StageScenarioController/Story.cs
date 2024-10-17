using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    [Header("스킵하시겠습니까? 창")]
    public GameObject alertSkip;    // 스킵 창 띄울 오브젝트
    public Alert alertSkipResult; 
    public GameObject SkipBtn; //Button 으로 하면 비활성화가 안됨

    private Setting setting;  // SettingMenuController의 인스턴스를 참조
    private string playingStage;   // 플레이 중인 스테이지 번호

    private void Start()
    {
        // Setting 싱글톤 인스턴스를 참조
        setting = Setting.instance;

        if (setting == null)
        {
            Debug.LogWarning("SettingMenuController 인스턴스가 존재하지 않습니다.");
        }

        playingStage = PlayerPrefs.GetInt("playingStage", 0).ToString();
        isSkipActive(); //스킵버튼 활성화 여부 판별
    }

    private void Update()
    {
        //버튼이 활성화 되어있고 그상태에서 엔터를 누르면 스킵창 출력
        if (Input.GetKeyDown(KeyCode.Return) && SkipBtn.activeSelf==true)
        {
            skipAlert();
        }
    }

    public void isSkipActive() //Setting에 스킵토글에 따라 스킵버튼 활성화 여부 동작 메소드
    {
        if(setting != null) //null인지 검사
        {
            //setting의 게임 컨트롤러를 가져와서 스토리스킵 토글이 활성화 상태인지 검사
            if ( setting.GetGamePlayController().getIsStorySkip() == true)
            {
                SkipBtn.SetActive(true);
            }
            else { SkipBtn.SetActive(false); } //아니라면 비활성화
        }
    }

    public void skipAlert() //스킵 Alert 동작 메소드
    {
        alertSkip.SetActive(true);

        alertSkipResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertSkip, alertSkipResult, (result) => {
            if (result)
            {
                Debug.Log("스킵하여 전투 씬으로 이동");
                string sceneName = "Fight Screen_" + playingStage + "Stage";
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                alertSkip.SetActive(false);
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
