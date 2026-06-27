using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: DarkDragon의 책임을 정의한다.
public class DarkDragon : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("DarkDragon", SummonRank.Boss, 3000, 400, 500);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.None, 370, 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.Burn, 0.2, 5, 2),
            new AttackData(new TargetedAttackStrategy(), StatusType.None, 450, 0),
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.Upgrade, 0.1, 2, 1));
    }

}
