using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 킹슬라임 적 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class KingSlime : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("KingSlime", SummonRank.Special, SummonType.KingSlime, 250, 50, 65);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.None, 35, 1),//전체공격
            new TargetedAttackStrategy(StatusType.Shield, 80, 2));//쉴드 
    }

}
