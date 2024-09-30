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
        Debug.Log(data.Length);

        // CSV ������ ��ü ��縦 �б� ����
        for (int i = 1; i < data.Length;) // �� ��° �ٺ��� �е��� 1���� ����
        {
            string[] row = ParseCSVLine(data[i]); // ���� �а� �Ľ�

            Dialogue dialogue = new Dialogue(); // ��� ����Ʈ ����

            // ĳ���� �̸� ó��: �� �ٸ��� ó��
            dialogue.name = row[1]; // 0�� ID, 1�� �̸�, 2�� ���

            List<string> contextList = new List<string>(); // ��縦 ���� ����Ʈ

            // �� ĳ������ ��縦 ��� �е��� �ϱ� ���� �ݺ���
            do
            {
                // ��� ������ �ִ��� Ȯ���ϰ� �߰�
                if (row.Length > 2)
                {
                    string dialogueText = row[2].Trim(' ', '"'); // �յ� ����� ����ǥ ����
                    contextList.Add(dialogueText); // ������ ����Ʈ�� �߰�
                    Debug.Log(dialogueText);
                }

                // ���� ���� �ִ��� Ȯ��
                if (++i < data.Length)
                {
                    row = ParseCSVLine(data[i]); // ���� ���� �Ľ�
                }
                else
                {
                    break; // �� �̻� ���� �����Ͱ� ������ �ݺ� ����
                }

            } while (string.IsNullOrEmpty(row[0])); // ���� �ٿ� ID�� ��ĭ�� �ƴ� ������ �ݺ�

            // Dialogue�� ĳ������ ��� ���� ���� �ְ� �Ѵ�.
            dialogue.context = contextList.ToArray(); // Dialogue

            dialogueList.Add(dialogue);
        }

        return dialogueList.ToArray();
    }

    // CSV ������ �� ���� �Ľ��ϴ� �Լ�, ��ǥ�� ����ǥ�� ó��
    private string[] ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"') // ����ǥ �߰�
            {
                inQuotes = !inQuotes; // ����ǥ ��/�� ��ȯ
                continue; // ����ǥ ��ü�� �������� ����
            }

            if (c == ',' && !inQuotes) // ��ǥ �߰� (����ǥ �ۿ� �ִ� ��츸 �����ڷ� ���)
            {
                fields.Add(currentField.Trim());
                currentField = ""; // ���ο� �ʵ� ����
            }
            else
            {
                currentField += c; // ���� �ʵ忡 ���� �߰�
            }
        }

        // ������ �ʵ� �߰�
        fields.Add(currentField.Trim());

        return fields.ToArray();
    }
}
