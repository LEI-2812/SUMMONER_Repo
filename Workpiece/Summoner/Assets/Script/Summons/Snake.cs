using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Start()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Snake";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 40; //�Ϲݰ���
        summonRank = SummonRank.Medium; // �߱� ��ȯ��
        summonType = SummonType.Snake;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //���� ����
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0.2, 3, 3) };//�ߵ�, ü�¿�20% ��Ÿ��3�� ���ӽð�3��

        ApplyMultiple(multiple);
    }

    public void ApplyMultiple(double multiple)
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
