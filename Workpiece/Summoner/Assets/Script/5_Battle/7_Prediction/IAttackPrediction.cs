using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 소환수별 공격 예측 클래스가 지켜야 하는 계약을 정의한다.
public interface IAttackPrediction
{
    // 역할: 가장 가까운 적을 일반 공격으로 처치할 수 있는지 구현체가 같은 방식으로 알려주게 한다.
    int GetIndexOfNormalAttackCanKill(Summon attackingSummon, List<Plate> enermyPlates);

    // 역할: 적 플레이트에서 가장 먼저 공격 대상이 될 적 위치를 구현체가 알려주게 한다.
    int GetClosestEnermyIndex(List<Plate> enermyPlates);

    // 역할: 소환수별 규칙으로 일반 공격과 특수 공격 중 어떤 선택 가능성이 높은지 예측하게 한다.
    public AttackPrediction GetAttackPrediction(Summon summon, int attackSummonPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates);

    // 역할: 이 예측기가 어떤 소환수 타입을 담당하는지 알려주게 한다.
    SummonType GetPreSummonType();
}
