using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    [Header("��ŵ�Ͻðڽ��ϱ�? â")]
    public GameObject alertSkip;    // ��ŵ â ��� ������Ʈ
    public Alert alertSkipResult; 
    public GameObject SkipBtn; //Button ���� �ϸ� ��Ȱ��ȭ�� �ȵ�

    private Setting setting;  // SettingMenuController�� �ν��Ͻ��� ����
    private string playingStage;   // �÷��� ���� �������� ��ȣ

    private void Start()
    {
        // Setting �̱��� �ν��Ͻ��� ����
        setting = Setting.instance;

        if (setting == null)
        {
            Debug.LogWarning("SettingMenuController �ν��Ͻ��� �������� �ʽ��ϴ�.");
        }

        playingStage = PlayerPrefs.GetInt("playingStage", 0).ToString();
        isSkipActive(); //��ŵ��ư Ȱ��ȭ ���� �Ǻ�
    }

    private void Update()
    {
        //��ư�� Ȱ��ȭ �Ǿ��ְ� �׻��¿��� ���͸� ������ ��ŵâ ���
        if (Input.GetKeyDown(KeyCode.Return) && SkipBtn.activeSelf==true)
        {
            skipAlert();
        }
    }

    public void isSkipActive() //Setting�� ��ŵ��ۿ� ���� ��ŵ��ư Ȱ��ȭ ���� ���� �޼ҵ�
    {
        if(setting != null) //null���� �˻�
        {
            //setting�� ���� ��Ʈ�ѷ��� �����ͼ� ���丮��ŵ ����� Ȱ��ȭ �������� �˻�
            if ( setting.GetGamePlayController().getIsStorySkip() == true)
            {
                SkipBtn.SetActive(true);
            }
            else { SkipBtn.SetActive(false); } //�ƴ϶�� ��Ȱ��ȭ
        }
    }

    public void skipAlert() //��ŵ Alert ���� �޼ҵ�
    {
        alertSkip.SetActive(true);

        alertSkipResult.ResetAlert();
        StartCoroutine(WaitForAlertResult(alertSkip, alertSkipResult, (result) => {
            if (result)
            {
                Debug.Log("��ŵ�Ͽ� ���� ������ �̵�");
                string sceneName = "Fight Screen_" + playingStage + "Stage";
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                alertSkip.SetActive(false);
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
