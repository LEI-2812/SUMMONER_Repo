using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 슬라임 적 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class Slime : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Slime", SummonRank.Normal, 200, 25, 40);
        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1),
            new TargetedAttackStrategy(StatusType.Shield, 50, 2));
    }

}
