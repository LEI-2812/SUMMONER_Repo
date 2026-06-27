using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: Skeleton의 책임을 정의한다.
public class Skeleton : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Skeleton", SummonRank.Normal, 650, 150, 170);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0), //근접공격
            new AttackData(new TargetedAttackStrategy(), StatusType.None, 160, 0));
    }

}
