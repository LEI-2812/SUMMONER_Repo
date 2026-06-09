using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{

    public override void SummonInitialize()
    {
        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        FallbackStatusSet("Eagle", SummonRank.High, SummonType.Eagle, 350, 45, 30);

        ApplayMultiple(StatMultiplierGet());

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategiesSet(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 1),
            new TargetedAttackStrategy(StatusType.None, GetHeavyAttackPower(), 2)); //저격 30데미지, 쿨타임 2턴

    }

}
