using System.Collections;
using UnityEngine;


//알림처리 클래스
public abstract class BaseAlertHandler : MonoBehaviour, IAlertHandler
{
    [SerializeField] protected GameObject alertObject;
    [SerializeField] protected Alert alertScript;
    [SerializeField] protected AudioSource alertClick;

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
        while (!alertScript.getIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);
        alertClick?.Play();
        callback(alertScript.getResult());
    }
}
