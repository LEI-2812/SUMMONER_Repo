using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static Menu instance;

    [Header("�޴� �ǳ�")]
    [SerializeField] private GameObject menuPanel;

    [Space]

    [Header("����ȭ���̵�")]
    [SerializeField] private GameObject toMain; //�������� Alert
    [SerializeField] private Alert toMainResult; //���� Alert ����

    [Space]

    [Header("����â")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Setting setting;

    [Space]

    [Header("��������")]
    [SerializeField] private GameObject toQuit; //������ �˸�â
    [SerializeField] private Alert toQuitResult; //������ Alert ����

    [Header("��׶��� ���")]
    [SerializeField ]private GameObject backGroundPanel;

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ�, ���� ������ ������Ʈ �ı�
        }
    }

    // esc Ű�� ��� �����Ͽ� ���� �� �޴�â ����/�ݱ�
    void Update()
    {
        //ESC�Է��� StartScreen���� �ȸ����� ����
        if (Input.GetKeyDown(KeyCode.Escape) && ! (SceneManager.GetActiveScene().name == "Start Screen"))
        {
            if (!setting.settingPanel.activeSelf)
                openCloseMenu();
        }
    }


    public void openCloseMenu()
    {
        if (menuPanel.activeSelf == true)
        {
            backGroundPanel.SetActive(false);
            menuPanel.SetActive(false);
        }
        else
        {
            menuPanel.SetActive(true);
            backGroundPanel.SetActive(true);
        }
    }
    public void toMainAlert()
    {
        // audioSource.Play(); �ϴ� �������ϴ�
        toMain.SetActive(true); // �˸�â Ȱ��ȭ

        // �˸�â ���¸� �ʱ�ȭ
        toMainResult.ResetAlert();
        // �ڷ�ƾ ����: newAlert�� ���� ó��
        StartCoroutine(WaitForAlertResult(toMain, toMainResult, (result) => {
            if (result)
            {
                SceneManager.LoadScene("Start Screen");
                openCloseMenu();
            }
            else
            {
                // No ��ư Ŭ�� �� ����
                toMain.SetActive(false);
                openCloseMenu();
            }
        }));
    }

    public void toQuitAlert()
    {
        toQuit.SetActive(true); // �˸�â Ȱ��ȭ

        // �˸�â ���¸� �ʱ�ȭ
        toQuitResult.ResetAlert();
        // �ڷ�ƾ ����: newAlert�� ���� ó��
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

    /*
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

    //���� ����
    public void ExitGame()
    {
        Debug.Log("������ �����մϴ�.");
        Application.Quit();
    }
    */
    // ����â �������� - �ٸ� ����� ����ȭ�鿡�� �ѹ� ����â�� �Ѿ� ������ �� ����
    public void openSettingCanvas()
    {
       setting.openOption();
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