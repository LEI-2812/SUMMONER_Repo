using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Summon
{

    protected override void Awake()
    {
        base.Awake();

        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Slime", SummonRank.Normal, SummonType.Slime, 200, 25, 40);
        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1),
            new TargetedAttackStrategy(StatusType.Shield, 50, 2));
    }

}
