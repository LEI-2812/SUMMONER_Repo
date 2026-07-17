using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 역할: 시작 화면의 새 게임, 이어하기, 옵션 버튼 입력을 처리한다.
public class StartScreenView : MonoBehaviour
{
    [Header("새 게임 확인 알림")]
    public GameObject newAlert;
    public ConfirmAlertView newAlertResult;

    [Header("이어하기 확인 알림")]
    public GameObject loadAlert;
    public ConfirmAlertView loadAlertResult;
    public Button loadButton;
    [SerializeField] private StageTextView loadStageTextView;

    [Header("옵션 버튼")]
    public Button settingBtn;

    [Header("효과음")]
    public AudioSource audioSource;

    private readonly StartNewGameUseCase startNewGameUseCase = new StartNewGameUseCase();
    private readonly ContinueGameUseCase continueGameUseCase = new ContinueGameUseCase();
    private readonly ExitGameUseCase exitGameUseCase = new ExitGameUseCase();

    void Start()
    {
        GameSceneUseCase.LoadHudAdditive();

        if (settingBtn == null)
        {
            Debug.LogError("SettingPanelView 버튼이 연결되지 않았습니다.");
        }
        else
        {
            settingBtn.onClick.RemoveListener(OpenOption);
            settingBtn.onClick.AddListener(OpenOption);
        }

        newAlert.SetActive(false);
        loadAlert.SetActive(false);

        ActiveSavedBtn();
    }

    public void NewStart()
    {
        audioSource.Play();
        newAlert.SetActive(true);

        newAlertResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(newAlert, newAlertResult, (result) => {
            if (result)
            {
                startNewGameUseCase.TryStartNewGame();
            }
            else
            {
                newAlert.SetActive(false);
            }
        }));
    }

    public void StartSavedStage() 
    {
        if (!continueGameUseCase.TryGetSavedStageDisplayData(out StageDisplayData stageDisplayData))
        {
            loadButton.interactable = false;
            return;
        }

        if (loadStageTextView == null)
        {
            Debug.LogError("이어하기 확인창의 StageTextView가 연결되지 않았습니다.");
            return;
        }

        audioSource.Play();
        loadStageTextView.Show(stageDisplayData);
        loadAlert.SetActive(true);
        loadAlertResult.ResetAlert();

        StartCoroutine(WaitForAlertResult(loadAlert, loadAlertResult, (result) => {
            if (result)
            {
                continueGameUseCase.TryContinueSavedGame();
            }
            else
            {
                loadAlert.SetActive(false);
            }
        }));
    }

    private void ActiveSavedBtn()
    {
        if (!continueGameUseCase.HasSavedGame())
        {
            loadButton.gameObject.SetActive(false);
        }
    }

    public void OpenOption()
    {
        audioSource?.Play();

        SettingPanelView settingPanelView = FindObjectOfType<SettingPanelView>();
        if (settingPanelView == null)
        {
            Debug.LogError("SettingPanelView를 찾을 수 없습니다.");
            return;
        }

        settingPanelView.OpenOption();
    }

    //게임 종료
    public void ExitGame()
    {
        audioSource.Play();
        exitGameUseCase.ExitGame();
    }



    private IEnumerator WaitForAlertResult(GameObject alertObject, ConfirmAlertView alertScript, System.Action<bool> callback)
    {
        alertObject.SetActive(true);

        while (!alertScript.GetIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);

        callback(alertScript.GetResult());
    }
}
