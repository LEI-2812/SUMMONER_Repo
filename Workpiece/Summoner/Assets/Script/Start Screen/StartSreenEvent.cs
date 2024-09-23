using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSreenEvent : MonoBehaviour
{
    private GameObject optionCanvasAudio;
    public GameObject newAlert;
    public Alert newAlertResult;


    public GameObject loadAlert;
    public Button settingBtn;

    public AudioSource audioSource;


    void Start()
    {
        // OptionCanvas_Audio ������Ʈ�� ������ ã�Ƽ� ����
        optionCanvasAudio = GameObject.Find("OptionCanvas_Audio");

        // ���� ���� ������Ʈ�� ���� ��� ���� ����
        if (optionCanvasAudio == null)
        {
            Debug.LogError("OptionCanvas_Audio ������Ʈ�� ����.");
        }

        // Setting ��ư Ŭ�� �̺�Ʈ�� ����
        if (settingBtn != null)
        {
            settingBtn.onClick.AddListener(openOption); // ��ư�� openOption �̺�Ʈ ����
        }
        else
        {
            Debug.LogError("Setting ��ư�� ������� �ʾҽ��ϴ�.");
        }

        newAlert.SetActive(false);
        loadAlert.SetActive(false);
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
                PlayerPrefs.Save();
                Debug.Log("����Ǿ��ִ� �����͸� ��� ������ ������ ����");
                SceneManager.LoadScene("Stage Select Screen");
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
        int savedStage = LoadStage();
        Debug.Log("����� �������� "+ savedStage+"�� �̵�");
        loadAlert.SetActive(true); // �˸�â Ȱ��ȭ
        // ����: ����� ���������� �� �ε�
        //SceneManager.LoadScene("Stage" + savedStage);
    }

    //�������� ���� ȭ������ �̵�
    public void SelectStage()
    {
        SceneManager.LoadScene("Stage Select Screen");
    }

    //����â ���� Ű�� -> ���� : SettingMenuContoller.cs���� ���� ����. Dontdestroyonload�� �Ѿ���� �� cs������ �Լ��� ��� �Ұ�.
    public void openOption()
    {
        // OptionCanvas_Audio�� ������ ��쿡�� ���� ����
        if (optionCanvasAudio != null)
        {
            audioSource.Play();

            // optionCanvasAudio�� �ڽ� ������Ʈ�� �ִ� option�� ã�Ƽ� ����
            GameObject option = optionCanvasAudio.transform.Find("Option").gameObject;

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
                Debug.LogError("Option ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("optionCanvasAudio ������Ʈ�� �������� �ʽ��ϴ�.");
        }
    }

    //���� ����
    public void ExitGame()
    {
        audioSource.Play();
        Application.Quit(); //�����ؾ� �۵���.
    }



    /// //////////////////////////////�� �� ������

    //�������� ���� (�� �������� Ŭ����� ȣ���ϸ��.)
    public void SaveStage(int stageNumber)
    {
        // "CurrentStage"��� Ű�� �������� ��ȣ�� �����մϴ�.
        PlayerPrefs.SetInt("SaveStage", stageNumber);
        PlayerPrefs.Save(); // ������ ���� �����մϴ�.
    }

    //�������� �ε�   ���� ����� �������� �ҷ����� ���� �� �˸�â yes ��ư�� ����
    public int LoadStage()
    {
        if (PlayerPrefs.HasKey("SaveStage"))
        {
            return PlayerPrefs.GetInt("SaveStage");
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
