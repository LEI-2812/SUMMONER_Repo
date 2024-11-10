using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelecter : MonoBehaviour
{
    [Header("�������� ��ư��(�������)")]
    public Button[] buttons;    // Ȱ��ȭ/��Ȱ��ȭ�� ��ư��

    [Header("��ư Ŭ����")]
    [SerializeField] private AudioSource audioSource;

    private StageController stageController;

    private void Awake()
    {
        stageController = FindObjectOfType<StageController>();
    }

    private void Start()
    {
        int stageNum = stageController.getStageNum();
        ButtonInteractivity(stageNum);
    }

    public void stageLoader(int stage)
    {
        Debug.Log("��ư Ŭ��");
        audioSource.Play();
        stageController.setStageNum(stage); //���� �������� ������ ��Ʈ�ѷ��� ����
        PlayerPrefs.SetInt("playingStage", stage);
        PlayerPrefs.Save();
        SendStage(stage);
    }

    public void SendStage(int stage)    // �������� ���� ȭ�鿡�� ���� ��(���丮 + ����)
    {
        switch (stage)
        {
            case 1:
                Summon.multiple = 1;
                SendStory(stage);
                break;
            case 2:
                Summon.multiple = 1;
                SendStory(stage);
                break;
            case 3:
                Summon.multiple = 1.5;
                SendStory(stage);
                break;
            case 4:
                Summon.multiple = 1.5;
                SendFight(stage);
                break;
            case 5:
                Summon.multiple = 2;
                SendStory(stage);
                break;
            case 6:
                Summon.multiple = 2;
                SendFight(stage);
                break;
            case 7:
                Summon.multiple = 4;
                SendStory(stage);
                break;
            // �ʿ��� ����������ŭ �߰�
            default:
                Debug.Log("�߸��� ���������Դϴ�.");
                break;
        }
    }

    //���丮������ �̵�
    private void SendStory(int stage)
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
