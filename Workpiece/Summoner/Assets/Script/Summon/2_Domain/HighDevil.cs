using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: HighDevil의 책임을 정의한다.
public class HighDevil : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("HighDevil", SummonRank.Special, 1000, 200, 250);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.None, 140, 0),
            new AttackData(new TargetedAttackStrategy(), StatusType.None, 230, 0));

    }

}
