using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 역할: 시작 화면 UI 표시와 시작 버튼 입력을 담당한다.
public class StartScreenView : MonoBehaviour
{
    private GameObject menuCanvas; //비파괴로 해놔서 public으로 할시 다른씬 다녀오면 missing나기때문에 직접 참조
    private StageController stageController;

    [Header("메인씬 처음부터 버튼")]
    public GameObject newAlert;
    public ConfirmAlertView newAlertResult;

    [Header("메인씬 이어서 버튼")]
    public GameObject loadAlert;
    public ConfirmAlertView loadAlertResult;
    public Button loadButton;

    [Header("메인씬 설정버튼")]
    public Button settingBtn;

    [Header("효과음 사운드")]
    public AudioSource audioSource;

    void Start()
    {
        stageController = FindObjectOfType<StageController>();
        // OptionCanvas_Audio 오브젝트를 씬에서 찾아서 참조
        menuCanvas = GameObject.Find("MenuCanvas");

        // 만약 씬에 오브젝트가 없을 경우 오류 방지
        if (menuCanvas == null)
        {
            Debug.LogError("MenuCanvas 오브젝트가 없음.");
        }

        if (settingBtn == null)
        {
            Debug.LogError("SettingPanelView 버튼이 연결되지 않았습니다.");
        }
        newAlert.SetActive(false);
        loadAlert.SetActive(false);

        //이어하기 활성화 여부
        ActiveSavedBtn();
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
                StartGameFlow.TryStartNewGame(stageController);
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
        if (!StartGameFlow.HasSavedGame())
        {
            loadButton.interactable = false;
            return;
        }

        audioSource.Play();
        loadAlert.SetActive(true); // 알림창 활성화

        loadAlertResult.ResetAlert();

        StartCoroutine(WaitForAlertResult(loadAlert, loadAlertResult, (result) => {
            if (result)
            {
                StartGameFlow.TryContinueSavedGame(stageController);
            }
            else
            {
                // No 버튼 클릭 시 로직
                loadAlert.SetActive(false);
            }
        }));
    }

    private void ActiveSavedBtn()
    {
        // 이어하기 버튼은 저장 데이터가 있을 때만 보여준다.
        if (!StartGameFlow.HasSavedGame())
        {
            loadButton.gameObject.SetActive(false);
        }
    }

    //설정창 끄기 키기
    public void OpenOption()
    {
        // OptionCanvas_Audio가 존재할 경우에만 로직 실행
        if (menuCanvas != null)
        {
            audioSource.Play();

            Transform optionTransform = menuCanvas.transform.Find("UI_20_Settings/Setting/SettingPanel")
                ?? menuCanvas.transform.Find("Setting/SettingPanel");
            GameObject option = optionTransform != null ? optionTransform.gameObject : null;

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
                Debug.LogError("SettingPanel 오브젝트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("MenuCanvas 오브젝트가 존재하지 않습니다.");
        }
    }

    //게임 종료
    public void ExitGame()
    {
        audioSource.Play();
        Application.Quit(); //빌드해야 작동함.
    }



    //그 외 로직들

    private IEnumerator WaitForAlertResult(GameObject alertObject, ConfirmAlertView alertScript, System.Action<bool> callback)
    {
        // 알림창을 활성화
        alertObject.SetActive(true);

        // 사용자가 버튼을 클릭할 때까지 대기
        while (!alertScript.GetIsClicked())
        {
            yield return null;  // 한 프레임 대기
        }

        // 알림창 비활성화
        alertObject.SetActive(false);

        // 버튼 클릭 후 결과 콜백 호출 (true: Yes, false: No)
        callback(alertScript.GetResult());
    }
}
