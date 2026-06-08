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

    public void ShowClearResultAlert(System.Action<bool> callback)
    {
        if (clearSound != null)
        {
            clearSound.Play();
        }

        if (!CanShowAlert(alertClear, ClearResult, "clear"))
        {
            callback?.Invoke(false);
            return;
        }

        alertClear.SetActive(true);
        ClearResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertClear, ClearResult, result =>
        {
            callback?.Invoke(result);
        }));
    }

    public void ShowFailResultAlert(System.Action<bool> callback)
    {
        if (failSound != null)
        {
            failSound.Play();
        }

        if (!CanShowAlert(alertFail, FailResult, "fail"))
        {
            callback?.Invoke(false);
            return;
        }

        alertFail.SetActive(true);
        FailResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertFail, FailResult, result =>
        {
            callback?.Invoke(result);
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
