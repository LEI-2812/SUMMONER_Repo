using UnityEngine;

// 역할: Stage1Scenario의 책임을 정의한다.
public class Stage1Scenario : StoryScenarioBase
{
    [Header("참조")]
    public GameObject confusebubbleImage;
    public GameObject dotbubbleImage;
    public GameObject dialogueBox;
    public GameObject skipBtn;


    void Awake()
    {
        confusebubbleImage.SetActive(false);
        dotbubbleImage.SetActive(false);
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                HideDialogueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                ShowDialogueBox();
                break;
            case 3:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                break;
            case 4:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                HideDialogueBox();
                playerMove.CharacterMove(150f, 400f);
                break;
            case 5:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                ShowDialogueBox();
                playerMove.CharacterMove(-150f, 400f);
                break;
            case 6:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                playerMove.CharacterMove(150f, 300f);
                break;
            case 7:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                HideDialogueBox();
                break;
            case 8:
                Debug.Log(scenarioStep);
                ShowDialogueBox();
                break;
            case 9:
                Debug.Log(scenarioStep);
                HideDialogueBox();
                ShowConfuseEffect();
                break;
            case 10:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                ShowDialogueBox();
                break;
            case 11:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                HideDialogueBox();
                ShowDotbubbleEffect();
                break;
            case 12:
                Debug.Log(scenarioStep);
                // 역할: Stage1Scenario의 책임을 정의한다.
                ShowDialogueBox();
                break;
        }
    }


    public void ShowConfuseEffect()
    {
        Invoke(nameof(OnConfuseImage), 0.4f);
    }
    private void OnConfuseImage()
    {
        confusebubbleImage.SetActive(true);
        Invoke(nameof(OffConfuseImage), 1f);
    }
    private void OffConfuseImage()
    {
        confusebubbleImage.SetActive(false);
        Invoke(nameof(EndConfuseEffect), 0.4f);
    }
    private void EndConfuseEffect()
    {
        playerMove.StopConfuseAni();
    }


    public void ShowDotbubbleEffect()
    {
        dotbubbleImage.SetActive(true);
        PauseDialogue();

        Invoke(nameof(EndDotbubbleEffect), 2f);
    }
    private void EndDotbubbleEffect()
    {
        dotbubbleImage.SetActive(false);
        ResumeDialogue();
    }

    private void ShowDialogueBox()
    {
        dialogueBox.SetActive(true);
    }

    private void HideDialogueBox()
    {
        dialogueBox.SetActive(false);
    }

}
