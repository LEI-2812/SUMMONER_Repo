using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Awake()
    {
        summonInitialize(5);
    }

    public override void summonInitialize(double n)
    {
        n = 5;
        summonName = "Snake";
        maxHP = (int)(200 * n);
        nowHP = maxHP;
        attackPower = (int)(30 * n); //�Ϲݰ���
        summonRank = SummonRank.Medium; // �߱� ��ȯ��
        summonType = SummonType.Snake;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //���� ����
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0.1, 3, 3) };//�ߵ�, ü�¿�10% ��Ÿ��3�� ���ӽð�3��
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
