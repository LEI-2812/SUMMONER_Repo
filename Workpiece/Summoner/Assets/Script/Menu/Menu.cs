using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{ 
    public GameObject menu;
    public GameObject toMain;
    public GameObject toQuit;

    // esc Ű�� ��� �����Ͽ� ���� �� �޴�â ����/�ݱ�
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            openMenu();
        }
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
