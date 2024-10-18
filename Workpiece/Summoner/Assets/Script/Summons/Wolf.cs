using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Wolf";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 50; //�Ϲݰ���
        summonRank = SummonRank.High; // �߱� ��ȯ��

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //��������
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.None, 25, 2) };//��ü����, 25������, ��Ÿ��2��
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
