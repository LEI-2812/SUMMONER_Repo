using UnityEngine;

// 역할: 현재 스토리 번호에 맞는 대사 목록을 불러온다.
public class StageDialogueLoader : MonoBehaviour
{
    [SerializeField]
    [Header("Dialogue")]
    private DialogueEvent dialogue;

    [SerializeField] private DialogueStore dialogueStore;

    private StoryDialogueRange storyDialogueRange;

    private void Awake()
    {
        storyDialogueRange = GetComponent<StoryDialogueRange>();
        if (dialogueStore == null)
        {
            dialogueStore = GetComponent<DialogueStore>();
        }
    }

    public Dialogue[] LoadCurrentStageDialogues()
    {
        storyDialogueRange.SetRangeByStoryNumber();
        dialogue.line.x = storyDialogueRange.GetStartDialogueNumber();
        dialogue.line.y = storyDialogueRange.GetEndDialogueNumber();
        dialogue.dialogues = dialogueStore.GetDialogues((int)dialogue.line.x, (int)dialogue.line.y);
        return dialogue.dialogues;
    }
}
