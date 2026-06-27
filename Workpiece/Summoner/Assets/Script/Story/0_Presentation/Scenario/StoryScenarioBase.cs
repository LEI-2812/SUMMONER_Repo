using UnityEngine;
using UnityEngine.EventSystems;

// 역할: StoryScenarioBase의 책임을 정의한다.
public abstract class StoryScenarioBase : MonoBehaviour, IPointerClickHandler
{
    private readonly GameplaySettingStore gameplaySettingStore = new GameplaySettingStore();
    private readonly StoryDialogueUseCase storyDialogueUseCase = new StoryDialogueUseCase();
    private readonly StoryStepUseCase storyStepUseCase = new StoryStepUseCase();
    private readonly StorySceneUseCase storySceneUseCase = new StorySceneUseCase();

    private StageDialogueLoader stageDialogueLoader;
    private StoryDialogueRange storyDialogueRange;
    private bool isDialogueActive;
    private bool isDialoguePaused;

    [Header("컨트롤러")]
    [SerializeField] protected StoryDialogueView storyDialogueView;
    [SerializeField] protected PlayerMove playerMove;

    protected virtual void Start()
    {
        stageDialogueLoader = storyDialogueView.GetComponent<StageDialogueLoader>();
        storyDialogueRange = storyDialogueView.GetComponent<StoryDialogueRange>();
        storyDialogueView.Clear();
        StartStory();
        RunScenarioStep();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsOnlyMouseEnabled())
        {
            OnClickDialogue();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickDialogue();
    }

    public void OnClickDialogue()
    {
        if (playerMove != null && playerMove.GetIsMoving())
        {
            return;
        }

        if (!ShowNextDialogue())
        {
            return;
        }

        storyDialogueView.ShowImage(storyDialogueUseCase.CurrentDialogueIndex);
        RunScenarioStep();
    }

    protected abstract void PlayScenarioStep(int scenarioStep);

    public void PauseDialogue()
    {
        isDialoguePaused = true;
    }

    public void ResumeDialogue()
    {
        isDialoguePaused = false;
    }

    private void StartStory()
    {
        if (isDialogueActive)
        {
            return;
        }

        if (!storyDialogueUseCase.TryStart(stageDialogueLoader.LoadCurrentStageDialogues()))
        {
            Debug.LogWarning("실행할 시나리오 내용이 없습니다.");
            return;
        }

        isDialogueActive = true;
        ShowNextDialogue();
    }

    private bool ShowNextDialogue()
    {
        if (!isDialogueActive || isDialoguePaused)
        {
            return false;
        }

        if (storyDialogueUseCase.TryGetNextLine(out Dialogue currentDialogue, out int dialogueLineIndex))
        {
            storyDialogueView.ShowDialogue(currentDialogue, dialogueLineIndex);
            return true;
        }

        EndStory();
        return false;
    }

    private void RunScenarioStep()
    {
        int currentDialogueIndex = storyDialogueUseCase.CurrentDialogueIndex;
        if (!storyStepUseCase.TryAdvance(currentDialogueIndex, out int scenarioStep))
        {
            return;
        }

        PlayScenarioStep(scenarioStep);
    }

    private void EndStory()
    {
        isDialogueActive = false;
        Debug.Log("시나리오가 종료되었습니다.");
        storyDialogueView.FadeOut(CompleteStory);
    }

    private void CompleteStory()
    {
        storySceneUseCase.LoadNextScene(storyDialogueRange.GetStoryNumber());
    }

    private bool IsOnlyMouseEnabled()
    {
        return gameplaySettingStore.LoadOnlyMouseEnabled();
    }
}
