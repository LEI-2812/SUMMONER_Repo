using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpirit : Summon
{

    private void Awake()
    {
        summonName = "FireSpirit"; //이름 슬라임
        maxHP = 350; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 60; //일반공격
        heavyAttakPower = 130;
        summonRank = SummonRank.Normal; // 일반 소환수

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Upgrade, 0.1, 3, 1) }; //공격력의 0.1만큼 강화, 쿨타임 1턴, 지속시간 1턴
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
