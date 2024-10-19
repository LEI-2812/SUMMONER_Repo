using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    private List<Plate> enermyPlates; // ���� ����� �÷���Ʈ ���

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private EnermyAttackController enermyAttackController;

    private void Start()
    {
        enermyPlates = enermyAttackController.getPlateController().getEnermyPlates(); //�� �÷���Ʈ�� �����´�.
    }

    public override void startTurn()
    {
        base.startTurn();
        // ���� �ൿ�� �ڵ����� ������ (��: �÷��̾ ����)
        takeAction();
    }

    public override void takeAction() //���⿡ AI���� �ۼ�
    {
        for (int i = 0; i < enermyPlates.Count; i++) //���� ������� ����
        {
            if (enermyPlates[i].getCurrentSummon() != null && !enermyPlates[i].getCurrentSummon().IsCursed())
            {
                enermyAttackController.EnermyAttackStart(enermyPlates[i].getCurrentSummon());
            }
        }



        EndTurn();
    }

    public override void EndTurn()
    {
        Debug.Log("�� �� ����");
        turnController.EndTurn();
    }


    public EnermyAttackController getEnermyAttackController()
    {
        return enermyAttackController;
    }
}
