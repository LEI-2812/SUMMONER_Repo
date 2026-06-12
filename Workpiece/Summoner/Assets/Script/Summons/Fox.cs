using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Summon
{

    public override void SummonInitialize()
    {
        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Fox", SummonRank.Low, SummonType.Fox, 250, 35, 0);
        ApplayMultiple(GetStatMultiplier());

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new TargetedAttackStrategy(StatusType.Upgrade, 0.3, 3, 1));//공격력 강화, 20% 상승, 쿨타임 3턴 //지속시간 1턴

    }

}
