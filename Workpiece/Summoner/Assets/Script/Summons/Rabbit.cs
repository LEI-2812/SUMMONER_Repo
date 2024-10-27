using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rabbit : Summon
{
    private void Awake()
    {
        summonInitialize(5);
    }

    public override void summonInitialize(double n)
    {
        n = 5;
        summonName = "Rabbit";
        maxHP = (int)(250 * n);
        nowHP = maxHP;
        attackPower = (int)(20 *n); //�Ϲݰ���
        summonRank = SummonRank.Medium; // �߱� ��ȯ��
        summonType = SummonType.Rabbit;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1); //��������
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Heal, 0.3, 3) };
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
