using System.Collections.Generic;
using UnityEngine;

class ExecuteEnemyTurnUseCase
{
    private readonly IReadOnlyList<BattleBoardInputController> playerPlates;
    private readonly IReadOnlyList<BattleBoardInputController> enemyPlates;
    private readonly PlayerAttackPrediction playerAttackPrediction;
    private readonly EnemyNormalAttackReaction normalAttackReaction;
    private readonly EnemySpecialAttackReaction specialAttackReaction;
    private readonly AttackStateMachine attackStateMachine;
    private readonly PlateBoardView plateBoardController;
    private readonly SpecialAttackExecution specialAttackExecution;

    public ExecuteEnemyTurnUseCase(
        PlateBoardView plateBoardController,
        PlayerAttackPrediction playerAttackPrediction,
        AttackStateMachine attackStateMachine)
    {
        playerPlates = plateBoardController.GetPlayerPlates();
        enemyPlates = plateBoardController.GetEnemyPlates();
        this.playerAttackPrediction = playerAttackPrediction;
        this.attackStateMachine = attackStateMachine;
        this.plateBoardController = plateBoardController;
        normalAttackReaction = new EnemyNormalAttackReaction(
            plateBoardController,
            attackStateMachine);
        specialAttackReaction = new EnemySpecialAttackReaction(
            plateBoardController,
            normalAttackReaction,
            attackStateMachine);
        specialAttackExecution = new SpecialAttackExecution(
            playerPlates,
            enemyPlates);
    }

    public void Execute()
    {
        List<AttackPredictionData> playerAttackPredictions = BuildPlayerAttackPredictions();
        RunEnemyActions(playerAttackPredictions);
    }

    private List<AttackPredictionData> BuildPlayerAttackPredictions()
    {
        PredictionBoardData boardData = new PredictionBoardData(
            playerPlates,
            enemyPlates);
        List<AttackPredictionData> playerAttackPredictions = playerAttackPrediction.GetPlayerAttackPredictionList(
            boardData.PlayerPlates,
            boardData.EnemyPlates);
        if (playerAttackPredictions.Count == 0)
        {
            Debug.Log("플레이어 공격 예측 목록이 비어 있습니다.");
        }

        return playerAttackPredictions;
    }

    private void RunEnemyActions(List<AttackPredictionData> playerAttackPredictions)
    {
        for (int index = 0; index < enemyPlates.Count; index++)
        {
            Summon attackingSummon = enemyPlates[index].GetCurrentSummon();
            if (attackingSummon == null)
            {
                continue;
            }

            playerAttackPredictions = ExecuteEnemyAction(
                attackingSummon,
                index,
                playerAttackPredictions);
        }
    }

    private List<AttackPredictionData> ExecuteEnemyAction(
        Summon attackingSummon,
        int enemyPlateIndex,
        List<AttackPredictionData> playerAttackPredictions)
    {
        if (!CanEnemyAct(attackingSummon))
        {
            return playerAttackPredictions;
        }

        if (TryUseHealBeforeReaction(attackingSummon, enemyPlateIndex))
        {
            return playerAttackPredictions;
        }

        playerAttackPredictions = ReactOnceToPlayerPrediction(
            attackingSummon,
            enemyPlateIndex,
            playerAttackPredictions);

        return ReactAgainIfCanContinueAttack(
            attackingSummon,
            enemyPlateIndex,
            playerAttackPredictions);
    }

    private bool CanEnemyAct(Summon attackingSummon)
    {
        return attackingSummon != null && !attackingSummon.IsStun();
    }

    private List<AttackPredictionData> ReactOnceToPlayerPrediction(
        Summon attackingSummon,
        int enemyPlateIndex,
        List<AttackPredictionData> playerAttackPredictions)
    {
        return ReactToPredictions(
            attackingSummon,
            enemyPlateIndex,
            playerAttackPredictions);
    }

    private List<AttackPredictionData> ReactAgainIfCanContinueAttack(
        Summon attackingSummon,
        int enemyPlateIndex,
        List<AttackPredictionData> playerAttackPredictions)
    {
        for (int seq = 0; seq < 2; seq++)
        {
            if (!CanContinueAttack(attackingSummon))
            {
                break;
            }

            Debug.Log("추가 공격이 발동했습니다.");
            playerAttackPredictions = ReactOnceToPlayerPrediction(
                attackingSummon,
                enemyPlateIndex,
                playerAttackPredictions);
        }

        return playerAttackPredictions;
    }

