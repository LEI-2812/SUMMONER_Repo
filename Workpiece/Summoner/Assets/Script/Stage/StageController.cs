using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{

    [Header("�������� ��ư��(�������)")]
    public Button[] buttons;    // Ȱ��ȭ/��Ȱ��ȭ�� ��ư��

    [Header("��ư Ŭ����")]
    [SerializeField] private AudioSource audioSource;

    private CheckStage checkStage;
    private static StageController instance;
    private void Awake()
    {
        // �ߺ� ������ ���� �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� GameObject�� �ı����� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� ������ ���� ������ ���� ����
        }
    }
    void Start()
    {
        checkStage = FindObjectOfType<CheckStage>();
        int stageNumber = PlayerPrefs.GetInt("savedStage");
        Debug.Log("savedStage �� : " + stageNumber);
        ButtonInteractivity(7);
    }

    // ���� �������� ���� ����
    [SerializeField]
    [Header("�������� ���൵")]
    private int savedStage = 7;

    // PlayerPrefs�� �������� ���� ���� ����
    void SaveStage()
    {
        PlayerPrefs.SetInt("savedStage", savedStage);
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
        PlayerPrefs.SetInt("playingStage", stage);
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
                break;
            case 2:
                SendStory(stage);
                Summon.multiple = 1;
                break;
            case 3:
                SendStory(stage);
                Summon.multiple = 1.5;
                break;
            case 4:
                SendFight(stage);
                Summon.multiple = 1.5;
                break;
            case 5:
                SendStory(stage);
                Summon.multiple = 2;
                break;
            case 6:
                SendFight(stage);
                Summon.multiple = 2;
                break;
            case 7:
                SendStory(stage);
                Summon.multiple = 4;
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

    void ButtonInteractivity(int stageNumber)
    {
        // ��� ��ư ��Ȱ��ȭ
        foreach (Button button in buttons)
        {
            button.interactable = false; // �⺻������ ��Ȱ��ȭ
        }

        for (int i = 0; i < stageNumber; i++)
        {
            if (i < buttons.Length) // �迭�� ������ ����� �ʵ��� üũ
            {
                buttons[i].interactable = true; // �ش� �ܰ��� ��ư Ȱ��ȭ
            }
        }
    }

}
