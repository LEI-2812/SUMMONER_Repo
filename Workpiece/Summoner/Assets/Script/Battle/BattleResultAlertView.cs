using System.Collections;
using UnityEngine;

public class BattleResultAlertView : MonoBehaviour
{
    [Header("전투 결과 알림창")]
    public GameObject alertClear;
    public ConfirmAlertView ClearResult;
    public GameObject alertFail;
    public ConfirmAlertView FailResult;

    [Header("효과음")]
    [SerializeField] private AudioSource clearSound;
    [SerializeField] private AudioSource failSound;

    private StageFlowController stageFlowController;

    private void Start()
    {
        // 전투 결과 이후의 씬 이동은 StageFlowController가 전담한다.
        stageFlowController = StageFlowController.instance;
        if (stageFlowController == null)
        {
            stageFlowController = FindObjectOfType<StageFlowController>();
        }

        if (stageFlowController == null)
        {
            Debug.LogError("StageFlowController가 전투 씬에 없습니다.");
        }
    }

    public void ShowClearResultAlert(int stageNum)
    {
        alertClear.SetActive(true);
        clearSound.Play();
        ClearResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertClear, ClearResult, (result) =>
        {
            if (result)
            {
                // 처음부터 다시하기를 선택하면 현재 전투 씬을 다시 연다.
                if (stageFlowController == null)
                {
                    Debug.LogError("StageFlowController가 준비되지 않아 전투를 다시 시작할 수 없습니다.");
                    return;
                }

                Debug.Log("전투를 다시 시작합니다.");
                stageFlowController.SendFight(stageNum);
                return;
            }

            // 다음 진행을 선택하면 저장 후 다음 흐름으로 이동한다.
            if (stageFlowController == null)
            {
                Debug.LogError("StageFlowController가 준비되지 않아 다음 스테이지로 이동할 수 없습니다.");
                return;
            }

            if (GameSaveController.instance == null)
            {
                Debug.LogError("GameSaveController가 전투 씬에 없습니다.");
                return;
            }

            // 이어하기 흐름은 1~7 스테이지만 사용하므로 마지막 스테이지에서는 8을 저장하지 않는다.
            if (stageNum < 7)
            {
                GameSaveController.instance.SaveClearedStage(stageNum + 1);
            }

            stageFlowController.SendNextStageAfterBattle(stageNum);
        }));
    }

    public void ShowFailResultAlert(int stageNum)
    {
        alertFail.SetActive(true);
        failSound.Play();
        FailResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertFail, FailResult, (result) =>
        {
            if (result)
            {
                // 패배 후 다시하기를 선택하면 현재 전투 씬을 다시 연다.
                if (stageFlowController == null)
                {
                    Debug.LogError("StageFlowController가 준비되지 않아 전투를 다시 시작할 수 없습니다.");
                    return;
                }

                Debug.Log("전투를 다시 시작합니다.");
                stageFlowController.SendFight(stageNum);
                return;
            }

            // 포기하면 스테이지 선택 화면으로 돌아간다.
            if (stageFlowController == null)
            {
                Debug.LogError("StageFlowController가 준비되지 않아 스테이지 선택 화면으로 이동할 수 없습니다.");
                return;
            }

            Debug.Log("스테이지 선택 화면으로 이동합니다.");
            stageFlowController.SendStageSelect();
        }));
    }

    private IEnumerator WaitForAlertResult(GameObject alertObject, ConfirmAlertView alertScript, System.Action<bool> callback)
    {
        // 알림창을 활성화하고 사용자의 선택을 기다린다.
        alertObject.SetActive(true);

        while (!alertScript.getIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);
        callback(alertScript.getResult());
    }
}
