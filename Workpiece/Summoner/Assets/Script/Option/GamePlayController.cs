using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour
{
    public static GamePlayController instance;

    [Header("���丮 ��ŵ����")]
    public Toggle isStorySkip;

    [Header("���콺�� ��밡�� ����")]
    public Toggle isOnlyMouse;

    [Header("��ư Ŭ����")]
    public AudioSource audioSource;

    private void Awake()
    {

        // PlayerPrefs���� ���� ���� �ҷ�����
        isStorySkip.isOn = PlayerPrefs.GetInt("IsStorySkip", 0) == 1;
        isOnlyMouse.isOn = PlayerPrefs.GetInt("IsOnlyMouse", 0) == 1;
    }



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
        audioSource.Play();
        if (isStorySkip.isOn)
        {
            Debug.Log("���丮�� ��ŵ�մϴ�.");
        }
        else
            Debug.Log("��ŵ�����ʽ��ϴ�.");
        // ���� ����
        PlayerPrefs.SetInt("IsStorySkip", isStorySkip.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void onlyUseMouse()
    {
        if (isOnlyMouse.isOn)
        {
            Debug.Log("���콺�θ� ������ �� �ֽ��ϴ�");
        }
        else
            Debug.Log("Ű���� & ���콺 ��밡��");

        // ���� ����
        PlayerPrefs.SetInt("IsOnlyMouse", isOnlyMouse.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void onClickSound()
    {
        audioSource.Play();
    }
}