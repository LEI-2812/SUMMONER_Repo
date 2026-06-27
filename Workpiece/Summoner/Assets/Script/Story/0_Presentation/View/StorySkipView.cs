using UnityEngine;
using UnityEngine.UI;

// 역할: StorySkipView의 책임을 정의한다.
public class StorySkipView : MonoBehaviour
{
    [Header("참조")]
    private SkipAlertHandler skipAlertHandler;
    public GameObject SkipBtn;

    [SerializeField] private AudioSource skipSound;

    private readonly StorySkipUseCase storySkipUseCase = new StorySkipUseCase();

    private void Start()
    {
        skipAlertHandler = FindObjectOfType<SkipAlertHandler>();
        IsSkipActive();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && SkipBtn.activeSelf)
        {
            SkipAlert();
        }
        IsSkipActive();
    }

    public void IsSkipActive()
    {
        SkipBtn.SetActive(storySkipUseCase.IsSkipEnabled());
    }

    public void SkipAlert()
    {
        ResolveSkipAlertHandler();
        if (skipAlertHandler == null)
        {
            Debug.LogError("SkipAlertHandler가 할당되지 않았습니다.");
            return;
        }

        if (skipSound != null)
        {
            skipSound.Play();
        }

        skipAlertHandler.ShowAlert(result =>
        {
            if (result)
            {
                Debug.Log("스킵하여 전투 씬으로 이동");
                storySkipUseCase.SkipToCurrentPlayingFight();
            }
        });
    }

    private void ResolveSkipAlertHandler()
    {
        if (skipAlertHandler != null)
        {
            return;
        }

        skipAlertHandler = FindObjectOfType<SkipAlertHandler>();
    }
}
