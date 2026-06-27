using UnityEngine;

// 역할: PlayerFeedbackView의 책임을 정의한다.
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
