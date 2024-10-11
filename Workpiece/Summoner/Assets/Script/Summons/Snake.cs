using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Awake()
    {
        summonName = "Snake";
        maxHP = 200;
        nowHP = maxHP;
        attackPower = 30; //일반공격
        specialPower = 25;
        summonRank = SummonRank.Medium; // 중급 소환수

        AttackStrategy = new ClosestEnemyAttackStrategy(); //근접 공격
        SpecialAttackStrategy = new StatusAttackStrategy(StatusType.Poison, 3);//상태이상 (중독)공격
    }

    public override int SpecialAttackCooldown() //중독은 쿨타임 3턴
    {
        return 3;
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
