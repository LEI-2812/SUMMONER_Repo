using System.Collections.Generic;
using UnityEngine;

// 역할: 적 소환수 1명의 상태 회복 선사용, 대응 공격, 연속 공격을 실행한다.
// 책임 아님: 적 플레이트 목록 순회, 플레이어 공격 예측 생성.
internal class EnemyTurnActionRunner
{
    // 역할: 플레이어 공격 예측에 대한 적 대응 흐름을 호출한다.
    private readonly EnemyPredictionReactionService predictionReactionService;

    // 역할: 상태 회복 선사용과 연속 공격 가능 여부를 판단한다.
    private readonly EnemyActionPicker actionPicker = new EnemyActionPicker();

    // 역할: 상태 회복용 특수 공격처럼 직접 실행이 필요한 공격을 처리한다.
    private readonly EnemyAttackExecutor attackExecutor;

    public EnemyTurnActionRunner(
        EnemyPredictionReactionService predictionReactionService,
        PlateController plateController)
    {
        this.predictionReactionService = predictionReactionService;
        attackExecutor = new EnemyAttackExecutor(plateController);
    }

    public List<AttackPrediction> RunEnemyAction(
        Summon attackingSummon,
        List<Plate> enemyPlates,
        int enemyPlateIndex,
        List<AttackPrediction> playerAttackPredictions)
    {
        if (!CanEnemyAct(attackingSummon))
        {
            return playerAttackPredictions;
        }

        if (TryUseHealBeforeReaction(attackingSummon, enemyPlates, enemyPlateIndex))
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

    private List<AttackPrediction> ReactOnceToPlayerPrediction(
        Summon attackingSummon,
        int enemyPlateIndex,
        List<AttackPrediction> playerAttackPredictions)
    {
        return predictionReactionService.ReactToPredictions(
            attackingSummon,
            enemyPlateIndex,
            playerAttackPredictions);
    }

    private List<AttackPrediction> ReactAgainIfCanContinueAttack(
        Summon attackingSummon,
        int enemyPlateIndex,
        List<AttackPrediction> playerAttackPredictions)
    {
        for (int seq = 0; seq < 2; seq++)
        {
            if (!actionPicker.CanContinueAttack(attackingSummon))
            {
                break;
            }

            Debug.Log("연속공격 발동");
            playerAttackPredictions = ReactOnceToPlayerPrediction(
                attackingSummon,
                enemyPlateIndex,
                playerAttackPredictions);
        }

        return playerAttackPredictions;
    }

    // 화상, 흡혈, 독성에 대해서는 힐스킬이 있을 경우 힐을 먼저 사용한다.
    private bool TryUseHealBeforeReaction(
        Summon attackingSummon,
        List<Plate> enemyPlates,
        int enemyPlateIndex)
    {
        int healSpecialAttackIndex = actionPicker.PickHealSpecialAttackIndexForDamageStatus(attackingSummon);
        if (healSpecialAttackIndex < 0)
        {
            return false;
        }

        attackExecutor.ExecuteDirectSpecialAttack(
            attackingSummon,
            enemyPlates,
            enemyPlateIndex,
            healSpecialAttackIndex);
        return true;
    }
}
