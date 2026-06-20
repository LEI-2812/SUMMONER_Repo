using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// 역할: 대화 한 줄의 화자, 내용, 이미지 정보를 담는다.
public class Dialogue
{
    [Tooltip("캐릭터이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] context;

    [Tooltip("이벤트 번호")]
    public string number;

    [Tooltip("스킵라인")]
    public string[] skipnum;
}

[System.Serializable]
// 역할: 대화 진행 중 실행할 이벤트 정보를 담는다.
public class DialogueEvent
{
    //이벤트 이름
    [Header("이벤트 이름")]
    public string name; //안적어도 됨.

    //csv시트에서 몇줄까지 읽을지 읽는 vectoer
    [Header("csv파일 시작행, 끝행 입력")]
    public Vector2 line;
    //Dialogue들 넣기
    public Dialogue[] dialogues;
}