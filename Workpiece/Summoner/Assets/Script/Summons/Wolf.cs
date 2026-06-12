using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wolf : Summon
{
    public override void SummonInitialize()
    {
        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Wolf", SummonRank.High, SummonType.Wolf, 350, 50, 30);
        ApplayMultiple(GetStatMultiplier());

        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1), //근접공격
            new AttackAllEnemiesStrategy(StatusType.None, GetHeavyAttackPower(), 2));//전체공격, 25데미지, 쿨타임2턴
    }

}
