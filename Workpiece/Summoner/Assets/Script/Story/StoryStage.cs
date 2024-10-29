using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StoryStage : MonoBehaviour
{
    [Header("���丮 �������� ��ȣ�Է�")]
    [SerializeField]private int storyNum;  //�������� ��ȣ �ޱ�
    private int x = 0;
    private int y = 0;

    public void checkStage() // �������� ��ȣ���� ����� ���丮 �ٸ��� ����
    {
        switch (storyNum)
        {
            case 0: // ���ѷα� ��� ����, ���� ����
                x = 1; y = 9;
                break;
            case 1:  // 1�������� ��� ����, ���� ����
                x = 16; y = 27;
                break;
            case 2:  // 2�������� ��� ����, ���� ����
                x = 28; y = 33;
                break;
            case 3:  // 3�������� ��� ����, ���� ����
                x = 34; y = 40;
                break;
            case 5:  // 5�������� ��� ����, ���� ����
                x = 41; y = 48;
                break;
            case 7:  // 7�������� ��� ����, ���� ����
                x = 49; y = 57;
                break;
            case 8:  // ���ʷα� ��� ����, ���� ����
                x = 10; y = 15;
                break;
            default:
                Debug.Log("�߸��� �������� ��ȣ�Դϴ�.");
                break;
        }
    }

    public int getStoryNum()
    {
        return storyNum;
    }
    public void setStoryNum(int stage)
    {
        storyNum = stage;
    }
    public int getX()
    {
        return x;
    }
    public int getY()
    {
        return y;
    }
}
