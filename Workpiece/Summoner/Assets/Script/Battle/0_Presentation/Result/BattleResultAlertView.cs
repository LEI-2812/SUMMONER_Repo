using System.Collections;
using UnityEngine;

// 역할: BattleResultAlertView의 책임을 정의한다.
public class BattleResultAlertView : MonoBehaviour
{
    [Header("전투 결과 alert")]
    public GameObject alertClear;
    public ConfirmAlertView ClearResult;
    public GameObject alertFail;
    public ConfirmAlertView FailResult;

    [Header("사운드")]
    [SerializeField] private AudioSource clearSound;
    [SerializeField] private AudioSource failSound;

    private void Awake()
    {
        HideAlerts();
    }

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

        Debug.LogWarning("전투 결과 " + alertName + " 알림이 연결되지 않았습니다. 기본 전투 흐름을 계속 진행합니다.");
        return false;
    }

    private void HideAlerts()
    {
        if (alertClear != null)
        {
            alertClear.SetActive(false);
        }

        if (alertFail != null)
        {
            alertFail.SetActive(false);
        }
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
