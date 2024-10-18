using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Summon
{


    private void Awake()
    {
        summonInitialize();

    }

    public override void summonInitialize()
    {
        summonName = "Fox"; //이름 Fox
        maxHP = 200; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 15; //일반공격
        summonRank = SummonRank.Low; // 하급 소환수

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Upgrade, 0.2, 3, 1) };//공격력 강화, 20% 상승, 쿨타임 2턴
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
