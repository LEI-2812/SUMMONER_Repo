// 역할: 스토리 대화 진행 상태와 다음 대화 줄 조회를 처리한다.
public class StoryDialogueUseCase
{
    private Dialogue[] dialogues;

    public int CurrentDialogueIndex { get; private set; }
    public int CurrentDialogueLineIndex { get; private set; }

    public bool TryStart(Dialogue[] dialogues)
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            return false;
        }

        this.dialogues = dialogues;
        CurrentDialogueIndex = 0;
        CurrentDialogueLineIndex = 0;
        return true;
    }

    public bool TryGetNextLine(out Dialogue dialogue, out int dialogueLineIndex)
    {
        dialogue = null;
        dialogueLineIndex = 0;

        if (dialogues == null)
        {
            return false;
        }

        while (CurrentDialogueIndex < dialogues.Length
            && CurrentDialogueLineIndex >= dialogues[CurrentDialogueIndex].context.Length)
        {
            CurrentDialogueLineIndex = 0;
            CurrentDialogueIndex++;
        }

        if (CurrentDialogueIndex >= dialogues.Length)
        {
            return false;
        }

        dialogue = dialogues[CurrentDialogueIndex];
        dialogueLineIndex = CurrentDialogueLineIndex;
        CurrentDialogueLineIndex++;
        return true;
    }
}
