using System.Collections.Generic;
using UnityEngine;

// 역할: 고양이 소환수의 기본 데이터와 특수 공격 동작을 초기화한다.
public class Cat : Summon
{

    public override void SummonInitialize()
    {
        if (TryApplyAssignedSummonData())
        {
            return;
        }

        SetFallbackStatus("Cat", SummonRank.Low, SummonType.Cat, 250, 30, 40);
        ApplayMultiple(GetStatMultiplier());

        // 일반 공격: 가장 가까운 적 공격
        SetAttackStrategies(
            new ClosestEnemyAttackStrategy(StatusType.None, GetAttackPower() ,1),
            new ClosestEnemyAttackStrategy(StatusType.None, GetHeavyAttackPower(), 1)); //근접공격, 20데미지, 쿨타임1턴

    }

    public override void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (!SpecialAttackIndexCheck(SpecialAttackArrayIndex))
        {
            Debug.Log("유효하지 않은 특수 공격 인덱스입니다.");
            return;
        }

        var specialAttack = GetSpecialAttackStrategy()[SpecialAttackArrayIndex];

        if (!AttackCanUse(specialAttack))
        {
            Debug.Log("특수 스킬이 쿨타임 중입니다.");
            return;
        }

        CatSpecialAttackWithHeavyAttackPowerExecute(
            specialAttack,
            enemyPlates,
            selectedPlateIndex,
            SpecialAttackArrayIndex);
    }

    private void CatSpecialAttackWithHeavyAttackPowerExecute(
        IAttackStrategy specialAttack,
        List<Plate> enemyPlates,
        int selectedPlateIndex,
        int specialAttackArrayIndex)
    {
        // 근접공격 effect가 attacker.GetAttackPower()를 사용하므로
        // 강공격도 버프/저주가 반영되는 현재 공격력 경로로 실행한다.
        double originAttackPower = GetAttackPower();
        SetAttackPower(GetHeavyAttackPower());

        specialAttack.Attack(this, enemyPlates, selectedPlateIndex, specialAttackArrayIndex);
        AttackMotionPlay(false);
        AttackCooldownApply(specialAttack);
        SetAttackPower(originAttackPower);
        SetIsAttack(false);
    }

}
