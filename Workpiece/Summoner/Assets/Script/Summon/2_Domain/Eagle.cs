using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: Eagle의 책임을 정의한다.
public class Eagle : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Eagle", SummonRank.High, 350, 45, 30);


        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 1),
            new AttackData(new TargetedAttackStrategy(), StatusType.None, GetHeavyAttackPower(), 2));

    }

}
