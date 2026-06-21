using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 불 정령 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class FireSpirit : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("FireSpirit", SummonRank.Normal, 350, 60, 130);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.Upgrade, 0.1, 3, 1)); //공격력의 0.1만큼 강화, 쿨타임 1턴, 지속시간 1턴
    }

}
