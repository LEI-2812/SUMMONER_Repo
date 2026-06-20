using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 스토리 대화 데이터를 불러오고 필요한 곳에 제공한다.
public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance; //싱글톤으로 어디서든 하나의 오브젝트로 관리하기 위함

    [SerializeField]
    [Header("CSV 파일 이름 .csv 없이")]
    string csv_FileName; //csv파일이름

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>(); //딕셔너리로 관리

    public static bool isFinish;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialgues = theParser.Parse(csv_FileName);
            for(int i=0; i<dialgues.Length; i++)
            {
                dialogueDic.Add(i+1, dialgues[i]); //각대사들을 키값 1부터 넣으며 딕셔너리에 넣는다. 첫대사의 키값은 1이 되는거임
            }
            isFinish = true;
        }
    }


    public Dialogue[] GetDialogue(int startNum, int endNum) //대사 꺼내오기
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for(int i=0; i <= endNum - startNum; i++) //한 캐릭터의 대사갯수만큼 반복
        {
            dialogueList.Add(dialogueDic[startNum + i]); //위에서 dialogueDic.Add(i+1, dialgues[i]);이 i+1이기때문
        }

        return dialogueList.ToArray(); //배열형태로 바꿔서 반환
    }
}
