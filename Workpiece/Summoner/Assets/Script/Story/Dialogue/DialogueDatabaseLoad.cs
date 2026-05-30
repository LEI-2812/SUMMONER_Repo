using UnityEngine;

public static class DialogueDatabaseLoad
{
    public static string LoadText(string csvFileName)
    {
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        return csvData.text;
    }
}
