// 역할: dialogue index 변화에 맞춰 스토리 연출 step을 한 번씩 진행한다.
public class StoryStepUseCase
{
    private int scenarioStep;
    private int previousDialogueIndex = -1;

    public bool TryAdvance(int currentDialogueIndex, out int nextScenarioStep)
    {
        nextScenarioStep = scenarioStep;

        if (currentDialogueIndex == previousDialogueIndex)
        {
            return false;
        }

        previousDialogueIndex = currentDialogueIndex;
        scenarioStep++;
        nextScenarioStep = scenarioStep;
        return true;
    }
}
