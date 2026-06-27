using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: KingSlime의 책임을 정의한다.
public class KingSlime : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("KingSlime", SummonRank.Special, 250, 50, 65);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.None, 35, 1),
            new AttackData(new TargetedAttackStrategy(), StatusType.Shield, 80, 2));
    }

}
