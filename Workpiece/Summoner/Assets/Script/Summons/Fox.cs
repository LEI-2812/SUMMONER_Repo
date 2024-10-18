using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Summon
{


    private void Awake()
    {
        summonInitialize();

    }

    public override void summonInitialize()
    {
        summonName = "Fox"; //�̸� Fox
        maxHP = 200; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 15; //�Ϲݰ���
        summonRank = SummonRank.Low; // �ϱ� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Upgrade, 0.2, 3, 1) };//���ݷ� ��ȭ, 20% ���, ��Ÿ�� 2��
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
