using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: GrassSpirit의 책임을 정의한다.
public class GrassSpirit : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("GrassSpirit", SummonRank.Normal, 350, 80, 110);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.Heal, 0.1, 3));
    }

}
