using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: Rabbit의 책임을 정의한다.
public class Rabbit : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Rabbit", SummonRank.Medium, 300, 37, 0);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 1), //근접공격
            new AttackData(new TargetedAttackStrategy(), StatusType.Heal, 0.3, 3));

    }

}
