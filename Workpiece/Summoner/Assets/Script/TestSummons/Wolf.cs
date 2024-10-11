using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Summon
{
    private void Awake()
    {
        summonName = "Wolf";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 50; //�Ϲݰ���
        summonRank = SummonRank.High; // �߱� ��ȯ��

        AttackStrategy = new ClosestEnemyAttackStrategy(); //��������
        SpecialAttackStrategy = new AttackAllEnemiesStrategy();//��ü����
    }

    private void Start()
    {
        nowHP = 80; //�׽�Ʈ��
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
