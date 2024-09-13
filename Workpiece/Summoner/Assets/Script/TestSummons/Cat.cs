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
        attackPower = 15; //일반공격
        SpecialPower = 20; //특수공격
        summonRank = SummonRank.Low; // 중급 소환수
    }

    private void Start()
    {
        nowHP = 80; //테스트용
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
