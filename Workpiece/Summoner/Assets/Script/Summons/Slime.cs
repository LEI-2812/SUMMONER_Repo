using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Summon
{
    private void Awake()
    {
        summonName = "Slime"; //�̸� ������
        maxHP = 200; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 25; //�Ϲݰ���
        summonRank = SummonRank.Normal; // �Ϲ� ��ȯ��
        heavyAttakPower = 40; //�����ݷ��� ����
        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Shield, 50, 2) };
    }


    public override void die()
    {
        base.die();
    }

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }

}
