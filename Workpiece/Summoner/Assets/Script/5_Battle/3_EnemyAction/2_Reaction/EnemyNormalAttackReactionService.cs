using UnityEngine;

// 역할: 특수공격 대응이 없을 때 적의 일반 대응 공격을 실행한다.
// 책임 아님: 예측 목록 순회, 예측 공격 타입별 특수 대응 분기.
internal class EnemyNormalAttackReactionService
{
    // 역할: 일반 대응 공격의 대상 플레이트와 아군 상태를 조회한다.
    private readonly PlateController plateController;

    // 역할: 선택된 일반 공격 또는 특수 공격을 실제로 실행한다.
    private readonly EnemyAttackExecutor attackExecutor;

    // 역할: 강공격 여부와 일반 대응 중 필요한 대상 조건을 판단한다.
    private readonly EnemyActionPicker actionPicker;

    // 역할: 일반 대응 중 보조로 사용할 수 있는 특수 공격 후보를 고른다.
    private readonly EnemySpecialAttackPicker specialAttackPicker;

    public EnemyNormalAttackReactionService(
        PlateController plateController,
        EnemyAttackExecutor attackExecutor,
        EnemyActionPicker actionPicker,
        EnemySpecialAttackPicker specialAttackPicker)
    {
        this.plateController = plateController;
        this.attackExecutor = attackExecutor;
        this.actionPicker = actionPicker;
        this.specialAttackPicker = specialAttackPicker;
    }

    public void ExecuteNormalReaction(Summon attacker, int attackerPlateIndex, int targetPlateIndex)
    {
        // 일반 대응 중에도 조건이 맞으면 보조 특수공격을 먼저 시도한다.
        if (!HasSpecialAttacks(attacker))
        {
            ExecutePickedNormalAttack(attacker, targetPlateIndex);
            return;
        }

        if (plateController.GetPlayerSummonCount() >= 2)
        {
            int highHealthPlayerIndex = actionPicker.PickHighHealthPlayerPlateIndexWithLowAlly(
                plateController.GetPlayerPlates());

            if (highHealthPlayerIndex != -1)
            {
                Debug.Log("소환수2마리 이상중 30%이상인 인덱스: " + highHealthPlayerIndex);
                EnemySpecialAttackPick pick = specialAttackPicker.PickLifeDrainReaction(
                    attacker,
                    highHealthPlayerIndex);

                if (TryExecuteSpecialAttackPick(attacker, pick))
                {
                    return;
                }
            }
            else
            {
                EnemySpecialAttackPick pick = specialAttackPicker.PickDefaultNormalReaction(
                    attacker,
                    targetPlateIndex);

                if (TryExecuteSpecialAttackPick(attacker, pick))
                {
                    return;
                }
            }
        }

        if (actionPicker.HasPlayerSummonOverMediumRank(plateController.GetPlayerPlates()))
        {
            EnemySpecialAttackPick pick = specialAttackPicker.PickMediumRankReaction(
                attacker,
                attackerPlateIndex,
                targetPlateIndex);

            if (TryExecuteSpecialAttackPick(attacker, pick))
            {
                return;
            }
        }

        ExecutePickedNormalAttack(attacker, targetPlateIndex);
    }

    private bool HasSpecialAttacks(Summon attacker)
    {
        return attacker.GetSpecialAttackStrategy() != null;
    }

    private void ExecutePickedNormalAttack(Summon attacker, int targetPlateIndex)
    {
        // 강공격 여부 선택은 Picker가, 실제 공격 실행은 Executor가 담당한다.
        if (actionPicker.ShouldUseHeavyNormalAttack())
        {
            attackExecutor.ExecuteHeavyNormalAttack(attacker);
            return;
        }

        attackExecutor.ExecuteNormalAttack(attacker, targetPlateIndex);
        Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
    }

    private bool TryExecuteSpecialAttackPick(Summon attacker, EnemySpecialAttackPick pick)
    {
        // 일반 대응 중 선택된 보조 특수공격 후보가 없으면 일반공격으로 이어진다.
        if (!pick.HasValue)
        {
            return false;
        }

        attackExecutor.ExecuteSpecialAttack(
            attacker,
            pick.TargetPlateIndex,
            pick.SpecialAttackIndex,
            pick.LogMessage);
        return true;
    }
}
