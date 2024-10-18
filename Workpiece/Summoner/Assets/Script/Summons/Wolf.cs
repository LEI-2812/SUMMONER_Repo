using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Wolf";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 50; //일반공격
        summonRank = SummonRank.High; // 중급 소환수

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //근접공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.None, 25, 2) };//전체공격, 25데미지, 쿨타임2턴
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
