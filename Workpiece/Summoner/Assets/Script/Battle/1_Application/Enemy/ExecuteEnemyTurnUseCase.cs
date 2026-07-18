using System.Collections.Generic;
using UnityEngine;

class ExecuteEnemyTurnUseCase
{
    private readonly IReadOnlyList<PlateData> enemyPlates;
    private readonly PlayerAttackPrediction playerAttackPrediction;
    private readonly ExecuteEnemyNormalAttackUseCase executeNormalAttackUseCase;
    private readonly ExecuteEnemyReactionUseCase executeEnemyReactionUseCase;
    private readonly AttackStateMachine attackStateMachine;
    private readonly BattleBoardData board;
    private readonly ExecuteSpecialAttackUseCase executeSpecialAttackUseCase;

    public ExecuteEnemyTurnUseCase(
        BattleBoardData board,
        PlayerAttackPrediction playerAttackPrediction,
        AttackStateMachine attackStateMachine)
    {
        enemyPlates = board.EnemyPlates;
        this.playerAttackPrediction = playerAttackPrediction;
        this.attackStateMachine = attackStateMachine;
        this.board = board;
        executeNormalAttackUseCase = new ExecuteEnemyNormalAttackUseCase(
            board,
            attackStateMachine);
        executeEnemyReactionUseCase = new ExecuteEnemyReactionUseCase(
            board,
            executeNormalAttackUseCase,
            attackStateMachine);
        executeSpecialAttackUseCase = new ExecuteSpecialAttackUseCase(board);
    }

    public void Execute()
    {
        List<AttackPredictionData> playerAttackPredictions = BuildPlayerAttackPredictions();
        RunEnemyActions(playerAttackPredictions);
    }

    private List<AttackPredictionData> BuildPlayerAttackPredictions()
    {
        PredictionBoardData boardData = new PredictionBoardData(board);
        List<AttackPredictionData> playerAttackPredictions =
            playerAttackPrediction.GetPlayerAttackPredictionList(boardData);
        if (playerAttackPredictions.Count == 0)
        {
            Debug.Log("플레이어 공격 예측 목록이 비어 있습니다.");
        }
        else
        {
            LogPlayerAttackPredictions(playerAttackPredictions);
        }

        return playerAttackPredictions;
    }

    private void LogPlayerAttackPredictions(
        IReadOnlyList<AttackPredictionData> playerAttackPredictions)
    {
        for (int index = 0; index < playerAttackPredictions.Count; index++)
        {
            AttackPredictionData prediction = playerAttackPredictions[index];
            Summon summon = prediction.GetAttackSummon();
            AttackProbabilityData probability = prediction.GetAttackProbability();
            string algorithmName = GetPredictionAlgorithmName(summon);
            string predictedAttack = prediction.GetAttackStrategy() == summon.GetAttackStrategy()
                ? "일반 공격"
                : "특수 공격";

            Debug.Log(
                $"[공격 예측] {algorithmName} 실행 | "
                + $"소환수: {summon.GetSummonName()} | "
                + $"공격 칸: {prediction.GetAttackSummonPlateIndex()} | "
                + $"대상 칸: {prediction.GetTargetPlateIndex()} | "
                + $"예측 공격: {predictedAttack} | "
                + $"일반: {probability.normalAttackProbability}% | "
                + $"특수: {probability.specialAttackProbability}% | "
                + $"이유: {probability.GetPredictionReason()}");
        }
    }

    private string GetPredictionAlgorithmName(Summon summon)
    {
        AttackData[] availableSpecialAttacks = summon.GetAvailableSpecialAttacks();
        if (availableSpecialAttacks == null || availableSpecialAttacks.Length == 0)
        {
            return "NormalAttackPrediction";
        }

        return summon.GetType().Name + "AttackPrediction";
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
            executeNormalAttackUseCase.Execute(
                attacker,
                attackerPlateIndex,
                board.FindClosestPlayerPlateIndex());
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

        executeNormalAttackUseCase.Execute(
            attacker,
            attackerPlateIndex,
            board.FindClosestPlayerPlateIndex());
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

            if (executeEnemyReactionUseCase.TryExecute(attacker, attackerPlateIndex, playerPrediction))
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

        if (!executeSpecialAttackUseCase.Execute(
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
    }
}
