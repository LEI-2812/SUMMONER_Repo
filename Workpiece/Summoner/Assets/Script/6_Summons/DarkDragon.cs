using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 다크 드래곤 적 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class DarkDragon : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("DarkDragon", SummonRank.Boss, SummonType.DarkDragon, 3000, 400, 500);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.None, 370, 0), //전체공격 데미지 370
            new AttackAllEnemiesStrategy(StatusType.Burn, 0.2, 5,2), //화상, 체력 20% 데미지, 쿨타임 5턴, 지속시간 2턴
            new TargetedAttackStrategy(StatusType.None, 450, 0), //저격, 데미지450
            new TargetedAttackStrategy(StatusType.LifeDrain, 0.2, 4, 2)); //대상에게 흡혈, 쿨타임 4턴, 지속시간 2턴
    }

}
