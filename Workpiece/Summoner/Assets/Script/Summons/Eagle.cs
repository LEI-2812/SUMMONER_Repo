using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{
    private void Awake()
    {
        summonInitialize(5);
    }

    public void summonInitialize(int n)
    {
        summonName = "Eagle";
        maxHP = 400 * n;
        nowHP = maxHP;
        attackPower = 45 * n; //�Ϲݰ���
        summonRank = SummonRank.High; // ��� ��ȯ��
        summonType = SummonType.Eagle;
        heavyAttakPower = 30 * n;
        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1);
        // Ư�� ����: Ÿ�� ���� ����
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.None, heavyAttakPower, 2) }; //���� 30������, ��Ÿ�� 2��
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
