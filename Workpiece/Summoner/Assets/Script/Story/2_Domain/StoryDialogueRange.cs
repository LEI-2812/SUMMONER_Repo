using UnityEngine;
using UnityEngine.Serialization;

// 역할: 스토리 번호에 맞는 CSV 대사 번호 범위를 제공한다.
public class StoryDialogueRange : MonoBehaviour
{
    [Header("참조")]
    [FormerlySerializedAs("storyNum")]
    [SerializeField] private int storyNumber;

    [SerializeField] private StoryDialogueRangeItem[] dialogueRanges =
    {
        new StoryDialogueRangeItem(0, 1, 9),
        new StoryDialogueRangeItem(1, 16, 27),
        new StoryDialogueRangeItem(2, 28, 33),
        new StoryDialogueRangeItem(3, 34, 40),
        new StoryDialogueRangeItem(5, 41, 48),
        new StoryDialogueRangeItem(7, 49, 57),
        new StoryDialogueRangeItem(8, 10, 15),
    };

    private int startDialogueNumber = 0;
    private int endDialogueNumber = 0;

    public void SetRangeByStoryNumber()
    {
        foreach (StoryDialogueRangeItem dialogueRange in dialogueRanges)
        {
            if (dialogueRange.storyNumber != storyNumber)
            {
                continue;
            }

            startDialogueNumber = dialogueRange.startDialogueNumber;
            endDialogueNumber = dialogueRange.endDialogueNumber;
            return;
        }

        Debug.LogWarning("스토리 대사 범위를 찾을 수 없습니다: " + storyNumber);
    }

    public int GetStoryNumber()
    {
        return storyNumber;
    }

    public void SetStoryNumber(int nextStoryNumber)
    {
        storyNumber = nextStoryNumber;
    }

    public int GetStartDialogueNumber()
    {
        return startDialogueNumber;
    }

    public int GetEndDialogueNumber()
    {
        return endDialogueNumber;
    }

    [System.Serializable]
    private class StoryDialogueRangeItem
    {
        public int storyNumber;
        public int startDialogueNumber;
        public int endDialogueNumber;

        public StoryDialogueRangeItem(int storyNumber, int startDialogueNumber, int endDialogueNumber)
        {
            this.storyNumber = storyNumber;
            this.startDialogueNumber = startDialogueNumber;
            this.endDialogueNumber = endDialogueNumber;
        }
    }
}
