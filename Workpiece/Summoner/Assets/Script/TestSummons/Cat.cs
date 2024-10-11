using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Summon
{
    private void Awake()
    {
        summonName = "Cat";
        maxHP = 100;
        nowHP = maxHP;
        attackPower = 15; //�Ϲݰ���
        SpecialPower = 20; //Ư������
        summonRank = SummonRank.Low; // �ϱ� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        AttackStrategy = new ClosestEnemyAttackStrategy();
        // Ư�� ����: ��ü �� ����
        SpecialAttackStrategy = new AttackAllEnemiesStrategy();
    }

    private void Start()
    {
        nowHP = 80; //�׽�Ʈ��
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
