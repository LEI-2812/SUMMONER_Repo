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
        ButtonInteractivity(stageNumber);
    }

    // ���� �������� ���� ����
    [SerializeField]
    [Header("�������� ���൵")]
    private int savedStage = 0;

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

        if (stage == 4 || stage == 6)
        {
            string sceneName = "Fight Screen_" + stage.ToString() + "Stage";
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            string sceneName = "Story Screen_" + stage.ToString() + "Stage";
            SceneManager.LoadScene(sceneName);
        }
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
