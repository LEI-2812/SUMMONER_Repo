using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleAlert : MonoBehaviour
{
    /*
      ��Ʋ ������ �ʿ��� �ڵ�... ���� �����ص� �̳� �������� �ϴ� �˸�â�� �������
      �޴� â�� �ִ� �͵� �״��, esc Ŭ�� �� ����
      ���� �¸�, �й� �� ��� �˸�â
     */

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
    /*
    // ���� �¸� �� �˸� â Ȱ��ȭ - ���� ���� �ý��� �̰��߷� ���� �Ŀ� �߰�
    public void winBattle()
    {
        Debug.Log("�������� �¸��Ͽ����ϴ�.");
        alertClear.SetActive(true);
    }

    // ���� �й� �� �˸� â Ȱ��ȭ - ���� ���� �ý��� �̰��߷� ���� �Ŀ� �߰�
    public void loseBattle()
    {
        Debug.Log("�������� �й��Ͽ����ϴ�.");
        alertFail.SetActive(true);
    }

    // alertClear�� alertFail���� "ó������ �ٽ��ϱ�" ��ư Ŭ�� ��
    public void resetBattle()
    {
        Debug.Log("������ �ٽ� �����մϴ�.");
        // �ٵ� �̰͵� ��Ʋ�� �ʱ� ���¸� ���س��� ���߿� ���� ��
    }

    // alertClear���� "���� ����������" ��ư Ŭ�� ��
    public void nextStage()
    {
        Debug.Log("���� ���������� �̵��մϴ�.");
        // �̰� ���� ���� ���������� �����س��� ���� ��Ʋ ������ �̵��ϴ� �ڵ� ������
        // SceneManager.LoadScene("Fight Screen stage2"); ����..?
    }

    // alertFail���� "�������� ���� ȭ������" ��ư Ŭ�� ��
    public void SelectStage()
    {
        Debug.Log("�������� ���� ȭ������ �̵��մϴ�.");
        SceneManager.LoadScene("Stage Select Screen");  // �������� ���� ȭ������ �� �̵�
    }
    */
  
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
