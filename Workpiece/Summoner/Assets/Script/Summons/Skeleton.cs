using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Summon
{

    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Skeleton"; //이름 슬라임
        maxHP = 650; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 150; //일반공격
        heavyAttakPower = 170;
        summonRank = SummonRank.Normal; // 특급 소환수

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] {
            new TargetedAttackStrategy(StatusType.None, 160,0)
        };
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
