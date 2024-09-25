using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Summon
{
    private void Awake()
    {
        summonName = "Rabbit";
        maxHP = 250;
        nowHP = maxHP;
        attackPower = 20; //�Ϲݰ���
        summonRank = SummonRank.Medium; // �߱� ��ȯ��
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

    public override void takeDamage(int damage)
    {
        base.takeDamage(damage);
    }

    public override void takeSkill()
    {
        base.takeSkill();
    }
}
