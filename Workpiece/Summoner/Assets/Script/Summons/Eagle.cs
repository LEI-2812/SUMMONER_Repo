using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Eagle";
        maxHP = 400;
        nowHP = maxHP;
        attackPower = 45; //�Ϲݰ���
        specialPower = 30; //Ư������
        summonRank = SummonRank.High; // ��� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1);
        // Ư�� ����: Ÿ�� ���� ����
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.None, specialPower,2) };
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
