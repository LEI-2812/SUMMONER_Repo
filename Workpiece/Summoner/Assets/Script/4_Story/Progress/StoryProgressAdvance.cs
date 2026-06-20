// 역할: 스토리 진행 단계를 다음 단계로 갱신한다.
public class StoryProgressAdvance
{
    private Dialogue[] dialogues;

    public int CurrentDialogueIndex { get; private set; }
    public int CurrentDialogueLineIndex { get; private set; }

    public void Start(Dialogue[] nextDialogues)
    {
        dialogues = nextDialogues;
        CurrentDialogueIndex = 0;
        CurrentDialogueLineIndex = 0;
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
