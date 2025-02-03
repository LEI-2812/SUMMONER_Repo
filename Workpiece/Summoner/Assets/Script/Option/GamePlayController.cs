using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour
{
    public static GamePlayController instance;

    [Header("스토리 스킵툴팁")]
    public Toggle isStorySkip;

    [Header("마우스만 사용가능 툴팁")]
    public Toggle isOnlyMouse;

    [Header("버튼 클릭음")]
    public AudioSource audioSource;

    private void Awake()
    {

        // PlayerPrefs에서 이전 설정 불러오기
        isStorySkip.isOn = PlayerPrefs.GetInt("IsStorySkip", 0) == 1;
        isOnlyMouse.isOn = PlayerPrefs.GetInt("IsOnlyMouse", 0) == 1;
    }



    private void Update()
    {
        if (isOnlyMouse.isOn)
        {
            // 아무 입력도 처리하지 않음
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
            Debug.Log("스토리를 스킵합니다.");
        }
        else
            Debug.Log("스킵하지않습니다.");
        // 설정 저장
        PlayerPrefs.SetInt("IsStorySkip", isStorySkip.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void onlyUseMouse()
    {
        if (isOnlyMouse.isOn)
        {
            Debug.Log("마우스로만 조작할 수 있습니다");
        }
        else
            Debug.Log("키보드 & 마우스 사용가능");

        // 설정 저장
        PlayerPrefs.SetInt("IsOnlyMouse", isOnlyMouse.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void onClickSound()
    {
        audioSource.Play();
    }
}