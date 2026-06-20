using UnityEngine;
using UnityEngine.UI;

// 역할: 게임플레이 설정 UI 값을 표시하고 변경 입력을 저장소에 반영한다.
public class GameplaySettingView : MonoBehaviour
{
    private readonly GameplaySettingStore gameplaySettingStore = new GameplaySettingStore();

    [SerializeField] private Toggle isStorySkip;

    [SerializeField] private Toggle isOnlyMouse;

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        isStorySkip.isOn = gameplaySettingStore.LoadStorySkipEnabled();
        isOnlyMouse.isOn = gameplaySettingStore.LoadOnlyMouseEnabled();
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

    public void StorySkip()
    {
        audioSource.Play();
        if (isStorySkip.isOn)
        {
            Debug.Log("스토리를 스킵합니다.");
        }

        gameplaySettingStore.SaveStorySkipEnabled(isStorySkip.isOn);
    }

    public void OnlyUseMouse()
    {
        if (isOnlyMouse.isOn)
        {
            Debug.Log("마우스로만 조작할 수 있습니다");
        }

        gameplaySettingStore.SaveOnlyMouseEnabled(isOnlyMouse.isOn);
    }

    public void OnClickSound()
    {
        audioSource.Play();
    }

    public bool GetIsStorySkip() { return isStorySkip.isOn; }

    public void SetIsStorySkip(bool isSkip) { this.isStorySkip.isOn = isSkip; }

    public bool GetIsOnlyMouse() { return isOnlyMouse.isOn; }

    public void SetIsOnlyMouse(bool isMouse) { this.isOnlyMouse.isOn = isMouse; }
}
