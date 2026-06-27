using UnityEngine;

// 역할: Stage3Scenario의 책임을 정의한다.
public class Stage3Scenario : StoryScenarioBase
{
    [Header("참조")]
    public GameObject angryImage;
    public GameObject characterFox;
    public GameObject dialogueBox;

    [SerializeField] private Animator foxAni;

    [Header("컨트롤러")]
    [SerializeField] private FoxMove foxMove;

    [Header("참조")]
    [SerializeField] private AudioSource angrySound;

    void Awake()
    {
        angryImage.SetActive(false);
        characterFox.SetActive(false);
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: //  34 ~ 40
                Debug.Log(scenarioStep);
                HideDialogueBox();
                playerMove.CharacterMove(860f, 400f);
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
                ShowBlueEffect();
                HideDialogueBox();
                break;
            case 5:
                Debug.Log(scenarioStep);
                // 역할: Stage3Scenario의 책임을 정의한다.
                ShowDialogueBox();
                break;
            case 6:
                Debug.Log(scenarioStep);
                ShowAngryEffect();
                angrySound.Play();
                HideDialogueBox();
                break;
            case 7:
                Debug.Log(scenarioStep);
                ShowDialogueBox();
                break;
        }
    }

    public void ShowAngryEffect()
    {
        angryImage.SetActive(true);
        foxMove.PlayAngryAni();
        //PauseDialogue();
    }
    private void EndAngryEffect()
    {
        angryImage.SetActive(false);
        foxMove.StopAngryAni();
        //ResumeDialogue();
    }

    public void ShowBlueEffect()
    {
        playerMove.PlayBlueAni();
        Invoke(nameof(ShowFox), 1.5f);
    }

    private void ShowFox()
    {
        characterFox.SetActive(true);
        foxAni.Play("Fox_Idle");
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
