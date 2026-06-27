using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: WaterSpirit의 책임을 정의한다.
public class WaterSpirit : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("WaterSpirit", SummonRank.Normal, 350, 70, 120);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new TargetedAttackStrategy(), StatusType.Shield, 150, 2));
    }

}
