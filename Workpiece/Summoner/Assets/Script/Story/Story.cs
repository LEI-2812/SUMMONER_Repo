using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    /* ���丮 ������ �ʿ��� �ڵ�
      - skip ��ư Ŭ�� �� �޴�â �߰� �ϱ�
      - �˸�â���� ���丮 ��ŵ yes �� �ٷ� ���� ȭ�� ���
      - no Ŭ�� �� �˸�â ����
      - ���⼭�� esc Ŭ�� �� �޴� â ���� */

    public GameObject menu;
    public GameObject toMain;
    public Alert toMainResult;

    public GameObject toQuit;
    public Alert toQuitResult;

    public GameObject alertSkip;    // ��ŵ â ��� ������Ʈ
    public Alert alertSkipResult;

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

    public void skipAlert()
    {
        alertSkip.SetActive(true);

        alertSkipResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertSkip, alertSkipResult, (result) => {
            if (result)
            {
                Debug.Log("���� ������ �̵�");
                SceneManager.LoadScene("Fight Screen");
            }
            else
            {
                toMain.SetActive(false);
            }
        }));
    }
    /*
    // ��ŵ ��ư Ŭ���� ��ŵ �˸�â ����
    public void openSkip()
    {
        alertSkip.SetActive(true);
    }

    // ���丮 ��ŵ yes Ŭ�� �� ���� ������ ������
    public void gotoBattle()
    {
        Debug.Log("���� ������ �̵�");
        // �̰� �ϴ� ���� �ִ� ����Ʈ ������ �̵��� �س��� �ϳ�..?
        SceneManager.LoadScene("Fight Screen");
    }
    
    // ���� ȭ�� ���� �� ���̺� �˸�â
    public void checkSave()
    {
        toMain.SetActive(true);
    }

    // ȭ�� ���� �� ���̺� �˸�â
    public void checkSave2()
    {
        toQuit.SetActive(true);
    }
    */
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
