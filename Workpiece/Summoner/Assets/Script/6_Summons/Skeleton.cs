using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 스켈레톤 적 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class Skeleton : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Skeleton", SummonRank.Normal, SummonType.Skeleton, 650, 150, 170);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0), //근접공격
            new TargetedAttackStrategy(StatusType.None, 160,0)); //타겟공격 160데미지
    }

}
