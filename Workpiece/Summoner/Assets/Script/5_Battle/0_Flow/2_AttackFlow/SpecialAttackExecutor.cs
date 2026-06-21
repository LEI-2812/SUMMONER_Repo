using System.Collections.Generic;
using UnityEngine;

// 역할: 전투 흐름에서 이미 선택된 특수 공격 전략을 실제 대상 플레이트에 실행한다.
public class SpecialAttackExecutor
{
    // 역할: 플레이어/적 플레이트 목록을 제공한다.
    private readonly PlateController plateController;

    public SpecialAttackExecutor(PlateController plateController)
    {
        this.plateController = plateController;
    }

    // 역할: 특수 공격 인덱스를 검증한 뒤 전략 종류에 맞는 실행 흐름으로 보낸다.
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

    // 역할: 소환수가 가진 특수 공격 배열에서 선택 인덱스를 사용할 수 있는지 확인한다.
    private bool IsValidSpecialAttackIndex(Summon attackSummon, int specialAttackIndex)
    {
        if (attackSummon == null || attackSummon.GetSpecialAttackStrategy() == null)
        {
            return false;
        }

        return specialAttackIndex >= 0 && specialAttackIndex < attackSummon.GetSpecialAttackStrategy().Length;
    }

    // 역할: 단일 대상 특수 공격의 대상 검증, 사운드 재생, 공격 적용을 한 흐름으로 처리한다.
    private bool TryHandleTargetedAttack(
        Summon attackSummon,
        TargetedAttackStrategy targetedAttack,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        IReadOnlyList<Plate> targetPlates = GetSpecialAttackTargetPlates(targetedAttack, isPlayer);

        if (!targetedAttack.TargetsOwnPlates() && !IsValidPlateIndex(selectedPlateIndex, targetPlates.Count))
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

    // 역할: 전체 대상 특수 공격이 적용될 플레이트 목록을 정하고 공격을 적용한다.
    private void HandleAttackAll(
        Summon attackSummon,
        AttackAllEnemiesStrategy allAttackStrategy,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        IReadOnlyList<Plate> targetPlates = GetSpecialAttackTargetPlates(allAttackStrategy, isPlayer);

        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            specialAttackIndex,
            isPlayer
                ? "아군의 특수 전체 공격이 성공적으로 수행되었습니다."
                : "적의 특수 전체 공격이 성공적으로 수행되었습니다.");
    }

    // 역할: 가장 가까운 대상 특수 공격이 적용될 플레이트 목록을 정하고 공격을 적용한다.
    private void HandleClosestEnemyAttack(
        Summon attackSummon,
        ClosestEnemyAttackStrategy closestAttack,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        IReadOnlyList<Plate> targetPlates = GetSpecialAttackTargetPlates(closestAttack, isPlayer);

        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            specialAttackIndex,
            isPlayer
                ? "아군의 특수 근접 공격이 성공적으로 수행되었습니다."
                : "적의 특수 근접 공격이 성공적으로 수행되었습니다.");
    }

    // 역할: 선택한 플레이트 인덱스가 대상 목록 범위 안에 있는지 확인한다.
    private bool IsValidPlateIndex(int selectedPlateIndex, int plateCount)
    {
        return selectedPlateIndex >= 0 && selectedPlateIndex < plateCount;
    }

    // 역할: 공격 주체와 전략의 아군 대상 여부를 기준으로 실제 대상 플레이트 목록을 고른다.
    private IReadOnlyList<Plate> GetSpecialAttackTargetPlates(IAttackStrategy attackStrategy, bool isPlayer)
    {
        bool targetsOwnPlates = attackStrategy.TargetsOwnPlates();

        if (isPlayer == targetsOwnPlates)
        {
            return plateController.GetPlayerPlates();
        }

        return plateController.GetEnermyPlates();
    }

    // 역할: 단일 대상 특수 공격이 성공했을 때 출력할 상황별 로그 문구를 만든다.
    private string GetTargetedAttackSuccessLog(
        TargetedAttackStrategy targetedAttack,
        int selectedPlateIndex,
        bool isPlayer)
    {
        if (isPlayer)
        {
            return targetedAttack.TargetsOwnPlates()
                ? $"플레이어가 선택한 아군의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다."
                : $"플레이어가 선택한 적의 플레이트 {selectedPlateIndex}가 공격 대상입니다.";
        }

        return targetedAttack.TargetsOwnPlates()
            ? $"적이 선택한 적의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다."
            : $"적이 선택한 플레이어의 플레이트 {selectedPlateIndex}가 공격 대상입니다.";
    }

    // 역할: 단일 대상 특수 공격에서 잘못된 대상 인덱스를 선택했을 때 출력할 로그 문구를 만든다.
    private string GetTargetedAttackInvalidLog(bool isPlayer)
    {
        return isPlayer
            ? "유효한 적의 플레이트 인덱스가 선택되지 않았습니다."
            : "유효한 플레이어의 플레이트 인덱스가 선택되지 않았습니다.";
    }

    // 역할: 소환수의 특수 공격을 실행하고 성공 로그를 남긴다.
    private void SpecialAttackApply(
        Summon attackSummon,
        IReadOnlyList<Plate> targetPlates,
        int selectedPlateIndex,
        int specialAttackIndex,
        string successLog)
    {
        attackSummon.SpecialAttack(targetPlates, selectedPlateIndex, specialAttackIndex);
        Debug.Log(successLog);
    }
}
