using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: Slime의 책임을 정의한다.
public class Slime : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Slime", SummonRank.Normal, 200, 25, 40);
        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 1),
            new AttackData(new TargetedAttackStrategy(), StatusType.Shield, 50, 2));
    }

}
