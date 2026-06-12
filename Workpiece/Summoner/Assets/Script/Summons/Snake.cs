using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    public override void SummonInitialize()
    {
        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Snake", SummonRank.Medium, SummonType.Snake, 300, 40, 0);
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1), //근접 공격
            new AttackAllEnemiesStrategy(StatusType.Poison, 0.1, 3, 2));//중독, 체력에20% 쿨타임3턴 지속시간3턴
        ApplayMultiple(GetStatMultiplier());
    }

}
