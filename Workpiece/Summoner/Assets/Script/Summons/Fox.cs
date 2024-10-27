using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Summon
{


    private void Awake()
    {

    }

    public override void summonInitialize(double n)
    {
        n = 5;
        summonName = "Fox"; //�̸� Fox
        maxHP = (int)(200 * n); //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = (int)(15 * n); //�Ϲݰ���
        summonRank = SummonRank.Low; // �ϱ� ��ȯ��
        summonType = SummonType.Fox;
        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Upgrade, 0.2, 3, 1) };//���ݷ� ��ȭ, 20% ���, ��Ÿ�� 3�� //���ӽð� 1��
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
