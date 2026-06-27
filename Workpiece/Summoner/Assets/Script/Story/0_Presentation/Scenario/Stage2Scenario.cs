using UnityEngine;

// 역할: Stage2Scenario의 책임을 정의한다.
public class Stage2Scenario : StoryScenarioBase
{
    [Header("참조")]
    public GameObject bangImage;
    public GameObject confusebubbleImage;
    public GameObject dialogueBox;

    void Awake()
    {
        bangImage.SetActive(false);
        confusebubbleImage.SetActive(false);
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: // 28 ~ 33
                Debug.Log(scenarioStep);
                // 역할: Stage2Scenario의 책임을 정의한다.
                ShowConfuseEffect();
                break;
            case 2:
                Debug.Log(scenarioStep);
                break;
            case 3:
                Debug.Log(scenarioStep);
                // 역할: Stage2Scenario의 책임을 정의한다.
                ShowBangEffect();
                break;
            case 4:
                Debug.Log(scenarioStep);
                break;
            case 5:
                Debug.Log(scenarioStep);
                HideDialogueBox();
                ShowYellowEffect();
                break;
            case 6:
                Debug.Log(scenarioStep);
                // 역할: Stage2Scenario의 책임을 정의한다.
                ShowDialogueBox();
                break;
        }
    }

    public void ShowConfuseEffect()
    {
        confusebubbleImage.SetActive(true);
        PauseDialogue();

        Invoke(nameof(EndConfuseEffect), 1.5f);
    }
    private void EndConfuseEffect()
    {
        confusebubbleImage.SetActive(false);
        ResumeDialogue();
    }

    public void ShowBangEffect()
    {
        bangImage.SetActive(true);
        PauseDialogue();

        Invoke(nameof(EndBangEffect), 1f);
    }
    private void EndBangEffect()
    {
        bangImage.SetActive(false);
        ResumeDialogue();
    }

    public void ShowYellowEffect()
    {
        playerMove.PlayYellowAni();
    }

    private void ShowDialogueBox()
    {
        dialogueBox.SetActive(true);
    }

    private void HideDialogueBox()
    {
        Debug.Log("대화창 닫기");
        dialogueBox.SetActive(false);
    }
}
