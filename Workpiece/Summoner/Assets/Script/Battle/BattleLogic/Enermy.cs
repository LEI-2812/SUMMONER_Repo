using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    [Header("�� �÷���Ʈ")]
    [SerializeField] private List<Plate> enermyPlates; // ���� ����� �÷���Ʈ ���

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private EnermyAttackController enermyAttackController;

    public override void startTurn()
    {
        base.startTurn();
        // ���� �ൿ�� �ڵ����� ������ (��: �÷��̾ ����)
        takeAction();
    }

    public override void takeAction() //���⿡ AI���� �ۼ�
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (enermyPlates[i].getSummon() != null && !enermyPlates[i].getSummon().IsCursed())
            {
                enermyAttackController.EnermyAttackStart(enermyPlates[i].getSummon());
            }
        }



        EndTurn();
    }

    public override void EndTurn()
    {
        Debug.Log("�� �� ����");
        turnController.EndTurn();
    }
}
