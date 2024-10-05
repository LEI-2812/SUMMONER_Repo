using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour
{
    [Header("스토리 스킵툴팁")]
    [SerializeField] private Toggle isStorySkip;

    [Header("마우스만 사용가능 툴팁")]
    [SerializeField] private Toggle isOnlyMouse;


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
        if (isStorySkip.isOn)
        {
            Debug.Log("스토리를 스킵합니다.");
        }
        else
            Debug.Log("스킵하지않습니다.");
    }

    public void onlyUseMouse()
    {
        if (isOnlyMouse.isOn)
        {
            Debug.Log("마우스로만 조작할 수 있습니다");
        }
        else
            Debug.Log("키보드 & 마우스 사용가능");
    }

    public bool getIsStorySkip(){ return isStorySkip.isOn; }

    public void setIsStorySkip(bool isSkip) { this.isStorySkip.isOn = isSkip; }

    public bool getIsOnlyMouse() { return isOnlyMouse.isOn; }

    public void setIsOnlyMouse(bool isMouse) { this.isOnlyMouse.isOn = isMouse; }
}
