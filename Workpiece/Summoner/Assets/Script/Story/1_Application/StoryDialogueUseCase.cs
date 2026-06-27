// 역할: 스토리 대화 진행 상태와 다음 대화 줄 조회를 처리한다.
public class StoryDialogueUseCase
{
    private readonly DialogueProgress dialogueProgress = new DialogueProgress();

    public int CurrentDialogueIndex
    {
        get { return dialogueProgress.CurrentDialogueIndex; }
    }

    public int CurrentDialogueLineIndex
    {
        get { return dialogueProgress.CurrentDialogueLineIndex; }
    }

    public bool TryStart(Dialogue[] dialogues)
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            return false;
        }

        dialogueProgress.Start(dialogues);
        return true;
    }

    public bool TryGetNextLine(out Dialogue dialogue, out int dialogueLineIndex)
    {
        return dialogueProgress.TryGetNextLine(out dialogue, out dialogueLineIndex);
    }
}
