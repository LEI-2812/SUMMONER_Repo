using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 여우 소환수의 기본 데이터와 특수 공격 동작을 초기화한다.
public class Fox : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Fox", SummonRank.Low, 250, 35, 0);
        ApplayMultiple(GetStatMultiplier());

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new TargetedAttackStrategy(StatusType.Upgrade, 0.3, 3, 1));//공격력 강화, 20% 상승, 쿨타임 3턴 //지속시간 1턴

    }

}
