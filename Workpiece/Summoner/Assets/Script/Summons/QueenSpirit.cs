using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenSpirit : Summon
{

    private void Awake()
    {
        summonName = "QueenSpirit"; //이름 슬라임
        maxHP = 400; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 100; //일반공격
        heavyAttakPower = 140;
        summonRank = SummonRank.Special; // 특급 소환수

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.None, 70, 0), //전체공격 데미지 70
            new AttackAllEnemiesStrategy(StatusType.Heal, 0.2, 3), //아군 전체 20% 회복 쿨타임 3턴
            new TargetedAttackStrategy(StatusType.Stun,0,3,1) //대상에게 혼란, 쿨타임 3턴, 지속시간 1턴
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
