using UnityEngine;

// 역할: 불러온 대화 데이터를 게임에서 사용할 구조로 해석한다.
public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSV_FileName)
    {
        return DialogueCsvParse.Parse(DialogueDatabaseLoad.LoadText(CSV_FileName));
    }
}
