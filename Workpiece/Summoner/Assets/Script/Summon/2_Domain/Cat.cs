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

    public override void SpecialAttack(
        BattleBoardData board,
        int selectedPlateIndex,
        int SpecialAttackArrayIndex,
        bool isPlayerAttack)
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
            board,
            selectedPlateIndex,
            isPlayerAttack);
    }

    private void CatSpecialAttackWithHeavyAttackPowerExecute(
        AttackData specialAttack,
        BattleBoardData board,
        int selectedPlateIndex,
        bool isPlayerAttack)
    {
        double originAttackPower = GetAttackPower();
        SetAttackPower(GetHeavyAttackPower());

        SpecialAttackExecute(specialAttack, board, selectedPlateIndex, isPlayerAttack);
        SetAttackPower(originAttackPower);
    }

}
