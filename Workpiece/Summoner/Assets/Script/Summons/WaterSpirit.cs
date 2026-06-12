using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpirit : Summon
{

    protected override void Awake()
    {
        base.Awake();

        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("WaterSpirit", SummonRank.Normal, SummonType.WaterSpirit, 350, 70, 120);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new TargetedAttackStrategy(StatusType.Shield, 80, 2));//쉴드
    }

}
