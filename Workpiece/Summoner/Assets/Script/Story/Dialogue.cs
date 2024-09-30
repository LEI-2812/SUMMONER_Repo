using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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