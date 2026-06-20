using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 상급 악마 적 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class HighDevil : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("HighDevil", SummonRank.Special, SummonType.HighDevil, 1000, 200, 250);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.None, 140,0), //전체공격 140
            new TargetedAttackStrategy(StatusType.None,230,0)); //타겟공격 230

    }

}
