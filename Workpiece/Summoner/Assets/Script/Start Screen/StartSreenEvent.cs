using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSreenEvent : MonoBehaviour
{

    public GameObject option;
    public GameObject newAlert;
    public GameObject loadAlert;

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

    //�̾��ϱ�
    public void StartSavedStage() 
    {
        int savedStage = LoadStage();
        Debug.Log("����� �������� "+ savedStage+"�� �̵�");
        loadAlert.SetActive(true); // �˸�â Ȱ��ȭ
        // ����: ����� ���������� �� �ε�
        //SceneManager.LoadScene("Stage" + savedStage);
    }

    //������
    public void NewStart()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("����Ǿ��ִ� �����͸� ��� ������ ������ ����");        
        newAlert.SetActive(true); // �˸�â Ȱ��ȭ
    }

    //�������� ���� ȭ������ �̵�
    public void SelectStage()
    {
        SceneManager.LoadScene("Stage Select Screen");
    }

    //����â ���� Ű�� -> ���� : SettingMenuContoller.cs���� ���� ����. Dontdestroyonload�� �Ѿ���� �� cs������ �Լ��� ��� �Ұ�.
    public void openOption()
    {
        if(option.activeSelf==true)
            option.SetActive(false);
        else
            option.SetActive(true);
    }

    //���� ����
    public void ExitGame()
    {
        Application.Quit(); //�����ؾ� �۵���.
    }

}
