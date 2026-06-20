using UnityEngine;

// 역할: 대화 데이터 파일을 읽어 파서에 전달한다.
public static class DialogueDatabaseLoad
{
    public static string LoadText(string csvFileName)
    {
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        return csvData.text;
    }
}
