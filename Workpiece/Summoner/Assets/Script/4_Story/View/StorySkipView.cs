using UnityEngine;
using UnityEngine.UI;

// 역할: 스토리 스킵 UI 표시와 입력을 담당한다.
public class StorySkipView : MonoBehaviour
{
    [Header("스킵 알림 핸들러")]
    private SkipAlertHandler skipAlertHandler;
    public GameObject SkipBtn;

    [SerializeField] private AudioSource skipSound;

    private readonly GameplaySettingStore gameplaySettingStore = new GameplaySettingStore();

    private void Start()
    {
        skipAlertHandler = FindObjectOfType<SkipAlertHandler>();
        IsSkipActive();
    }

    private void Update()
    {
        // 버튼이 활성화 되어있고 그 상태에서 엔터를 누르면 스킵창 출력
        if (Input.GetKeyDown(KeyCode.Return) && SkipBtn.activeSelf)
        {
            SkipAlert();
        }
        IsSkipActive();
    }

    public void IsSkipActive()
    {
        SkipBtn.SetActive(gameplaySettingStore.LoadStorySkipEnabled());
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
                StorySceneFlow.LoadCurrentPlayingFightScene();
            }
            else
            {
                skipAlertHandler.HideAlert();
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
