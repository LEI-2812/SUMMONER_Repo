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
        summonRank = SummonRank.Low; // �߱� ��ȯ��
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
