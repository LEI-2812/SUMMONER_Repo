using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingView : MonoBehaviour
{
    [SerializeField] private Toggle isStorySkip;

    [SerializeField] private Toggle isOnlyMouse;

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
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

        PlayerPrefs.SetInt("IsOnlyMouse", isOnlyMouse.isOn ? 1 : 0);
        PlayerPrefs.Save();
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
