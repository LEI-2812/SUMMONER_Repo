using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenEvent : MonoBehaviour
{
    private GameObject menuCanvas; //���ı��� �س��� public���� �ҽ� �ٸ��� �ٳ���� missing���⶧���� ���� ����

    [Header("���ξ� ó������ ��ư")]
    public GameObject newAlert;
    public Alert newAlertResult;

    [Header("���ξ� �̾ ��ư")]
    public GameObject loadAlert;
    public Alert loadAlertResult;

    [Header("���ξ� ������ư")]
    public Button settingBtn;

    [Header("ȿ���� ����")]
    public AudioSource audioSource;

    private Setting setting;

    void Start()
    {
        // OptionCanvas_Audio ������Ʈ�� ������ ã�Ƽ� ����
        menuCanvas = GameObject.Find("MenuCanvas");

        // ���� ���� ������Ʈ�� ���� ��� ���� ����
        if (menuCanvas == null)
        {
            Debug.LogError("MenuCanvas ������Ʈ�� ����.");
        }

        // Setting ��ư Ŭ�� �̺�Ʈ�� ����
        if (settingBtn != null)
        {
            settingBtn.onClick.AddListener(openOption); // ��ư�� openOption �̺�Ʈ ����
        }
        else{ Debug.LogError("Setting ��ư�� ������� �ʾҽ��ϴ�."); }
        newAlert.SetActive(false);
        loadAlert.SetActive(false);
        int savedStageValue = PlayerPrefs.GetInt("savedStage");
        Debug.Log("savedStage �� : " + savedStageValue);
    }

    //������
    public void NewStart()
    {
        audioSource.Play();
        newAlert.SetActive(true); // �˸�â Ȱ��ȭ

        // �˸�â ���¸� �ʱ�ȭ
        newAlertResult.ResetAlert();
        // �ڷ�ƾ ����: newAlert�� ���� ó��
        StartCoroutine(WaitForAlertResult(newAlert, newAlertResult, (result) => {
            if (result)
            {
                
                // Yes ��ư Ŭ�� �� ����
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("savedStage", 1); // �������� ���� ��Ȳ �ʱ�ȭ
                PlayerPrefs.Save();
                CheckStage.stageNum = 0;
                Debug.Log("����Ǿ��ִ� �����͸� ��� ������ ������ ����");
                SceneManager.LoadScene("Pro_Epi Screen");
            }
            else
            {
                // No ��ư Ŭ�� �� ����
                newAlert.SetActive(false);
            }
        }));
    }

    //�̾��ϱ�
    public void StartSavedStage() 
    {
        audioSource.Play();
        loadAlert.SetActive(true); // �˸�â Ȱ��ȭ

        int savedStage = LoadStage();
        loadAlertResult.ResetAlert();

        StartCoroutine(WaitForAlertResult(loadAlert, loadAlertResult, (result) => {
            if (result)
            {
                // Yes ��ư Ŭ�� �� ����
                // Debug.Log("����� �������� " + savedStage + "�� �̵�");
                Debug.Log("����� ���������� �̵�");
                PlayerPrefs.GetInt("savedStage"); // ���� ���� ��Ȳ �޾ƿ���
                // ����: ����� ���������� �� �ε�
                //SceneManager.LoadScene("Stage" + savedStage);
            }
            else
            {
                // No ��ư Ŭ�� �� ����
                loadAlert.SetActive(false);
            }
        }));
    }

    //����â ���� Ű��
    public void openOption()
    {
        // OptionCanvas_Audio�� ������ ��쿡�� ���� ����
        if (menuCanvas != null)
        {
            audioSource.Play();

            GameObject option = menuCanvas.transform.Find("Setting/SettingPanel").gameObject;

            if (option != null)
            {
                // �г��� Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ, ��Ȱ��ȭ�Ǿ� ������ Ȱ��ȭ
                if (option.activeSelf)
                {
                    option.SetActive(false);
                }
                else
                {
                    option.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("SettingPanel ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("MenuCanvas ������Ʈ�� �������� �ʽ��ϴ�.");
        }
    }

    //���� ����
    public void ExitGame()
    {
        audioSource.Play();
        Application.Quit(); //�����ؾ� �۵���.
    }



    //�� �� ������

    //���� �־��� SaveStage() StageController.cs�� �ű�

    //�������� �ε�   ���� ����� �������� �ҷ����� ���� �� �˸�â yes ��ư�� ����
    public int LoadStage()
    {
        if (PlayerPrefs.HasKey("savedStage"))
        {
            return PlayerPrefs.GetInt("savedStage");
        }
        else
        {
            // ����� ���� ���� �� �߰��۾�
            return 1; // �Ǵ� ���ϴ� �ٸ� ��
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