using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageText : MonoBehaviour
{
    public Text stageText; // ǥ���� TextMeshPro ������Ʈ

    void Start()
    {
        // PlayerPrefs���� savedStage �� �ҷ�����
        int savedStageValue = PlayerPrefs.GetInt("savedStage", 0); // �⺻�� 0
        if(savedStageValue > 7)
        {
            PlayerPrefs.SetInt("savedStage", 7);
        }

        // �гο� savedStage ���� ǥ��
        stageText.text = "<b>[" + savedStageValue.ToString() + " �������� : " + ShowTextSavedStage(savedStageValue) + "]</b>" +
                 "\n������� �����Ͻðڽ��ϱ�?";
        // ���߿� �������� �̸��鵵 �������� �� �ܰ迡 �´� ������������ �־������ ���� ������..?
    }

    private string ShowTextSavedStage(int stageNum)
    {
        string stageName = "";
        switch (stageNum)
        {
            case 1:
                stageName = "��Ȱ�� �긮�� ��� ����";
                break;
            case 2:
                stageName = "��Ȱ�� �긮�� ��� ����";
                break;
            case 3:
                stageName = "���� �������� ����";
                break;
            case 4:
                stageName = "���� �������� ������";
                break;
            case 5:
                stageName = "�߰����� ��������";
                break;
            case 6:
                stageName = "ȭ������ ����";
                break;
            case 7:
                stageName = "��ũ �巡���� ����";
                break;
        }

        return stageName;
    }
}
