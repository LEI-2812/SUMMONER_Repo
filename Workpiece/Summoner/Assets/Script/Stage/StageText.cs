using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageText : MonoBehaviour
{
    public TMP_Text stageText; // ǥ���� TextMeshPro ������Ʈ

    void Start()
    {
        // PlayerPrefs���� savedStage �� �ҷ�����
        int savedStageValue = PlayerPrefs.GetInt("savedStage", 0); // �⺻�� 0

        // �гο� savedStage ���� ǥ��
        stageText.text = "<b>[" + savedStageValue.ToString() + "�������� : �ӽ� ��������]</b>" +
                            "<br>������� �����Ͻðڽ��ϱ�?";
        // ���߿� �������� �̸��鵵 �������� �� �ܰ迡 �´� ������������ �־������ ���� ������..?
    }
}
