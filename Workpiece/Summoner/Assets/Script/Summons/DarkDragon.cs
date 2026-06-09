using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkDragon : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        FallbackStatusSet("DarkDragon", SummonRank.Boss, SummonType.DarkDragon, 3000, 400, 500);

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategiesSet(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.None, 370, 0), //전체공격 데미지 370
            new AttackAllEnemiesStrategy(StatusType.Burn, 0.2, 5,2), //화상, 체력 20% 데미지, 쿨타임 5턴, 지속시간 2턴
            new TargetedAttackStrategy(StatusType.None, 450, 0), //저격, 데미지450
            new TargetedAttackStrategy(StatusType.LifeDrain, 0.2, 4, 2)); //대상에게 흡혈, 쿨타임 4턴, 지속시간 2턴
    }

}
