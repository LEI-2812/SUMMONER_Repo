using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Cat";
        maxHP = 100;
        nowHP = maxHP;
        attackPower = 15; //�Ϲݰ���
        SpecialPower = 20; //Ư������
        summonRank = SummonRank.Low; // �ϱ� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        AttackStrategy = new ClosestEnemyAttackStrategy(StatusType.None);
        // Ư�� ����: ��ü �� ����
        specialAttackStrategy = new ClosestEnemyAttackStrategy(StatusType.None);
    }

    public override void attack()
    {
        
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
