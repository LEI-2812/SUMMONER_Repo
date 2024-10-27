using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Summon
{
    private void Awake()
    {
    }

    public override void summonInitialize(double n)
    {
        n = 5;
        summonName = "Wolf";
        maxHP = (int)(300 *n);
        nowHP = maxHP;
        attackPower = (int)(50 * n); //일반공격
        heavyAttakPower = (int)(25 * n);
        summonRank = SummonRank.High; // 중급 소환수
        summonType = SummonType.Wolf;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //근접공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.None, heavyAttakPower, 2) };//전체공격, 25데미지, 쿨타임2턴
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
