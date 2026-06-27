using System.Collections;
using UnityEngine;


// 역할: BaseAlertHandler의 책임을 정의한다.
// 역할: BaseAlertHandler의 책임을 정의한다.
public abstract class BaseAlertHandler : MonoBehaviour
{
    [SerializeField] protected GameObject alertObject;
    [SerializeField] protected ConfirmAlertView alertScript;
    [SerializeField] protected AudioSource alertClick;

    internal void InitializeAlert(GameObject alertObject, ConfirmAlertView alertScript, AudioSource alertClick)
    {
        this.alertObject = alertObject;
        this.alertScript = alertScript;
        this.alertClick = alertClick;
    }

    public virtual void ShowAlert(System.Action<bool> callback)
    {
        if (alertObject == null || alertScript == null)
        {
            Debug.LogError("Alert Object 또는 Alert Script가 할당되지 않았습니다.");
            return;
        }

        alertObject.SetActive(true);
        alertScript.ResetAlert();
        StartCoroutine(WaitForAlertResult(callback));
    }

    protected IEnumerator WaitForAlertResult(System.Action<bool> callback)
    {
        while (!alertScript.GetIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);
        alertClick?.Play();
        callback(alertScript.GetResult());
    }
}

