using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rabbit : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Rabbit";
        maxHP = 250;
        nowHP = maxHP;
        attackPower = 20; //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수

        AttackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower); //근접공격
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Heal, 0) };
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
