using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSV_FileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // Dialogue 리스트
        TextAsset csvData = Resources.Load<TextAsset>(CSV_FileName); // CSV 파일 가져옴

        string[] data = csvData.text.Split(new char[] { '\n' }); // 행마다 읽기
        Debug.Log(data.Length);

        // CSV 파일의 전체 대사를 읽기 시작
        for (int i = 1; i < data.Length;) // 두 번째 줄부터 읽도록 1부터 시작
        {
            string[] row = ParseCSVLine(data[i]); // 행을 읽고 파싱

            Dialogue dialogue = new Dialogue(); // 대사 리스트 생성

            // 캐릭터 이름 처리: 각 줄마다 처리
            dialogue.name = row[1]; // 0은 ID, 1은 이름, 2는 대사

            List<string> contextList = new List<string>(); // 대사를 넣을 리스트

            // 한 캐릭터의 대사를 계속 읽도록 하기 위한 반복문
            do
            {
                // 대사 내용이 있는지 확인하고 추가
                if (row.Length > 2)
                {
                    string dialogueText = row[2].Trim(' ', '"'); // 앞뒤 공백과 따옴표 제거
                    contextList.Add(dialogueText); // 대사들을 리스트에 추가
                    Debug.Log(dialogueText);
                }

                // 다음 줄이 있는지 확인
                if (++i < data.Length)
                {
                    row = ParseCSVLine(data[i]); // 다음 줄을 파싱
                }
                else
                {
                    break; // 더 이상 읽을 데이터가 없으면 반복 종료
                }

            } while (string.IsNullOrEmpty(row[0])); // 다음 줄에 ID가 빈칸이 아닐 때까지 반복

            // Dialogue에 캐릭터의 대사 여러 개를 넣게 한다.
            dialogue.context = contextList.ToArray(); // Dialogue

            dialogueList.Add(dialogue);
        }

        return dialogueList.ToArray();
    }

    // CSV 파일의 한 줄을 파싱하는 함수, 쉼표와 따옴표를 처리
    private string[] ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"') // 따옴표 발견
            {
                inQuotes = !inQuotes; // 따옴표 안/밖 전환
                continue; // 따옴표 자체는 저장하지 않음
            }

            if (c == ',' && !inQuotes) // 쉼표 발견 (따옴표 밖에 있는 경우만 구분자로 사용)
            {
                fields.Add(currentField.Trim());
                currentField = ""; // 새로운 필드 시작
            }
            else
            {
                currentField += c; // 현재 필드에 문자 추가
            }
        }

        // 마지막 필드 추가
        fields.Add(currentField.Trim());

        return fields.ToArray();
    }
}
