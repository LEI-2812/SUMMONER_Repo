using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{
    private void Awake()
    {
        summonInitialize(5);
    }

    public void summonInitialize(int n)
    {
        summonName = "Eagle";
        maxHP = 400 * n;
        nowHP = maxHP;
        attackPower = 45 * n; //일반공격
        summonRank = SummonRank.High; // 상급 소환수
        summonType = SummonType.Eagle;
        heavyAttakPower = 30 * n;
        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1);
        // 특수 공격: 타겟 지정 공격
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.None, heavyAttakPower, 2) }; //저격 30데미지, 쿨타임 2턴
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
