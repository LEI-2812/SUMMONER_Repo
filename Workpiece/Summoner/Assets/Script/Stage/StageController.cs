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
        string sceneName;

        switch (stage)
        {
            case 1:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 1;
                break;
            case 2:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 1;
                break;
            case 3:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 1.5;
                break;
            case 4:
                Summon.multiple = 1.5;
                sceneName = "Fight Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                break;
            case 5:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 2;
                break;
            case 6:
                sceneName = "Fight Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 2;
                break;
            case 7:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 4;
                break;
            // �ʿ��� ����������ŭ �߰�
            default:
                Debug.Log("�߸��� ���������Դϴ�.");
                break;
        }

        //if (stage == 4 || stage == 6)
        //{
        //    string sceneName = "Fight Screen_" + stage.ToString() + "Stage";
        //    SceneManager.LoadScene(sceneName);
        //}
        //else
        //{
        //    string sceneName = "Story Screen_" + stage.ToString() + "Stage";
        //    SceneManager.LoadScene(sceneName);
        //}
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
