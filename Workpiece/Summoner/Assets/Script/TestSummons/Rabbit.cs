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
        attackPower = 20; //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수

        AttackStrategy = new ClosestEnemyAttackStrategy(); //근접공격
        SpecialAttackStrategy = new StatusAttackStrategy();//상태 이상
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

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }

    public override void takeSkill()
    {
        base.takeSkill();
    }
}
