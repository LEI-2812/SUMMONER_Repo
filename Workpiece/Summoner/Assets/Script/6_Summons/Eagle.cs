using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 독수리 소환수의 기본 데이터와 특수 공격 동작을 초기화한다.
public class Eagle : Summon
{

    public override void SummonInitialize()
    {
        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Eagle", SummonRank.High, SummonType.Eagle, 350, 45, 30);

        ApplayMultiple(GetStatMultiplier());

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1),
            new TargetedAttackStrategy(StatusType.None, GetHeavyAttackPower(), 2)); //저격 30데미지, 쿨타임 2턴

    }

}
