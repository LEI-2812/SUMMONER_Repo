using UnityEngine;

// 역할: 플레이어 행동의 사운드와 로그 피드백만 담당한다.
public class PlayerFeedbackView
{
    private readonly AudioSource clickSound;
    private readonly AudioSource failSound;

    public PlayerFeedbackView(AudioSource clickSound, AudioSource failSound)
    {
        this.clickSound = clickSound;
        this.failSound = failSound;
    }

    public void PlayClick()
    {
        clickSound.Play();
    }

    public void PlayFail()
    {
        failSound.Play();
    }

    public void Log(string message)
    {
        Debug.Log(message);
    }

    public void LogError(string message)
    {
        Debug.LogError(message);
    }
}
