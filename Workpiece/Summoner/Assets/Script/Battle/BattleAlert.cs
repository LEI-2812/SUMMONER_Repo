using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleAlert : MonoBehaviour
{
    [Header("전투 결과 알림창")]
    public GameObject alertClear; // 전투 승리 알림창
    public Alert ClearResult; // 전투 승리 알림 결과
    public GameObject alertFail; // 전투 패배 알림창
    public Alert FailResult; // 전투 패배 알림 결과

    [Header("효과음")]
    [SerializeField] private AudioSource clearSound;
    [SerializeField] private AudioSource failSound;

    private StageController stageController;

    private void Start()
    {
        // StageController 인스턴스를 찾아 참조 설정
        stageController = FindObjectOfType<StageController>();
        if (stageController == null)
        {
            Debug.LogError("StageController가 씬에 없습니다. StageController를 추가해 주세요.");
        }
    }

    public void clearAlert(int stageNum)
    {
        alertClear.SetActive(true);
        clearSound.Play();
        ClearResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertClear, ClearResult, (result) => {
            if (result) // 처음부터 다시하기
            {
                Debug.Log("전투를 다시 시작합니다.");
                stageController.SendFightStage(stageController.getStageNum()); //스테이지 컨트롤러의 값으로 시작
            }
            else // 다음 스테이지로
            {
                if (stageNum == 7)
                {
                    Debug.Log("에필로그로 이동합니다.");
                    stageController.SendEpilogue();
                }
                else
                {
                    Debug.Log("다음 스테이지 " + (stageNum + 1) + " 로 이동합니다.");
                    PlayerPrefs.SetInt("playingStage", stageNum + 1);
                    PlayerPrefs.SetInt("savedStage", stageNum + 1); //다음스테이지 번호 저장
                    PlayerPrefs.Save();
                    stageController.SendStage(stageNum + 1);
                }
            }
        }));
    }

    // 패배 시 호출할 함수
    public void failAlert(int stageNum)
    {
        alertFail.SetActive(true);
        failSound.Play();
        FailResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertFail, FailResult, (result) => {
            if (result) // 처음부터 다시하기
            {
                Debug.Log("전투를 다시 시작합니다.");
                stageController.SendFightStage(stageNum);
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
