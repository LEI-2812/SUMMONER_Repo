using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CheckStage : MonoBehaviour
{
    public int stageNum;  //�������� ��ȣ �ޱ�
    public static int x = 0;
    public static int y = 0;

    public void checkStage() // �������� ��ȣ���� ����� ���丮 �ٸ��� ����
    {
        switch (stageNum)
        {
            case 0: // ���ѷα� ��� ����, ���� ����
                x = 1; y = 9;
                break;
            case 1:  // 1�������� ��� ����, ���� ����
                x = 16; y = 26;
                break;
            case 2:  // 2�������� ��� ����, ���� ����
                x = 27; y = 31;
                break;
            case 3:  // 3�������� ��� ����, ���� ����
                x = 32; y = 35;
                break;
            case 5:  // 5�������� ��� ����, ���� ����
                x = 36; y = 40;
                break;
            case 7:  // 7�������� ��� ����, ���� ����
                x = 41; y = 50;
                break;
            case 8:  // ���ʷα� ��� ����, ���� ����
                x = 10; y = 15;
                break;
            default:
                Debug.Log("�߸��� �������� ��ȣ�Դϴ�.");
                break;
        }
    }
}
