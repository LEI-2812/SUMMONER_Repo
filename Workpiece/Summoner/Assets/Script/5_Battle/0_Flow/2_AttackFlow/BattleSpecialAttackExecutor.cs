using System.Collections.Generic;
using UnityEngine;

// 역할: 이미 선택된 특수 공격 전략을 대상 플레이트에 실행한다.
public class BattleSpecialAttackExecutor
{
    private readonly PlateController plateController;

    public BattleSpecialAttackExecutor(PlateController plateController)
    {
        this.plateController = plateController;
    }

    public bool Execute(
        Summon attackSummon,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        if (attackSummon == null)
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
            return false;
        }

        if (!IsValidSpecialAttackIndex(attackSummon, specialAttackIndex))
        {
            Debug.LogError("유효하지 않은 특수 공격 인덱스입니다. 인덱스: " + specialAttackIndex);
            return false;
        }

        IAttackStrategy attackStrategy = attackSummon.GetSpecialAttackStrategy()[specialAttackIndex];

        if (attackStrategy is TargetedAttackStrategy targetedAttack)
        {
            return TryHandleTargetedAttack(
                attackSummon,
                targetedAttack,
                selectedPlateIndex,
                specialAttackIndex,
                isPlayer);
        }

        if (attackStrategy is AttackAllEnemiesStrategy attackAll)
        {
            attackSummon.AttackSoundPlay();
            HandleAttackAll(attackSummon, attackAll, selectedPlateIndex, specialAttackIndex, isPlayer);
            return true;
        }

        if (attackStrategy is ClosestEnemyAttackStrategy closestAttack)
        {
            attackSummon.AttackSoundPlay();
            HandleClosestEnemyAttack(attackSummon, closestAttack, selectedPlateIndex, specialAttackIndex, isPlayer);
            return true;
        }

        Debug.LogWarning("알 수 없는 공격 전략입니다.");
        return true;
    }

    private bool IsValidSpecialAttackIndex(Summon attackSummon, int specialAttackIndex)
    {
        if (attackSummon == null || attackSummon.GetSpecialAttackStrategy() == null)
        {
            return false;
        }

        return specialAttackIndex >= 0 && specialAttackIndex < attackSummon.GetSpecialAttackStrategy().Length;
    }

    private bool TryHandleTargetedAttack(
        Summon attackSummon,
        TargetedAttackStrategy targetedAttack,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        List<Plate> targetPlates = GetSpecialAttackTargetPlates(targetedAttack, isPlayer);

        if (!targetedAttack.BenefitEffectCheck() && !IsValidPlateIndex(selectedPlateIndex, targetPlates.Count))
        {
            Debug.Log(GetTargetedAttackInvalidLog(isPlayer));
            return false;
        }

        attackSummon.AttackSoundPlay();
        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            specialAttackIndex,
            GetTargetedAttackSuccessLog(targetedAttack, selectedPlateIndex, isPlayer));

        return true;
    }

    private void HandleAttackAll(
        Summon attackSummon,
        AttackAllEnemiesStrategy allAttackStrategy,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        List<Plate> targetPlates = GetSpecialAttackTargetPlates(allAttackStrategy, isPlayer);

        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            specialAttackIndex,
            isPlayer
                ? "아군의 특수 전체 공격이 성공적으로 수행되었습니다."
                : "적의 특수 전체 공격이 성공적으로 수행되었습니다.");
    }

    private void HandleClosestEnemyAttack(
        Summon attackSummon,
        ClosestEnemyAttackStrategy closestAttack,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        List<Plate> targetPlates = GetSpecialAttackTargetPlates(closestAttack, isPlayer);

        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            specialAttackIndex,
            isPlayer
                ? "아군의 특수 근접 공격이 성공적으로 수행되었습니다."
                : "적의 특수 근접 공격이 성공적으로 수행되었습니다.");
    }

    private bool IsValidPlateIndex(int selectedPlateIndex, int plateCount)
    {
        return selectedPlateIndex >= 0 && selectedPlateIndex < plateCount;
    }

    private List<Plate> GetSpecialAttackTargetPlates(IAttackStrategy attackStrategy, bool isPlayer)
    {
        bool targetsOwnPlates = attackStrategy.BenefitEffectCheck();

        if (isPlayer == targetsOwnPlates)
        {
            return plateController.GetPlayerPlates();
        }

        return plateController.GetEnermyPlates();
    }

    private string GetTargetedAttackSuccessLog(
        TargetedAttackStrategy targetedAttack,
        int selectedPlateIndex,
        bool isPlayer)
    {
        if (isPlayer)
        {
            return targetedAttack.BenefitEffectCheck()
                ? $"플레이어가 선택한 아군의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다."
                : $"플레이어가 선택한 적의 플레이트 {selectedPlateIndex}가 공격 대상입니다.";
        }

        return targetedAttack.BenefitEffectCheck()
            ? $"적이 선택한 적의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다."
            : $"적이 선택한 플레이어의 플레이트 {selectedPlateIndex}가 공격 대상입니다.";
    }

    private string GetTargetedAttackInvalidLog(bool isPlayer)
    {
        return isPlayer
            ? "유효한 적의 플레이트 인덱스가 선택되지 않았습니다."
            : "유효한 플레이어의 플레이트 인덱스가 선택되지 않았습니다.";
    }

    private void SpecialAttackApply(
        Summon attackSummon,
        List<Plate> targetPlates,
        int selectedPlateIndex,
        int specialAttackIndex,
        string successLog)
    {
        attackSummon.SpecialAttack(targetPlates, selectedPlateIndex, specialAttackIndex);
        Debug.Log(successLog);
    }
}
