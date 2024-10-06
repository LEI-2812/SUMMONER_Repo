using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("ĳ�����̸�")]
    public string name;

    [Tooltip("��� ����")]
    public string[] context;

    [Tooltip("�̺�Ʈ ��ȣ")]
    public string number;

    [Tooltip("��ŵ����")]
    public string[] skipnum;
}

[System.Serializable]
public class DialogueEvent
{
    //�̺�Ʈ �̸�
    [Header("�̺�Ʈ �̸�")]
    public string name; //����� ��.

    //csv��Ʈ���� ���ٱ��� ������ �д� vectoer
    [Header("csv���� ������, ���� �Է�")]
    public Vector2 line;
    //Dialogue�� �ֱ�
    public Dialogue[] dialogues;
}