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
    public Alert loadAlertResult;

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
