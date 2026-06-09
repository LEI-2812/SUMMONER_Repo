using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        FallbackStatusSet("KingSlime", SummonRank.Special, SummonType.KingSlime, 250, 50, 65);

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategiesSet(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.None, 35, 1),//전체공격
            new TargetedAttackStrategy(StatusType.Shield, 80, 2));//쉴드 
    }

}
