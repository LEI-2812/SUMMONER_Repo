using UnityEngine;
using UnityEngine.UI;

// 역할: 스토리 스킵 UI 표시와 입력을 담당한다.
public class StorySkipView : MonoBehaviour
{
    [Header("스킵 알림 핸들러")]
    private SkipAlertHandler skipAlertHandler;
    public GameObject SkipBtn;

    [SerializeField] private AudioSource skipSound;

    private SettingPanelView setting;

    private void Start()
    {
        skipAlertHandler = FindObjectOfType<SkipAlertHandler>();
        if (skipAlertHandler == null)
        {
            Debug.LogWarning("스킵 알림 핸들러가 할당되지 않았습니다.");
        }

        setting = SettingPanelView.instance;

        if (setting == null)
        {
            Debug.LogWarning("SettingPanelView 인스턴스가 존재하지 않습니다.");
        }

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
        if (setting != null)
        {
            if (setting.GetGamePlayController().GetIsStorySkip())
            {
                SkipBtn.SetActive(true);
            }
            else
            {
                SkipBtn.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("설정이 할당되지 않았습니다.");
        }
    }

    public void SkipAlert()
    {
        if (skipAlertHandler == null)
        {
            Debug.LogError("SkipAlertHandler가 할당되지 않았습니다.");
            return;
        }

        skipSound.Play();
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
}
