using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 역할: 늑대 소환수의 기본 데이터와 특수 공격 동작을 초기화한다.
public class Wolf : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Wolf", SummonRank.High, 350, 50, 30);
        ApplayMultiple(GetStatMultiplier());

        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1), //근접공격
            new AttackAllEnemiesStrategy(StatusType.None, GetHeavyAttackPower(), 2));//전체공격, 25데미지, 쿨타임2턴
    }

}
