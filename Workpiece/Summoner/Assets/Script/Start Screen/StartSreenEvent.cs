using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSreenEvent : MonoBehaviour
{
    private GameObject optionCanvasAudio;
    public GameObject newAlert;
    public Alert newAlertResult;

    public GameObject loadAlert;
    public Alert loadAlertResult;

    public Button settingBtn;

    public AudioSource audioSource;

    void Start()
    {
        // OptionCanvas_Audio 오브젝트를 씬에서 찾아서 참조
        optionCanvasAudio = GameObject.Find("OptionCanvas_Audio");

        // 만약 씬에 오브젝트가 없을 경우 오류 방지
        if (optionCanvasAudio == null)
        {
            Debug.LogError("OptionCanvas_Audio 오브젝트가 없음.");
        }

        // Setting 버튼 클릭 이벤트를 연결
        if (settingBtn != null)
        {
            settingBtn.onClick.AddListener(openOption); // 버튼에 openOption 이벤트 연결
        }
        else{ Debug.LogError("Setting 버튼이 연결되지 않았습니다."); }

        newAlert.SetActive(false);
        loadAlert.SetActive(false);
        int savedStageValue = PlayerPrefs.GetInt("savedStage");
        Debug.Log("savedStage 값 : " + savedStageValue);
    }

    //새게임
    public void NewStart()
    {
        audioSource.Play();
        newAlert.SetActive(true); // 알림창 활성화

        // 알림창 상태를 초기화
        newAlertResult.ResetAlert();
        // 코루틴 실행: newAlert에 대한 처리
        StartCoroutine(WaitForAlertResult(newAlert, newAlertResult, (result) => {
            if (result)
            {
                
                // Yes 버튼 클릭 시 로직
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("savedStage", 1); // 스테이지 진행 상황 초기화
                PlayerPrefs.Save();
                Debug.Log("저장되어있던 데이터를 모두 삭제후 새게임 시작");
                SceneManager.LoadScene("Stage Select Screen");
            }
            else
            {
                // No 버튼 클릭 시 로직
                newAlert.SetActive(false);
            }
        }));
    }

    //이어하기
    public void StartSavedStage() 
    {
        audioSource.Play();
        loadAlert.SetActive(true); // 알림창 활성화

        int savedStage = LoadStage();
        loadAlertResult.ResetAlert();

        StartCoroutine(WaitForAlertResult(loadAlert, loadAlertResult, (result) => {
            if (result)
            {
                // Yes 버튼 클릭 시 로직
                // Debug.Log("저장된 스테이지 " + savedStage + "로 이동");
                Debug.Log("저장된 스테이지로 이동");
                PlayerPrefs.GetInt("savedStage"); // 현재 진행 상황 받아오기
                // 예시: 저장된 스테이지로 씬 로드
                //SceneManager.LoadScene("Stage" + savedStage);
            }
            else
            {
                // No 버튼 클릭 시 로직
                loadAlert.SetActive(false);
            }
        }));
    }

    //설정창 끄기 키기
    public void openOption()
    {
        // OptionCanvas_Audio가 존재할 경우에만 로직 실행
        if (optionCanvasAudio != null)
        {
            audioSource.Play();

            // optionCanvasAudio의 자식 오브젝트로 있는 option을 찾아서 제어
            GameObject option = optionCanvasAudio.transform.Find("Option").gameObject;

            if (option != null)
            {
                // 패널이 활성화되어 있으면 비활성화, 비활성화되어 있으면 활성화
                if (option.activeSelf)
                {
                    option.SetActive(false);
                }
                else
                {
                    option.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("Option 오브젝트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("optionCanvasAudio 오브젝트가 존재하지 않습니다.");
        }
    }

    //게임 종료
    public void ExitGame()
    {
        audioSource.Play();
        Application.Quit(); //빌드해야 작동함.
    }



    //그 외 로직들

    //여기 있었던 SaveStage() StageController.cs로 옮김

    //스테이지 로드   추후 저장된 스테이지 불러오기 가능 시 알림창 yes 버튼에 적용
    public int LoadStage()
    {
        if (PlayerPrefs.HasKey("savedStage"))
        {
            return PlayerPrefs.GetInt("savedStage");
        }
        else
        {
            // 저장된 값이 없을 때 추가작업
            return 1; // 또는 원하는 다른 값
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
