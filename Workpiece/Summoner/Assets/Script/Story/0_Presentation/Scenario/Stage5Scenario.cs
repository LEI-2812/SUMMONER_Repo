using UnityEngine;

// 역할: Stage5Scenario의 책임을 정의한다.
public class Stage5Scenario : StoryScenarioBase
{
    [Header("참조")]
    public GameObject bangImage;
    public GameObject dialogueBox;


    [Header("컨트롤러")]
    [SerializeField] private EnemyMove enemyMove;

    [Header("참조")]
    [SerializeField] private AudioClip bangSound;
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        bangImage.SetActive(false);
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: // 42 ~ 49
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
                PlayBangSound();
                ShowBangEffect();
                HideDialogueBox();
                break;
            case 4:
                Debug.Log(scenarioStep);
                ShowDialogueBox();
                break;
            case 5:
                Debug.Log(scenarioStep);
                //  ...
                break;
            case 6:
                Debug.Log(scenarioStep);
                break;
            case 7:
                Debug.Log(scenarioStep);
                enemyMove.CharacterMove(-600f, 550f);
                HideDialogueBox();
                break;
            case 8:
                Debug.Log(scenarioStep);
                ShowDialogueBox();
                break;
        }
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
    private void PlayBangSound()
    {
        if (audioSource != null && bangSound != null)
        {
            audioSource.PlayOneShot(bangSound);
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
