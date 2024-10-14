using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Eagle";
        maxHP = 400;
        nowHP = maxHP;
        attackPower = 45; //일반공격
        specialPower = 30; //특수공격
        summonRank = SummonRank.High; // 상급 소환수

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1);
        // 특수 공격: 타겟 지정 공격
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.None, specialPower,2) };
    }



    public override void die()
    {
        base.die();
    }

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }

}
