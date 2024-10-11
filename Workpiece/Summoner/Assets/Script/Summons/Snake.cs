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

        AttackStrategy = new ClosestEnemyAttackStrategy(); //���� ����
        SpecialAttackStrategy = new StatusAttackStrategy(StatusType.Poison, 3);//�����̻� (�ߵ�)����
    }

    public override int SpecialAttackCooldown() //�ߵ��� ��Ÿ�� 3��
    {
        return 3;
    }

    public override void attack()
    {
        base.attack();
    }

    public override void die()
    {
        base.die();
    }

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }

    public override void takeSkill()
    {
        base.takeSkill();
    }
}
