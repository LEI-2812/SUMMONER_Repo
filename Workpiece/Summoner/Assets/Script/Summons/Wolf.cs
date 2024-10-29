using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wolf : Summon
{
    public override void summonInitialize()
    {
        summonName = "Wolf";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 50; //일반공격
        heavyAttakPower = 25;
        summonRank = SummonRank.High; // 중급 소환수
        summonType = SummonType.Wolf;
        ApplayMultiple(multiple);

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //근접공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.None, heavyAttakPower, 2) };//전체공격, 25데미지, 쿨타임2턴
    }

    public override void ApplayMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //일반공격
        heavyAttakPower = (int)(heavyAttakPower * multiple);
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
