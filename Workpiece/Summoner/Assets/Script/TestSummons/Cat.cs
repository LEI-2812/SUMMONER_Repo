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
        summonRank = SummonRank.Low; // 하급 소환수

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategy = new ClosestEnemyAttackStrategy();
        // 특수 공격: 전체 적 공격
        SpecialAttackStrategy = new AttackAllEnemiesStrategy();
    }

    private void Start()
    {
        nowHP = 80; //테스트용
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
