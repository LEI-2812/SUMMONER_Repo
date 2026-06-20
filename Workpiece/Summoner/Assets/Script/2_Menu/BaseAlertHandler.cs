using System.Collections;
using UnityEngine;


//알림처리 클래스
// 역할: 여러 알림 처리 클래스가 공통으로 쓰는 알림창 표시와 콜백 연결 흐름을 제공한다.
public abstract class BaseAlertHandler : MonoBehaviour
{
    [SerializeField] protected GameObject alertObject;
    [SerializeField] protected ConfirmAlertView alertScript;
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
        while (!alertScript.GetIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);
        alertClick?.Play();
        callback(alertScript.GetResult());
    }
}

