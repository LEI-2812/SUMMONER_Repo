using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// 역할: Dialogue의 책임을 정의한다.
public class Dialogue
{
    [Tooltip("Name")]
    public string name;

    [Tooltip("설명")]
    public string[] context;

    [Tooltip("설명")]
    public string number;

    [Tooltip("설명")]
    public string[] skipnum;
}

[System.Serializable]
// 역할: DialogueEvent의 책임을 정의한다.
public class DialogueEvent
{
    [Header("참조")]
    public string name;

    [Header("참조")]
    public Vector2 line;
    public Dialogue[] dialogues;
}
