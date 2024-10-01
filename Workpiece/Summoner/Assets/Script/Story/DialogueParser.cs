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
        Dialogue currentDialogue = null; // 현재 대화를 저장할 변수
        List<string> contextList = new List<string>(); // 대사를 넣을 리스트

        for (int i = 1; i < data.Length; i++) // csv 파일 전체를 읽는다
        {
            if (string.IsNullOrWhiteSpace(data[i])) continue; // 빈 줄 건너뜀

            string[] row = data[i].Split(new char[] { ',' }, 3); // 처음 두 부분만 분리하고 나머지는 대사로 합침

            if (!string.IsNullOrEmpty(row[0])) // ID가 있는 경우, 새로운 대사 시작
            {
                // 현재 대사가 있으면 리스트에 추가
                if (currentDialogue != null)
                {
                    currentDialogue.context = contextList.ToArray();
                    dialogueList.Add(currentDialogue);
                }

                // 새로운 Dialogue 생성
                currentDialogue = new Dialogue();
                currentDialogue.name = row[1]; // 캐릭터 이름 설정
                contextList = new List<string>(); // 대사 리스트 초기화

                // 대사 부분을 추가
                if (row.Length > 2)
                {
                    string combinedContext = Regex.Replace(row[2].Trim(), "^\\s*\"|\"\\s*$", "");
                    contextList.Add(combinedContext);
                }
            }
            else if (currentDialogue != null && row.Length > 2) // ID가 없으면 이전 대사에 이어서 추가
            {
                string combinedContext = Regex.Replace(row[2].Trim(), "^\\s*\"|\"\\s*$", "");
                contextList.Add(combinedContext);
            }
        }

        // 마지막 대사 추가
        if (currentDialogue != null)
        {
            currentDialogue.context = contextList.ToArray();
            dialogueList.Add(currentDialogue);
        }

        return dialogueList.ToArray();
    }

}
