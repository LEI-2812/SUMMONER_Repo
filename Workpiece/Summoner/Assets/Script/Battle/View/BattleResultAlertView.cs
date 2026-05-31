using System.Collections;
using UnityEngine;

public class BattleResultAlertView : MonoBehaviour
{
    [Header("Battle result alert")]
    public GameObject alertClear;
    public ConfirmAlertView ClearResult;
    public GameObject alertFail;
    public ConfirmAlertView FailResult;

    [Header("Sound")]
    [SerializeField] private AudioSource clearSound;
    [SerializeField] private AudioSource failSound;

    private StageFlowController stageFlowController;

    private void Start()
    {
        stageFlowController = StageFlowController.instance;
        if (stageFlowController == null)
        {
            stageFlowController = FindObjectOfType<StageFlowController>();
        }

        if (stageFlowController == null)
        {
            Debug.LogError("StageFlowController is missing in fight scene.");
        }

    }

    public void ShowClearResultAlert(int stageNum)
    {
        if (clearSound != null)
        {
            clearSound.Play();
        }

        if (!CanShowAlert(alertClear, ClearResult, "clear"))
        {
            CompleteClearResult(stageNum, false);
            return;
        }

        alertClear.SetActive(true);
        ClearResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertClear, ClearResult, result =>
        {
            CompleteClearResult(stageNum, result);
        }));
    }

    public void ShowFailResultAlert(int stageNum)
    {
        if (failSound != null)
        {
            failSound.Play();
        }

        if (!CanShowAlert(alertFail, FailResult, "fail"))
        {
            CompleteFailResult(stageNum, false);
            return;
        }

        alertFail.SetActive(true);
        FailResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertFail, FailResult, result =>
        {
            CompleteFailResult(stageNum, result);
        }));
    }

    private bool CanShowAlert(GameObject alertObject, ConfirmAlertView alertScript, string alertName)
    {
        if (alertObject != null && alertScript != null)
        {
            return true;
        }

        Debug.LogWarning("Battle result " + alertName + " alert is not assigned. Default battle flow will continue.");
        return false;
    }

    private void CompleteClearResult(int stageNum, bool shouldRetry)
    {
        if (stageFlowController == null)
        {
            Debug.LogError("Cannot handle clear result because StageFlowController is missing.");
            return;
        }

        if (shouldRetry)
        {
            stageFlowController.SendFight(stageNum);
            return;
        }

        if (GameSaveController.instance == null)
        {
            Debug.LogError("Cannot save clear result because GameSaveController is missing.");
            return;
        }

        if (stageNum < 7)
        {
            GameSaveController.instance.SaveClearedStage(stageNum + 1);
        }

        stageFlowController.SendNextStageAfterBattle(stageNum);
    }

    private void CompleteFailResult(int stageNum, bool shouldRetry)
    {
        if (stageFlowController == null)
        {
            Debug.LogError("Cannot handle fail result because StageFlowController is missing.");
            return;
        }

        if (shouldRetry)
        {
            stageFlowController.SendFight(stageNum);
            return;
        }

        stageFlowController.SendStageSelect();
    }

    private IEnumerator WaitForAlertResult(GameObject alertObject, ConfirmAlertView alertScript, System.Action<bool> callback)
    {
        alertObject.SetActive(true);

        while (!alertScript.getIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);
        callback(alertScript.getResult());
    }
}
