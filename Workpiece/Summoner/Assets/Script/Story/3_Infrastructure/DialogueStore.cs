using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 역할: CSV에서 읽은 대사를 번호별로 보관하고 범위 조회를 제공한다.
public class DialogueStore : MonoBehaviour
{
    [SerializeField]
    [Header("참조")]
    [FormerlySerializedAs("csv_FileName")]
    private string csvFileName;

    private readonly Dictionary<int, Dialogue> dialoguesByNumber = new Dictionary<int, Dialogue>();

    private void Awake()
    {
        string csvText = LoadCsvText();
        Dialogue[] dialogues = DialogueCsvParser.Parse(csvText);
        StoreDialoguesByNumber(dialogues);
    }

    private string LoadCsvText()
    {
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        return csvData.text;
    }

    private void StoreDialoguesByNumber(Dialogue[] dialogues)
    {
        dialoguesByNumber.Clear();

        foreach (Dialogue dialogue in dialogues)
        {
            if (!int.TryParse(dialogue.number, out int dialogueNumber))
            {
                Debug.LogWarning("대화 번호가 올바르지 않습니다: " + dialogue.number);
                continue;
            }

            if (dialoguesByNumber.ContainsKey(dialogueNumber))
            {
                Debug.LogWarning("중복된 대화 번호입니다: " + dialogueNumber);
                continue;
            }

            dialoguesByNumber.Add(dialogueNumber, dialogue);
        }
    }

    public Dialogue[] GetDialogues(int startNumber, int endNumber)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for (int number = startNumber; number <= endNumber; number++)
        {
            if (dialoguesByNumber.TryGetValue(number, out Dialogue dialogue))
            {
                dialogueList.Add(dialogue);
            }
            else
            {
                Debug.LogWarning("대화 번호를 찾을 수 없습니다: " + number);
            }
        }

        return dialogueList.ToArray();
    }
}
