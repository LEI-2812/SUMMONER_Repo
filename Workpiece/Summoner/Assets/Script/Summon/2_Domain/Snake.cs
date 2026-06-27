using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: Snake의 책임을 정의한다.
public class Snake : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Snake", SummonRank.Medium, 300, 40, 0);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 1), //근접 공격
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.Poison, 0.2, 3, 3));
    }

}
