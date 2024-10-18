using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Summon
{
    private void Awake()
    {
        summonName = "Slime"; //이름 슬라임
        maxHP = 200; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 25; //일반공격
        summonRank = SummonRank.Normal; // 일반 소환수
        heavyAttakPower = 40; //강공격력이 없음
        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Shield, 50, 2) };
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
