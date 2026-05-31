using UnityEngine;
using UnityEngine.EventSystems;

public abstract class StoryScenarioControllerBase : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("컨트롤러")]
    [SerializeField] protected InteractionController interactionController;
    [SerializeField] protected PlayerMove playerMove;

    private int scenarioFlowCount = 0;
    private int sameDialogueIndex = -1;

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickDialogue();
        }
    }

    public void scenarioFlow()
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
        if (!playerMove.getIsMoving())
        {
            interactionController.ShowNextLine();
            scenarioFlow();
        }
    }

    protected abstract void PlayScenarioStep(int scenarioStep);

    private bool IsSameDialogueIndex()
    {
        int currentDialogueIndex = interactionController.getCurrentDialogueIndex();

        if (currentDialogueIndex == sameDialogueIndex)
        {
            return true;
        }

        sameDialogueIndex = currentDialogueIndex;
        scenarioFlowCount++;
        return false;
    }
}
