using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour
{
    [Header("���丮 ��ŵ����")]
    public Toggle isStorySkip;

    [Header("���콺�� ��밡�� ����")]
    public Toggle isOnlyMouse;

    private void Update()
    {
        if (isOnlyMouse.isOn)
        {
            // �ƹ� �Էµ� ó������ ����
            if (Input.anyKey)
            {
                return;
            }
        }
    }

    public void storySkip()
    {
        if (isStorySkip.isOn)
        {
            Debug.Log("���丮�� ��ŵ�մϴ�.");
        }
        else
            Debug.Log("��ŵ�����ʽ��ϴ�.");
    }

    public void onlyUseMouse()
    {
        if (isOnlyMouse.isOn)
        {
            Debug.Log("���콺�θ� ������ �� �ֽ��ϴ�");
        }
        else
            Debug.Log("Ű���� & ���콺 ��밡��");
    }
}
