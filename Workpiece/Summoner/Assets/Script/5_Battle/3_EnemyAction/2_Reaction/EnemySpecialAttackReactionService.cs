using UnityEngine;

// 역할: 플레이어 예측 1개에 대해 적 특수공격 대응을 시도한다.
// 책임 아님: 예측 목록 순회와 제거, 일반 대응 공격의 세부 선택.
internal class EnemySpecialAttackReactionService
{
    // 역할: 대응 특수 공격의 대상이 될 플레이어/적 플레이트 위치를 조회한다.
    private readonly PlateController plateController;

    // 역할: 예측된 공격 종류에 맞는 특수 공격 후보를 고른다.
    private readonly EnemySpecialAttackPicker specialAttackPicker;

    // 역할: 특수 공격으로 대응할 수 없을 때 일반 대응 공격으로 넘긴다.
    private readonly EnemyNormalAttackReactionService normalAttackReactionService;

    // 역할: 선택된 특수 공격 후보를 실제 공격 실행으로 연결한다.
    private readonly EnemyAttackExecutor attackExecutor;

    public EnemySpecialAttackReactionService(
        PlateController plateController,
        EnemySpecialAttackPicker specialAttackPicker,
        EnemyNormalAttackReactionService normalAttackReactionService,
        EnemyAttackExecutor attackExecutor)
    {
        this.plateController = plateController;
        this.specialAttackPicker = specialAttackPicker;
        this.normalAttackReactionService = normalAttackReactionService;
        this.attackExecutor = attackExecutor;
    }

    public bool TryReactToPrediction(
        Summon attacker,
        int attackerPlateIndex,
        AttackPrediction playerPrediction)
    {
        // 예측된 공격 전략의 종류와 상태 타입만 보고 어느 대응 규칙을 쓸지 고른다.
        if (playerPrediction.GetAttackStrategy() is AttackAllEnemiesStrategy allAttackStrategy)
        {
            if (allAttackStrategy.GetStatusType() == StatusType.Poison)
            {
                return TryReactToPoisonAllAttack(attacker, attackerPlateIndex, playerPrediction);
            }

            if (allAttackStrategy.GetStatusType() == StatusType.None)
            {
                return TryReactToAllAttack(attacker, attackerPlateIndex, playerPrediction);
            }
        }
        else if (playerPrediction.GetAttackStrategy() is TargetedAttackStrategy targetAttackStrategy)
        {
            if (targetAttackStrategy.GetStatusType() == StatusType.None)
            {
                return TryReactToTargetAttack(attacker, attackerPlateIndex, playerPrediction);
            }

            if (targetAttackStrategy.GetStatusType() == StatusType.Upgrade)
            {
                return TryReactToUpgradeAttack(attacker, attackerPlateIndex, playerPrediction);
            }

            if (targetAttackStrategy.GetStatusType() == StatusType.Heal)
            {
                return TryReactToHealAttack(attacker, attackerPlateIndex, playerPrediction);
            }
        }
        else if (playerPrediction.GetAttackStrategy() is ClosestEnemyAttackStrategy)
        {
            return false;
        }
        else
        {
            Debug.Log("적 소환수의 특수공격 대응에서 공격이 잘못 들어옴");
            return false;
        }

        Debug.Log("적 소환수의 특수공격 대응 조건에 맞는 분기가 없습니다.");
        return false;
    }

    private bool TryReactToPoisonAllAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPrediction playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReactionService.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetTargetPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 스킬이 없어서 일반 공격을 실행했습니다.");
            return true;
        }

        EnemySpecialAttackPick pick = specialAttackPicker.PickPoisonReaction(
            attacker,
            playerPrediction,
            plateController.GetLowestHealthEnermyPlateIndex());
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToTargetAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPrediction playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReactionService.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
            return true;
        }

        EnemySpecialAttackPick pick = specialAttackPicker.PickTargetNoneReaction(
            attacker,
            attackerPlateIndex,
            playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToAllAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPrediction playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReactionService.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 일반 대응했습니다.");
            return true;
        }

        EnemySpecialAttackPick pick = specialAttackPicker.PickAllNoneReaction(
            attacker,
            attackerPlateIndex,
            playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToHealAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPrediction playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReactionService.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
            return true;
        }

        EnemySpecialAttackPick pick = specialAttackPicker.PickHealReaction(attacker, playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToUpgradeAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPrediction playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReactionService.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
            return true;
        }

        EnemySpecialAttackPick pick = specialAttackPicker.PickUpgradeReaction(attacker, playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool HasSpecialAttacks(Summon attacker)
    {
        // 특수공격이 없으면 특수 대응 대신 일반 대응 규칙으로 떨어진다.
        return attacker.GetSpecialAttackStrategy() != null;
    }

    private bool TryExecuteSpecialAttackPick(Summon attacker, EnemySpecialAttackPick pick)
    {
        // Picker는 후보만 고르므로, 실제 실행은 여기에서 Executor에 위임한다.
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
