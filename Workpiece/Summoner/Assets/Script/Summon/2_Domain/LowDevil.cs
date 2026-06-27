using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: LowDevil의 책임을 정의한다.
public class LowDevil : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("LowDevil", SummonRank.Normal, 700, 180, 220);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 0),
            new AttackData(new TargetedAttackStrategy(), StatusType.Curse, 0.2 , 4, 1),
            new AttackData(new TargetedAttackStrategy(), StatusType.OnceInvincibility, 0, 2));
    }

}
