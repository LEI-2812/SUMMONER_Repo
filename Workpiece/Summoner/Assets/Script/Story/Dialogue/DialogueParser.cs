using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSV_FileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // Dialogue ����Ʈ
        TextAsset csvData = Resources.Load<TextAsset>(CSV_FileName); // CSV ���� ������

        string[] data = csvData.text.Split(new char[] { '\n' }); // �ึ�� �б�
        Dialogue currentDialogue = null; // ���� ��ȭ�� ������ ����
        List<string> contextList = new List<string>(); // ��縦 ���� ����Ʈ

        for (int i = 1; i < data.Length; i++) // csv ���� ��ü�� �д´�
        {
            if (string.IsNullOrWhiteSpace(data[i])) continue; // �� �� �ǳʶ�

            string[] row = data[i].Split(new char[] { ',' }, 3); // ó�� �� �κи� �и��ϰ� �������� ���� ��ħ

            if (!string.IsNullOrEmpty(row[0])) // ID�� �ִ� ���, ���ο� ��� ����
            {
                // ���� ��簡 ������ ����Ʈ�� �߰�
                if (currentDialogue != null)
                {
                    currentDialogue.context = contextList.ToArray();
                    dialogueList.Add(currentDialogue);
                }

                // ���ο� Dialogue ����
                currentDialogue = new Dialogue();
                currentDialogue.name = row[1]; // ĳ���� �̸� ����
                contextList = new List<string>(); // ��� ����Ʈ �ʱ�ȭ

                // ��� �κ��� �߰�
                if (row.Length > 2)
                {
                    string combinedContext = Regex.Replace(row[2].Trim(), "^\\s*\"|\"\\s*$", "");
                    contextList.Add(combinedContext);
                }
            }
            else if (currentDialogue != null && row.Length > 2) // ID�� ������ ���� ��翡 �̾ �߰�
            {
                string combinedContext = Regex.Replace(row[2].Trim(), "^\\s*\"|\"\\s*$", "");
                contextList.Add(combinedContext);
            }
        }

        // ������ ��� �߰�
        if (currentDialogue != null)
        {
            currentDialogue.context = contextList.ToArray();
            dialogueList.Add(currentDialogue);
        }

        return dialogueList.ToArray();
    }

}
