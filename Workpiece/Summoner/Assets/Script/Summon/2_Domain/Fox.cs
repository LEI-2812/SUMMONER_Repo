using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: Fox의 책임을 정의한다.
public class Fox : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Fox", SummonRank.Low, 250, 35, 0);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new TargetedAttackStrategy(), StatusType.Upgrade, 0.2, 2, 1));
    }

}
