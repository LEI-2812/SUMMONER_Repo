using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Summon
{
    private void Awake()
    {
        summonName = "Wolf";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 50; //�Ϲݰ���
        specialPower = 25;
        summonRank = SummonRank.High; // �߱� ��ȯ��

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1); //��������
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.None, specialPower,2) };//��ü����
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
