using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 뱀 소환수의 기본 데이터와 특수 공격 동작을 초기화한다.
public class Snake : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Snake", SummonRank.Medium, 300, 40, 0);
        ApplayMultiple(GetStatMultiplier());

        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1), //근접 공격
            new AttackAllEnemiesStrategy(StatusType.Poison, 0.1, 3, 2));//중독, 체력에20% 쿨타임3턴 지속시간3턴
    }

}
