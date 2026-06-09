using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpirit : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        FallbackStatusSet("GrassSpirit", SummonRank.Normal, SummonType.GrassSpirit, 350, 80, 110);

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategiesSet(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.Heal, 0.1, 3)); //최대 체력의 0.1만큼 회복, 쿨타임 3턴
    }

}
