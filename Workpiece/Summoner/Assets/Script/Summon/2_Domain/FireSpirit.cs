using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: FireSpirit의 책임을 정의한다.
public class FireSpirit : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("FireSpirit", SummonRank.Normal, 350, 60, 130);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.Upgrade, 0.1, 2, 1));
    }

}
