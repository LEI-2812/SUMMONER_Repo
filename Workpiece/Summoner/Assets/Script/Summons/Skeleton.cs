using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        FallbackStatusSet("Skeleton", SummonRank.Normal, SummonType.Skeleton, 650, 150, 170);

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategiesSet(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0), //근접공격
            new TargetedAttackStrategy(StatusType.None, 160,0)); //타겟공격 160데미지
    }

}
