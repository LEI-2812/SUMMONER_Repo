using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rabbit : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Rabbit";
        maxHP = 250;
        nowHP = maxHP;
        attackPower = 20; //�Ϲݰ���
        summonRank = SummonRank.Medium; // �߱� ��ȯ��

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1); //��������
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Heal, 0, 3) };
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
