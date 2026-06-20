using UnityEngine;
using UnityEngine.EventSystems;

// 역할: 스테이지 시나리오 컨트롤러들의 공통 클릭 진행 흐름을 제공한다.
public abstract class StoryScenarioControllerBase : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    private readonly GameplaySettingStore gameplaySettingStore = new GameplaySettingStore();

    [Header("컨트롤러")]
    [SerializeField] protected InteractionController interactionController;
    [SerializeField] protected PlayerMove playerMove;

    private int scenarioFlowCount = 0;
    private int sameDialogueIndex = -1;

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsOnlyMouseEnabled())
        {
            OnClickDialogue();
        }
    }

    public void ScenarioFlow()
    {
        if (IsSameDialogueIndex())
        {
            return;
        }

        PlayScenarioStep(scenarioFlowCount);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickDialogue();
    }

    public void OnClickDialogue()
    {
        if (!playerMove.GetIsMoving())
        {
            interactionController.ShowNextLine();
            ScenarioFlow();
        }
    }

    protected abstract void PlayScenarioStep(int scenarioStep);

    private bool IsOnlyMouseEnabled()
    {
        return gameplaySettingStore.LoadOnlyMouseEnabled();
    }

    private bool IsSameDialogueIndex()
    {
        int currentDialogueIndex = interactionController.GetCurrentDialogueIndex();

        if (currentDialogueIndex == sameDialogueIndex)
        {
            return true;
        }

        sameDialogueIndex = currentDialogueIndex;
        scenarioFlowCount++;
        return false;
    }
}
