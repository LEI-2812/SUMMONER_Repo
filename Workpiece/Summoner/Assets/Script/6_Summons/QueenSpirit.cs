using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 여왕 정령 소환수의 기본 데이터와 공격 동작을 초기화한다.
public class QueenSpirit : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("QueenSpirit", SummonRank.Special, 400, 100, 140);

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower(), 0),
            new AttackAllEnemiesStrategy(StatusType.None, 70, 0), //전체공격 데미지 70
            new AttackAllEnemiesStrategy(StatusType.Heal, 0.2, 3), //아군 전체 20% 회복 쿨타임 3턴
            new TargetedAttackStrategy(StatusType.Stun,0,3,1)); //대상에게 혼란, 쿨타임 3턴, 지속시간 1턴
    }

}
