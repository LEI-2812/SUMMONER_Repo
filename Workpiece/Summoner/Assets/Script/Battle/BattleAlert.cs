using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleAlert : MonoBehaviour
{
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
