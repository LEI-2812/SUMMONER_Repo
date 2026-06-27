using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 역할: Wolf의 책임을 정의한다.
public class Wolf : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Wolf", SummonRank.High, 350, 50, 30);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 1), //근접공격
            new AttackData(new AttackAllEnemiesStrategy(), StatusType.None, GetHeavyAttackPower(), 2));
    }

}
