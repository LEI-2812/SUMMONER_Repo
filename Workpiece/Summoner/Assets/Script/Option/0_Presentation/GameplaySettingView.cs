using UnityEngine;
using UnityEngine.UI;

// 역할: GameplaySettingView의 책임을 정의한다.
public class GameplaySettingView : MonoBehaviour
{
    private readonly GameplaySettingUseCase gameplaySettingUseCase = new GameplaySettingUseCase();

    [SerializeField] private Toggle isStorySkip;

    [SerializeField] private Toggle isOnlyMouse;

    [Header("참조")]
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        isStorySkip.isOn = gameplaySettingUseCase.LoadStorySkipEnabled();
        isOnlyMouse.isOn = gameplaySettingUseCase.LoadOnlyMouseEnabled();
    }

    public void StorySkip()
    {
        audioSource.Play();
        if (isStorySkip.isOn)
        {
        Debug.Log("스토리를 스킵합니다.");
        }

        gameplaySettingUseCase.SaveStorySkipEnabled(isStorySkip.isOn);
    }

    public void OnlyUseMouse()
    {
        if (isOnlyMouse.isOn)
        {
        Debug.Log("마우스로 조작할 수 있습니다.");
        }

        gameplaySettingUseCase.SaveOnlyMouseEnabled(isOnlyMouse.isOn);
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
