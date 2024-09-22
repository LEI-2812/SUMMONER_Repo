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
    public GameObject toQuit;
    public GameObject alertSkip;    // ��ŵ â ��� ������Ʈ

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
