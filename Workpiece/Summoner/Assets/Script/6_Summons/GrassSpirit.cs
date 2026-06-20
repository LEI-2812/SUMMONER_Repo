using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 풀 정령 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class GrassSpirit : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("GrassSpirit", SummonRank.Normal, SummonType.GrassSpirit, 350, 80, 110);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.Heal, 0.1, 3)); //최대 체력의 0.1만큼 회복, 쿨타임 3턴
    }

}
