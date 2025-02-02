using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�˸�â �������̽� ����
public interface IAlertHandler
{
    void ShowAlert(System.Action<bool> callback);
}

//�˸�â ���� Ŭ����
public class AlertHandler : MonoBehaviour, IAlertHandler
{
    [SerializeField] private GameObject alertObject;
    [SerializeField] private Alert alertScript;
    [SerializeField] private AudioSource alertClick;

    public void ShowAlert(System.Action<bool> callback)
    {
        if (alertObject == null || alertScript == null)
        {
            Debug.LogError("Alert Object �Ǵ� Alert Script�� �Ҵ���� �ʾҽ��ϴ�.");
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

