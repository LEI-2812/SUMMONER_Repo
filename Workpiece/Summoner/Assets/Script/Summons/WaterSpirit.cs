using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpirit : Summon
{

    private void Awake()
    {
        summonName = "WaterSpirit"; //�̸� ������
        maxHP = 350; //�ִ�ü�� 350
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 70; //�Ϲݰ��� 70
        heavyAttakPower = 120; //������
        summonRank = SummonRank.Normal; // �Ϲ� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Shield, 80, 2)};//����
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
