using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSV_FileName)
    {
        return DialogueCsvParse.Parse(DialogueDatabaseLoad.LoadText(CSV_FileName));
    }
}
