using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    [Header("�� �÷���Ʈ")]
    [SerializeField] private List<Plate> enermyPlates; // ���� ����� �÷���Ʈ ���

    [Header("�� ��Ʈ�ѷ�")]
    [SerializeField] private TurnController turnController;

    public override void startTurn()
    {
        base.startTurn();
        // ���� �ൿ�� �ڵ����� ������ (��: �÷��̾ ����)
        takeAction();
    }

    public override void takeAction() //���⿡ AI���� �ۼ�
    {
        // ���� �ൿ�� ���� (������ ���� �ൿ)
        Debug.Log($"{gameObject.name} ����!");
        
        EndTurn();
    }

    public override void EndTurn()
    {
        Debug.Log("�� �� ����");
        turnController.EndTurn();
    }
}
