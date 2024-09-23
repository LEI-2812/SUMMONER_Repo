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
    public GameObject toQuit;
    public GameObject alertClear;   // ���� �¸� �� ��� ������Ʈ
    public GameObject alertFail;    // ���� �й� �� ��� ������Ʈ

    private GameObject setting;
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
        if (setting == null)
        {
            Debug.LogWarning("SettingMenuController �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    // ���� �¸� �� �˸� â Ȱ��ȭ - ���� ���� �ý��� �̰��߷� ���� �Ŀ� �߰�
    public void winBattle()
    {
        Debug.LogWarning("�������� �¸��Ͽ����ϴ�.");
        alertClear.SetActive(true);
    }

    // ���� �й� �� �˸� â Ȱ��ȭ - ���� ���� �ý��� �̰��߷� ���� �Ŀ� �߰�
    public void loseBattle()
    {
        Debug.LogWarning("�������� �й��Ͽ����ϴ�.");
        alertFail.SetActive(true);
    }

    // alertClear�� alertFail���� "ó������ �ٽ��ϱ�" ��ư Ŭ�� ��
    public void resetBattle()
    {
        Debug.LogWarning("������ �ٽ� �����մϴ�.");
        // �ٵ� �̰͵� ��Ʋ�� �ʱ� ���¸� ���س��� ���߿� ���� ��
    }

    // alertClear���� "���� ����������" ��ư Ŭ�� ��
    public void nextStage()
    {
        Debug.LogWarning("���� ���������� �̵��մϴ�.");
        // �̰� ���� ���� ���������� �����س��� ���� ��Ʋ ������ �̵��ϴ� �ڵ� ������
        // SceneManager.LoadScene("Fight Screen stage2"); ����..?
    }

    // alertFail���� "�������� ���� ȭ������" ��ư Ŭ�� ��
    public void SelectStage()
    {
        Debug.LogWarning("�������� ���� ȭ������ �̵��մϴ�.");
        SceneManager.LoadScene("Stage Select Screen");  // �������� ���� ȭ������ �� �̵�
    }

    public void openMenu()
    {
        if (menu.activeSelf == true)
            menu.SetActive(false);
        else
            menu.SetActive(true);
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

    // ���� ȭ������ �̵�
    public void gotoMain()
    {
        SceneManager.LoadScene("Start Screen");
        menu.SetActive(false); // �������� ���� ȭ������ �̵��� ���ÿ� �˸�â ��Ȱ��ȭ
        toMain.SetActive(false);
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

    //���� ����
    public void ExitGame()
    {
        Debug.Log("������ �����մϴ�.");
        Application.Quit();
    }
}
