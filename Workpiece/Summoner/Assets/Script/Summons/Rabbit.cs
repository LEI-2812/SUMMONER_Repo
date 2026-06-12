using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rabbit : Summon
{
    public override void SummonInitialize()
    {
        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Rabbit", SummonRank.Medium, SummonType.Rabbit, 300, 37, 0);
        ApplayMultiple(GetStatMultiplier());

        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(),1), //근접공격
            new TargetedAttackStrategy(StatusType.Heal, 0.3, 3));

    }

}
