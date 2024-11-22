using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{

    public override void summonInitialize()
    {
        summonName = "Eagle";
        maxHP = 350;
        nowHP = maxHP;
        attackPower = 45; //일반공격
        summonRank = SummonRank.High; // 상급 소환수
        summonType = SummonType.Eagle;
        heavyAttakPower = 30;

        ApplayMultiple(multiple);

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1);
        // 특수 공격: 타겟 지정 공격
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.None, heavyAttakPower, 2) }; //저격 30데미지, 쿨타임 2턴

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
