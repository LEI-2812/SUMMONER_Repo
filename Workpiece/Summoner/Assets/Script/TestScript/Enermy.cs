using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enermy : Character
{
    public List<Plate> Enermyplates; // ���� ����� �÷���Ʈ ���

    private void Start()
    {
        InitializeSummons(); //��ȯ�� ��ȯ
    }

    private void InitializeSummons() //���� ���۽� ��ȯ���� �̸� �����ǵ���
    {
        Debug.Log("�� ��ȯ�� �̸� ��ȯ");
    }
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
