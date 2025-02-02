using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//알림창 인터페이스 정의
public interface IAlertHandler
{
    void ShowAlert(System.Action<bool> callback);
}

//알림창 관리 클래스
public class AlertHandler : MonoBehaviour, IAlertHandler
{
    [SerializeField] private GameObject alertObject;
    [SerializeField] private Alert alertScript;
    [SerializeField] private AudioSource alertClick;

    public void ShowAlert(System.Action<bool> callback)
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

    private IEnumerator WaitForAlertResult(System.Action<bool> callback)
    {
        while (!alertScript.getIsClicked())
        {
            yield return null;
        }

        alertObject.SetActive(false);
        if (alertClick != null)
        {
            alertClick.Play();
        }
        callback(alertScript.getResult());
    }
}

