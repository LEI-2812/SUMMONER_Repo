using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Battle : MonoBehaviour
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

    // === Private References ===
    private Setting setting; // SettingMenuController �ν��Ͻ� ����


    private void Start()
    {
        getSettingMenuController(); //SettingMenuController�ν��Ͻ� ��������
    }


    //SettingMenuController �ν��Ͻ� �޾ƿ��� �޼ҵ�
    private void getSettingMenuController()
    {
        setting = Setting.instance;
        if (setting == null)
        {
            Debug.LogWarning("SettingMenuController �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

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
  

    // ����â �������� - �ٸ� ����� ����ȭ�鿡�� �ѹ� ����â�� �Ѿ� ������ �� ����
    public void OpenOptionCanvas()
    {
        if (Setting.instance != null)
        {
            Setting.instance.openOption();
        }
        else
        {
            Debug.LogWarning("SettingMenuController �ν��Ͻ��� �������� �ʽ��ϴ�.");
        }
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
