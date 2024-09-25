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

    public GameObject menu;

    public GameObject toMain;
    public Alert toMainResult;

    public GameObject toQuit; //������
    public Alert toQuitResult;

    public GameObject alertClear;   // ���� �¸� �� ��� ������Ʈ
    public Alert ClearResult;

    public GameObject alertFail;    // ���� �й� �� ��� ������Ʈ
    public Alert FailResult;

    private SettingMenuController settingController;  // SettingMenuController�� �ν��Ͻ��� ����
    private void Start()
    {
        getSettingMenuController(); //SettingMenuController�ν��Ͻ� ��������
    }
    // esc Ű�� ��� �����Ͽ� ���� �� �޴�â ����/�ݱ�
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settingController.option.activeSelf)
                openMenu();
        }
    }

    //SettingMenuController �ν��Ͻ� �޾ƿ��� �޼ҵ�
    private void getSettingMenuController()
    {
        settingController = SettingMenuController.instance;
        if (settingController == null)
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
    public void openMenu()
    {
        if (menu.activeSelf == true)
            menu.SetActive(false);
        else
            menu.SetActive(true);
    }

    public void toMainAlert()
    {
        toMain.SetActive(true);

        toMainResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(toMain, toMainResult, (result) => {
            if (result)
            {
                SceneManager.LoadScene("Start Screen");
            }
            else
            {
                // No ��ư Ŭ�� �� ����
                toMain.SetActive(false);
            }
        }));
    }

    public void toQuitAlert()
    {
        toQuit.SetActive(true);

        toQuitResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(toQuit, toQuitResult, (result) => {
            if (result)
            {
                Debug.Log("������ �����մϴ�.");
                Application.Quit();
            }
            else
            {
                // No ��ư Ŭ�� �� ����
                toQuit.SetActive(false);
            }
        }));
    }

    // ����â �������� - �ٸ� ����� ����ȭ�鿡�� �ѹ� ����â�� �Ѿ� ������ �� ����
    public void OpenOptionCanvas()
    {
        if (SettingMenuController.instance != null)
        {
            SettingMenuController.instance.openOption();
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
