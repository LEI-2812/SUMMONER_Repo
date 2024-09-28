using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSV_FileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); //dialogue리스트
        TextAsset csvData = Resources.Load<TextAsset>(CSV_FileName); //csv파일 가져옴

        string[] data = csvData.text.Split(new char[] {'\n'}); //Enter 즉 행마다 읽기
        Debug.Log(data.Length);
        //csv파일의 전체 대사를 읽기 시작
        for(int i=1; i < data.Length;) //두번째줄부터 읽도록 1부터시작
        {
            string[] row = data[i].Split(new char[]{','}); //쉼표단위로 한 행을 끊어 읽도록

            Dialogue dialogue = new Dialogue(); //대사 리스트 생성

            dialogue.name = row[1]; //0은 ID, 1은 이름, 2는 대사
            Debug.Log(row[1]);
            List<string> contextList = new List<string>(); //대사를 넣을 리스트

            // 한 캐릭터의 대사를 계속 읽도록 하기 위한 반복문
            do
            {
                // 대사 내용이 있는지 확인하고 추가
                //이거 안하면 대사가 비어있는데 강제로 넣어버리면서 에러걸림
                if (row.Length > 2)
                {
                    contextList.Add(row[2]); // 대사들을 리스트에 추가
                    Debug.Log(row[2]);
                }

                // 다음 줄이 있는지 확인
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' }); // 다음 줄을 쉼표 단위로 읽어옴
                }
                else
                {
                    break; // 더 이상 읽을 데이터가 없으면 반복 종료
                }

            } while (row[0].ToString() == ""); // 다음 줄에 ID가 빈칸이 아닐 때까지 반복

            //Dialogue에 캐릭1에 대사 여러개를 넣게 한다.
            dialogue.context = contextList.ToArray(); //Dialogue

            dialogueList.Add(dialogue);

        }


        return dialogueList.ToArray();
    }

}