    private List<AttackPredictionData> ReactToPredictions(
        Summon attacker,
        int attackerPlateIndex,
        List<AttackPredictionData> playerAttackPredictions)
    {
        if (playerAttackPredictions.Count == 0)
        {
            normalAttackReaction.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                plateBoardController.GetClosestPlayerPlateIndex());
            Debug.Log("플레이어 예측이 없어 적이 일반 공격을 사용했습니다.");
            return playerAttackPredictions;
        }

        Debug.Log("적이 플레이어 공격 예측을 확인합니다.");

        if (TryReactToAnyPrediction(
            attacker,
            attackerPlateIndex,
            playerAttackPredictions,
            out int reactedPredictionIndex))
        {
            playerAttackPredictions.RemoveAt(reactedPredictionIndex);
            Debug.Log("적이 플레이어 예측 하나에 반응했습니다.");
            return playerAttackPredictions;
        }

        normalAttackReaction.ExecuteNormalReaction(
            attacker,
            attackerPlateIndex,
            plateBoardController.GetClosestPlayerPlateIndex());
        Debug.Log("일치하는 반응이 없어 적이 일반 공격을 사용했습니다.");
        return playerAttackPredictions;
    }

    private bool TryReactToAnyPrediction(
        Summon attacker,
        int attackerPlateIndex,
        List<AttackPredictionData> playerAttackPredictions,
        out int reactedPredictionIndex)
    {
        reactedPredictionIndex = -1;

        for (int i = 0; i < playerAttackPredictions.Count; i++)
        {
            AttackPredictionData playerPrediction = playerAttackPredictions[i];
            AttackProbabilityData attackProbabilityData = playerPrediction.GetAttackProbability();

            if (!CanReactWithSpecialAttack(attackProbabilityData))
            {
                continue;
            }

            if (specialAttackReaction.TryReactToPrediction(attacker, attackerPlateIndex, playerPrediction))
            {
                reactedPredictionIndex = i;
                return true;
            }
        }

        return false;
    }

    private bool CanReactWithSpecialAttack(AttackProbabilityData attackProbabilityData)
    {
        float randomValue = Random.Range(0f, 100f);
        if (randomValue < attackProbabilityData.specialAttackProbability)
        {
            Debug.Log("적 특수 반응을 선택했습니다.");
        }

        return randomValue < attackProbabilityData.specialAttackProbability;
    }

    private bool TryUseHealBeforeReaction(
        Summon attackingSummon,
        int enemyPlateIndex)
    {
        int healSpecialAttackIndex = PickHealSpecialAttackIndexForDamageStatus(attackingSummon);
        if (healSpecialAttackIndex < 0)
        {
            return false;
        }

        if (!specialAttackExecution.Execute(
            attackingSummon,
            enemyPlateIndex,
            healSpecialAttackIndex,
            isPlayerAttacker: false))
        {
            return false;
        }

        attackStateMachine.CompleteAttack();
        ResetAttackState();
        return true;
    }

    private int PickHealSpecialAttackIndexForDamageStatus(Summon summon)
    {
        if (!HasDamageStatus(summon))
        {
            return -1;
        }

        AttackData[] specialAttackStrategies = summon.GetSpecialAttackStrategy();
        if (specialAttackStrategies == null)
        {
            return -1;
        }

        for (int i = 0; i < specialAttackStrategies.Length; i++)
        {
            AttackData attackStrategy = specialAttackStrategies[i];
            if (attackStrategy == null)
            {
                continue;
            }

            if (attackStrategy.GetStatusType() == StatusType.Heal && attackStrategy.GetCurrentCooldown() <= 0)
            {
                return i;
            }
        }

        return -1;
    }

    private bool CanContinueAttack(Summon summon)
    {
        float randomValue = Random.Range(0f, 100f);

        if (summon.GetSummonRank() == SummonRank.Special)
        {
            return randomValue <= 20f;
        }

        if (summon.GetSummonRank() == SummonRank.Boss)
        {
            return randomValue <= 30f;
        }

        return false;
    }

    private bool HasDamageStatus(Summon summon)
    {
        foreach (StatusType statusType in summon.GetAllStatusTypes())
        {
            if (statusType == StatusType.Burn || statusType == StatusType.LifeDrain || statusType == StatusType.Poison)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetAttackState()
    {
        attackStateMachine.Reset();
        plateBoardController.ResetAllPlateHighlight();
    }
}
