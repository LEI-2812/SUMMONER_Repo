using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpirit : Summon
{

    private void Awake()
    {
        summonName = "WaterSpirit"; //이름 물정령
        maxHP = 350; //최대체력 350
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 70; //일반공격 70
        heavyAttakPower = 120; //강공격
        summonRank = SummonRank.Normal; // 일반 소환수

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Shield, 80, 2)};//쉴드
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
