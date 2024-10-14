using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Awake()
    {
        summonName = "Snake";
        maxHP = 200;
        nowHP = maxHP;
        attackPower = 30; //�Ϲݰ���
        specialPower = 25;
        summonRank = SummonRank.Medium; // �߱� ��ȯ��

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1); //���� ����
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0,3,3) };//�����̻� (�ߵ�)����
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
