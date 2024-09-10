using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : Character
{
    public List<Plate> Playerplates; // �÷��̾ ����� �÷���Ʈ ���
    public int Mana = 10; // �÷��̾�� ������ ������ ����
    public TurnController turnController; // TurnController ����

    //�� ���۽� ������ Ȯ����
    public override void startTurn()
    {
        base.startTurn();
        Debug.Log($"{gameObject.name} �� ����: {Mana}");
        if (Mana > 0)
        {
            takeSummon();
        }
        else
        {
            takeAction();
            Debug.Log("������ �����Ͽ� ��ȯ�� ��ŵ�մϴ�.");
        }
    }


    //�÷��̾��� Ȱ�� ���� �����ϸ��
    public override void takeAction()
    {
        Debug.Log("�÷��̾� takeAction ����");
    }

    public override void takeSummon()
    {
        Debug.Log("��ȯ�� ��ȯ");
    }


    //�÷��̾�� ��ư Ŭ���� ���ؼ��� �����Ḧ ��Ų��.
    public void PlayerTurnOverBtn() //��ư�� ���� �޼ҵ�
    {
        // �÷��̾� ���� ���� �� ���� ����
        if (turnController.currentTurn == TurnController.Turn.PlayerTurn)
        {
            EndTurn();
        }
        else
        {
            Debug.Log("�÷��̾� ���� �ƴմϴ�.");
        }
    }

    public override void EndTurn()
    {
        base.EndTurn(); //�׳� �� ���� ����׸� ����.

        turnController.EndTurn();
    }
}
