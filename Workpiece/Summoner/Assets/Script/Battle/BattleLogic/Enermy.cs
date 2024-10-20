using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    private List<Plate> enermyPlates; // ���� ����� �÷���Ʈ ���

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private TurnController turnController;
    private EnermyAttackController enermyAttackController;
    private EnermyAlgorithm enermyAlgorithm;

    private void Awake()
    {
        enermyAttackController = GetComponent<EnermyAttackController>();
        enermyAlgorithm = GetComponent<EnermyAlgorithm>();
    }

    public override void startTurn()
    {
        base.startTurn();
        // ���� �ൿ�� �ڵ����� ������ (��: �÷��̾ ����)
        takeAction();
    }

    public override void takeAction() //���⿡ AI���� �ۼ�
    {
        //�÷��̾��� �������� ����Ʈ�� ��������
        List<AttackPrediction> playerAttackPredictionsList = enermyAlgorithm.getPlayerAttackPredictionsList();

        Debug.Log("����Ʈ�� �����ͼ� �� ��������");
        //���� ���� ����
        enermyAttackController.EnermyAttackStart(playerAttackPredictionsList);



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
