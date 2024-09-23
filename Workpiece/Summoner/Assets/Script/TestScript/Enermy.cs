using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    public List<Plate> enermyPlates; // ���� ����� �÷���Ʈ ���

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
        base.takeAction(); // �� ���� ó��
        EndTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();
        // �� ���� �� TurnController���� �÷��̾��� ������ �Ѿ�� ��
        TurnController turnController = FindObjectOfType<TurnController>();
        turnController.EndTurn();
    }
}
