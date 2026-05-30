using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class DialogueCsvParse
{
    public static Dialogue[] Parse(string csvText)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();
        string[] data = csvText.Split(new char[] { '\n' });
        Dialogue currentDialogue = null;
        List<string> contextList = new List<string>();

        for (int i = 1; i < data.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(data[i]))
            {
                continue;
            }

            string[] row = data[i].Split(new char[] { ',' }, 3);

            if (!string.IsNullOrEmpty(row[0]))
            {
                if (currentDialogue != null)
                {
                    currentDialogue.context = contextList.ToArray();
                    dialogueList.Add(currentDialogue);
                }

                currentDialogue = new Dialogue();
                currentDialogue.name = row[1];
                contextList = new List<string>();

                if (row.Length > 2)
                {
                    contextList.Add(CleanContext(row[2]));
                }
            }
            else if (currentDialogue != null && row.Length > 2)
            {
                contextList.Add(CleanContext(row[2]));
            }
        }

        if (currentDialogue != null)
        {
            currentDialogue.context = contextList.ToArray();
            dialogueList.Add(currentDialogue);
        }

        return dialogueList.ToArray();
    }

    private static string CleanContext(string context)
    {
        return Regex.Replace(context.Trim(), "^\\s*\"|\"\\s*$", "");
    }
}
