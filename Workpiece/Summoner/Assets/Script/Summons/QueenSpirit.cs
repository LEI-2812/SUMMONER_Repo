using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenSpirit : Summon
{

    protected override void Awake()
    {
        base.Awake();

        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        FallbackStatusSet("QueenSpirit", SummonRank.Special, SummonType.QueenSpirit, 400, 100, 140);

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategiesSet(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.None, 70, 0), //전체공격 데미지 70
            new AttackAllEnemiesStrategy(StatusType.Heal, 0.2, 3), //아군 전체 20% 회복 쿨타임 3턴
            new TargetedAttackStrategy(StatusType.Stun,0,3,1)); //대상에게 혼란, 쿨타임 3턴, 지속시간 1턴
    }

}
