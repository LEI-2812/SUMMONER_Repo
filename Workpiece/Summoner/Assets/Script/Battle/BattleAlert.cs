using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleAlert : MonoBehaviour
{
    [Header("���� ��� �˸�â")]
    public GameObject alertClear; // ���� �¸� �˸�â
    public Alert ClearResult; // ���� �¸� �˸� ���
    public GameObject alertFail; // ���� �й� �˸�â
    public Alert FailResult; // ���� �й� �˸� ���

    // �¸� �� ȣ���� �Լ� (���� �ȵǴ� ������ �긦 ȣ���ϴ� ���� ��� �׷���.. ���� failAlert�� ��������)
    public void clearAlert()
    {
        alertClear.SetActive(true);

        ClearResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertClear, ClearResult, (result) => {
            if (result) // ó������ �ٽ��ϱ�
            {
                Debug.Log("������ �ٽ� �����մϴ�.");
            }
            else // ���� ����������
            {
                Debug.Log("���� ���������� �̵��մϴ�.");
            }
        }));
    }

    // �й� �� ȣ���� �Լ�
    public void failAlert()
    {
        alertFail.SetActive(true);

        FailResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertFail, FailResult, (result) => {
            if (result) // ó������ �ٽ��ϱ�
            {
                Debug.Log("������ �ٽ� �����մϴ�.");
            }
            else // �������� ����ȭ������
            {
                Debug.Log("�������� ���� ȭ������ �̵��մϴ�.");
                SceneManager.LoadScene("Stage Select Screen");
            }
        }));
    }
  
    private IEnumerator WaitForAlertResult(GameObject alertObject, Alert alertScript, System.Action<bool> callback)
    {
        // �˸�â�� Ȱ��ȭ
        alertObject.SetActive(true);

        // ����ڰ� ��ư�� Ŭ���� ������ ���
        while (!alertScript.getIsClicked())
        {
            yield return null;  // �� ������ ���
        }

        // �˸�â ��Ȱ��ȭ
        alertObject.SetActive(false);

        // ��ư Ŭ�� �� ��� �ݹ� ȣ�� (true: Yes, false: No)
        callback(alertScript.getResult());
    }
}
