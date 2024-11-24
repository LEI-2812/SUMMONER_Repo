using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    public int stageNum;  //���� �÷����� �������� ��ȣ �ޱ�
    public StoryStage storystage;

    [Header("��ư Ŭ����")]
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        stageNum = PlayerPrefs.GetInt("savedStage");
    }
    void Start()
    {
        int stageNumber = PlayerPrefs.GetInt("savedStage");
        Debug.Log("savedStage �� : " + stageNum);
    }

    // PlayerPrefs�� �������� ���� ���� ����
    void SaveStage()
    {
        PlayerPrefs.SetInt("savedStage", stageNum);
        PlayerPrefs.Save();
    }

    //�������� ���� (�� �������� Ŭ����� ȣ���ϸ��.)
    public void SaveStage(int stageNumber)
    {
        // "savedStage"��� Ű�� �������� ��ȣ�� �����մϴ�.
        PlayerPrefs.SetInt("savedStage", stageNumber);
        PlayerPrefs.Save(); // ������ ���� �����մϴ�.
    }

    public void stageLoader(int stage)
    {
        Debug.Log("��ư Ŭ��");
        audioSource.Play();
        // ���� �÷��� ���� ���������� "playingStage"�� ����
        PlayerPrefs.SetInt("savedStage", stage);
        PlayerPrefs.Save();
        SendStage(stage);
    }

    public void SendStage(int stage)    // �������� ���� ȭ�鿡�� ���� ��(���丮 + ����)
    {
        switch (stage)
        {
            case 1:
                SendStory(stage);
                Summon.multiple = 1;
                SaveStage(1);
                break;
            case 2:
                SendStory(stage);
                Summon.multiple = 1;
                SaveStage(2);
                break;
            case 3:
                SendStory(stage);
                Summon.multiple = 1.2;
                SaveStage(3);
                break;
            case 4:
                SendFight(stage);
                Summon.multiple = 1.2;
                SaveStage(4);
                break;
            case 5:
                SendStory(stage);
                Summon.multiple = 1.5;
                SaveStage(5);
                break;
            case 6:
                SendFight(stage);
                Summon.multiple = 1.5;
                SaveStage(6);
                break;
            case 7:
                SendStory(stage);
                Summon.multiple = 2.5;
                SaveStage(7);
                break;
            // �ʿ��� ����������ŭ �߰�
            default:
                Debug.Log("�߸��� ���������Դϴ�.");
                break;
        }
    }
    public void SendFightStage(int stage)   // �¸�/�й�â���� ���� ��(���� ����)
    {
        switch (stage)
        {
            case 1:
                SendFight(stage);
                Summon.multiple = 1;
                break;
            case 2:
                SendFight(stage);
                Summon.multiple = 1;
                break;
            case 3:
                SendFight(stage);
                Summon.multiple = 1.5;
                break;
            case 4:
                SendFight(stage);
                Summon.multiple = 1.5;                
                break;
            case 5:
                SendFight(stage);
                Summon.multiple = 2;
                break;
            case 6:
                SendFight(stage);
                Summon.multiple = 2;
                break;
            case 7:
                SendFight(stage);
                Summon.multiple = 4;
                break;
            // �ʿ��� ����������ŭ �߰�
            default:
                Debug.Log("�߸��� ���������Դϴ�.");
                break;
        }
    }

    public void SendStory(int stage)
    {
        string sceneName;
        sceneName = "Story Screen_" + stage.ToString() + "Stage";
        SceneManager.LoadScene(sceneName);
    }
    public void SendFight(int stage)
    {
        string sceneName;
        sceneName = "Fight Screen_" + stage.ToString() + "Stage";
        SceneManager.LoadScene(sceneName);
    }

    public void SendEpilogue()
    {
        SceneManager.LoadScene("Epilogue Screen");
    }
    public int getStageNum()
    {
        return stageNum;
    }
    public void setStageNum(int stage)
    {
        stageNum = stage;
    }

}
