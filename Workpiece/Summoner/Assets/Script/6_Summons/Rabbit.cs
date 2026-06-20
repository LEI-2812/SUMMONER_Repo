using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: 토끼 소환수의 기본 데이터와 특수 공격 동작을 초기화한다.
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
