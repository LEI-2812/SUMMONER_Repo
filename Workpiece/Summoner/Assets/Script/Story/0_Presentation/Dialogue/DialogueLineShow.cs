using UnityEngine.UI;

// 역할: DialogueLineShow의 책임을 정의한다.
public static class DialogueLineShow
{
    public static void Show(Text characterName, Text dialogueContext, Dialogue dialogue, int dialogueLineIndex)
    {
        characterName.text = dialogue.name;
        dialogueContext.text = BuildCombinedDialogue(dialogue, dialogueLineIndex);
    }

    public static string BuildCombinedDialogue(Dialogue dialogue, int dialogueLineIndex)
    {
        string combinedDialogue = "";

        for (int i = 0; i <= dialogueLineIndex; i++)
        {
            if (i % 2 == 0)
            {
                combinedDialogue = "";
            }

            combinedDialogue += dialogue.context[i];

            if (i < dialogueLineIndex)
            {
                combinedDialogue += "\n";
            }
        }

        return combinedDialogue;
    }
}
