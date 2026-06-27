using UnityEngine;

// 역할: Stage7Scenario의 책임을 정의한다.
public class Stage7Scenario : StoryScenarioBase
{
    [Header("참조")]
    public GameObject dialogueBox;


    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: //  50 ~ 58
                Debug.Log(scenarioStep);
                HideDialogueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioStep);
                ShowDialogueBox();
                break;
            case 3:
                Debug.Log(scenarioStep);
                break;
            case 4:
                Debug.Log(scenarioStep);
                // 역할: Stage7Scenario의 책임을 정의한다.
                break;
            case 5:
                Debug.Log(scenarioStep);
                // 역할: Stage7Scenario의 책임을 정의한다.
                break;
            case 6:
                Debug.Log(scenarioStep);
                // 역할: Stage7Scenario의 책임을 정의한다.
                break;
            case 7:
                Debug.Log(scenarioStep);
                // 역할: Stage7Scenario의 책임을 정의한다.
                break;
            case 8:
                Debug.Log(scenarioStep);
                // 역할: Stage7Scenario의 책임을 정의한다.
                break;
            case 9:
                Debug.Log(scenarioStep);
                // 역할: Stage7Scenario의 책임을 정의한다.
                break;
        }
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
