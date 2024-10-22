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

    public void summonInitialize(int n)
    {
        summonName = "Rabbit";
        maxHP = 250 * n;
        nowHP = maxHP;
        attackPower = 20*n; //�Ϲݰ���
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
