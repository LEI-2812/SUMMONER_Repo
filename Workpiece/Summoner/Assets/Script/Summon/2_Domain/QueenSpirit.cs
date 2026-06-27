using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: QueenSpirit의 책임을 정의한다.
public class QueenSpirit : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("QueenSpirit", SummonRank.Special, 400, 100, 140);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.None, 70, 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.Heal, 0.2, 3),
            new AttackData(new TargetedAttackStrategy(), StatusType.Stun, 0, 3, 1));
    }

}
