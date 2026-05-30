using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingView : MonoBehaviour
{
    [Header("ìŠ¤í† ë¦¬ ìŠ¤í‚µíˆ´íŒ")]
    [SerializeField] private Toggle isStorySkip;

    [Header("ë§ˆìš°ìŠ¤ë§Œ ì‚¬ìš©ê°€ëŠ¥ íˆ´íŒ")]
    [SerializeField] private Toggle isOnlyMouse;

    [Header("ë²„íŠ¼ í´ë¦­ìŒ")]
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        // PlayerPrefs¿¡¼­ ÀÌÀü ¼³Á¤ ºÒ·¯¿À±â
        isStorySkip.isOn = PlayerPrefs.GetInt("IsStorySkip", 0) == 1;
        isOnlyMouse.isOn = PlayerPrefs.GetInt("IsOnlyMouse", 0) == 1;
    }

    private void Update()
    {
        if (isOnlyMouse.isOn)
        {
            // ì•„ë¬´ ì…ë ¥ë„ ì²˜ë¦¬í•˜ì§€ ì•ŠìŒ
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
            Debug.Log("ìŠ¤í† ë¦¬ë¥¼ ìŠ¤í‚µí•©ë‹ˆë‹¤.");
        }
        else
<<<<<<< Updated upstream:Workpiece/Summoner/Assets/Script/Option/GamePlayController.cs
            Debug.Log("½ºÅµÇÏÁö¾Ê½À´Ï´Ù.");
        // ¼³Á¤ ÀúÀå
        PlayerPrefs.SetInt("IsStorySkip", isStorySkip.isOn ? 1 : 0);
        PlayerPrefs.Save();
=======
            Debug.Log("ìŠ¤í‚µí•˜ì§€ì•ŠìŠµë‹ˆë‹¤.");
>>>>>>> Stashed changes:Workpiece/Summoner/Assets/Script/Option/GameplaySettingView.cs
    }

    public void onlyUseMouse()
    {
        if (isOnlyMouse.isOn)
        {
            Debug.Log("ë§ˆìš°ìŠ¤ë¡œë§Œ ì¡°ì‘í•  ìˆ˜ ìˆìŠµë‹ˆë‹¤");
        }
        else
<<<<<<< Updated upstream:Workpiece/Summoner/Assets/Script/Option/GamePlayController.cs
            Debug.Log("Å°º¸µå & ¸¶¿ì½º »ç¿ë°¡´É");

        // ¼³Á¤ ÀúÀå
        PlayerPrefs.SetInt("IsOnlyMouse", isOnlyMouse.isOn ? 1 : 0);
        PlayerPrefs.Save();
=======
            Debug.Log("í‚¤ë³´ë“œ & ë§ˆìš°ìŠ¤ ì‚¬ìš©ê°€ëŠ¥");
>>>>>>> Stashed changes:Workpiece/Summoner/Assets/Script/Option/GameplaySettingView.cs
    }

    public void onClickSound()
    {
        audioSource.Play();
    }

    public bool getIsStorySkip() { return isStorySkip.isOn; }

    public void setIsStorySkip(bool isSkip) { this.isStorySkip.isOn = isSkip; }

    public bool getIsOnlyMouse() { return isOnlyMouse.isOn; }

    public void setIsOnlyMouse(bool isMouse) { this.isOnlyMouse.isOn = isMouse; }
}
