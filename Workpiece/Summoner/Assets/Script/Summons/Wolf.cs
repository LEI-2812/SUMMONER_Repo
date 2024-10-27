using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Summon
{
    private void Awake()
    {
    }

    public override void summonInitialize(double n)
    {
        n = 5;
        summonName = "Wolf";
        maxHP = (int)(300 *n);
        nowHP = maxHP;
        attackPower = (int)(50 * n); //�Ϲݰ���
        heavyAttakPower = (int)(25 * n);
        summonRank = SummonRank.High; // �߱� ��ȯ��
        summonType = SummonType.Wolf;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //��������
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.None, heavyAttakPower, 2) };//��ü����, 25������, ��Ÿ��2��
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
