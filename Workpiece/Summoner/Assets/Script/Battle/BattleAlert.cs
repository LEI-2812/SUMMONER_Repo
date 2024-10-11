using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleAlert : MonoBehaviour
{
    /*
      배틀 씬에서 필요한 코드... 대충 생각해도 겁나 많겠지만 일단 알림창을 띄워보자
      메뉴 창에 있는 것들 그대로, esc 클릭 시 띄우기
      전투 승리, 패배 시 띄울 알림창
     */

    [Header("전투 결과 알림창")]
    public GameObject alertClear; // 전투 승리 알림창
    public Alert ClearResult; // 전투 승리 알림 결과
    public GameObject alertFail; // 전투 패배 알림창
    public Alert FailResult; // 전투 패배 알림 결과

    // 승리 시 호출할 함수 (지금 안되는 이유가 얘를 호출하는 곳이 없어서 그런듯.. 밑의 failAlert도 마찬가지)
    public void clearAlert()
    {
        alertClear.SetActive(true);

        ClearResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertClear, ClearResult, (result) => {
            if (result) // 처음부터 다시하기
            {
                Debug.Log("전투를 다시 시작합니다.");
            }
            else // 다음 스테이지로
            {
                Debug.Log("다음 스테이지로 이동합니다.");
            }
        }));
    }

    // 패배 시 호출할 함수
    public void failAlert()
    {
        alertFail.SetActive(true);

        FailResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertFail, FailResult, (result) => {
            if (result) // 처음부터 다시하기
            {
                Debug.Log("전투를 다시 시작합니다.");
            }
            else // 스테이지 선택화면으로
            {
                Debug.Log("스테이지 선택 화면으로 이동합니다.");
                SceneManager.LoadScene("Stage Select Screen");
            }
        }));
    }
    /*
    // 게임 승리 시 알림 창 활성화 - 현재 전투 시스템 미개발로 인해 후에 추가
    public void winBattle()
    {
        Debug.Log("전투에서 승리하였습니다.");
        alertClear.SetActive(true);
    }

    // 게임 패배 시 알림 창 활성화 - 현재 전투 시스템 미개발로 인해 후에 추가
    public void loseBattle()
    {
        Debug.Log("전투에서 패배하였습니다.");
        alertFail.SetActive(true);
    }

    // alertClear와 alertFail에서 "처음부터 다시하기" 버튼 클릭 시
    public void resetBattle()
    {
        Debug.Log("전투를 다시 시작합니다.");
        // 근데 이것도 배틀의 초기 상태를 정해놔야 나중에 넣을 듯
    }

    // alertClear에서 "다음 스테이지로" 버튼 클릭 시
    public void nextStage()
    {
        Debug.Log("다음 스테이지로 이동합니다.");
        // 이것 역시 다음 스테이지를 구현해놔야 다음 배틀 씬으로 이동하는 코드 넣을듯
        // SceneManager.LoadScene("Fight Screen stage2"); 정도..?
    }

    // alertFail에서 "스테이지 선택 화면으로" 버튼 클릭 시
    public void SelectStage()
    {
        Debug.Log("스테이지 선택 화면으로 이동합니다.");
        SceneManager.LoadScene("Stage Select Screen");  // 스테이지 선택 화면으로 씬 이동
    }
    */
  
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
