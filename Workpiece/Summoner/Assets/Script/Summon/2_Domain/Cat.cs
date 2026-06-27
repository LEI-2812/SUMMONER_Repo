using System.Collections.Generic;
using UnityEngine;

// 역할: Cat의 책임을 정의한다.
public class Cat : Summon
{

    protected override void ApplyFallbackData()
    {
        SetFallbackStatus("Cat", SummonRank.Low, 250, 30, 40);

        SetAttackStrategies(
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetAttackPower(), 1),
            new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, GetHeavyAttackPower(), 1));

    }

    public override void SpecialAttack(IReadOnlyList<BattleBoardInputController> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (!SpecialAttackIndexCheck(SpecialAttackArrayIndex))
        {
            Debug.Log("유효하지 않은 특수 공격 인덱스입니다.");
            return;
        }

        var specialAttack = GetSpecialAttackStrategy()[SpecialAttackArrayIndex];

        if (!AttackCanUse(specialAttack))
        {
            Debug.Log("특수 공격이 쿨타임 중이거나 사용할 수 없습니다.");
            return;
        }

        CatSpecialAttackWithHeavyAttackPowerExecute(
            specialAttack,
            enemyPlates,
            selectedPlateIndex);
    }

    private void CatSpecialAttackWithHeavyAttackPowerExecute(
        AttackData specialAttack,
        IReadOnlyList<BattleBoardInputController> enemyPlates,
        int selectedPlateIndex)
    {
        double originAttackPower = GetAttackPower();
        SetAttackPower(GetHeavyAttackPower());

        SpecialAttackExecute(specialAttack, enemyPlates, selectedPlateIndex);
        SetAttackPower(originAttackPower);
    }

}
