using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSreenEvent : MonoBehaviour
{

    public GameObject option;

    //�������� ���� (�� �������� Ŭ����� ȣ���ϸ��.)
    public void SaveStage(int stageNumber)
    {
        // "CurrentStage"��� Ű�� �������� ��ȣ�� �����մϴ�.
        PlayerPrefs.SetInt("SaveStage", stageNumber);
        PlayerPrefs.Save(); // ������ ���� �����մϴ�.
    }

    //�������� �ε�
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
        // ����: ����� ���������� �� �ε�
        //SceneManager.LoadScene("Stage" + savedStage);
    }

    //������
    public void NewStart()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("����Ǿ��ִ� �����͸� ��� ������ ������ ����");
    }

    //����â ���� Ű��
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
