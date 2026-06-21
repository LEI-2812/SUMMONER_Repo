using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 하급 악마 적 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class LowDevil : Summon
{
    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("LowDevil", SummonRank.Normal, 700, 180, 220);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new TargetedAttackStrategy(StatusType.Curse, 0.2 ,4,1),//저주, 공격력의 20% 감소, 쿨타임 4턴, 적용 1턴
            new TargetedAttackStrategy(StatusType.OnceInvincibility,0,2)); //1번 무적, 쿨타임 2턴
    }

}
