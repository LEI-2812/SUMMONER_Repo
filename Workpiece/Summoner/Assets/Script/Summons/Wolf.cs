using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wolf : Summon
{
    public override void summonInitialize()
    {
        summonName = "Wolf";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 50; //�Ϲݰ���
        heavyAttakPower = 25;
        summonRank = SummonRank.High; // �߱� ��ȯ��
        summonType = SummonType.Wolf;
        ApplayMultiple(multiple);

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //��������
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.None, heavyAttakPower, 2) };//��ü����, 25������, ��Ÿ��2��
    }

    public override void ApplayMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //�Ϲݰ���
        heavyAttakPower = (int)(heavyAttakPower * multiple);
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
