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
        attackPower = 50; //일반공격
        summonRank = SummonRank.High; // 중급 소환수

        AttackStrategy = new ClosestEnemyAttackStrategy(); //근접공격
        SpecialAttackStrategy = new AttackAllEnemiesStrategy();//전체공격
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
