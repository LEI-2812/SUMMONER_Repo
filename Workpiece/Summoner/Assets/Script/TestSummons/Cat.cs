using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Summon
{
    private void Start()
    {
        summonName = "Cat";
        health = 100;
        attackPower = 15; //�Ϲݰ���
        SpecialPower = 20; //Ư������
        summonRank = SummonRank.Low; // �߱� ��ȯ��
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
