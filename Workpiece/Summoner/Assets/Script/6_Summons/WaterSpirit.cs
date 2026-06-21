using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 물 정령 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class WaterSpirit : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("WaterSpirit", SummonRank.Normal, 350, 70, 120);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new TargetedAttackStrategy(StatusType.Shield, 80, 2));//쉴드
    }

}
