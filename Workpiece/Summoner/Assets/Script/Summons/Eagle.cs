using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{
    private void Awake()
    {
        summonName = "Eagle";
        maxHP = 400;
        nowHP = maxHP;
        attackPower = 45; //�Ϲݰ���
        SpecialPower = 30; //Ư������
        summonRank = SummonRank.High; // ��� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        AttackStrategy = new ClosestEnemyAttackStrategy();
        // Ư�� ����: Ÿ�� ���� ����
        SpecialAttackStrategy = new TargetedAttackStrategy();
    }

    private void Start()
    {
      
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