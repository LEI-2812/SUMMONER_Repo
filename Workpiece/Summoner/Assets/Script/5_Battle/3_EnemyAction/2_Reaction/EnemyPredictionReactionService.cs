using System.Collections.Generic;
using UnityEngine;

// 역할: 플레이어 공격 예측 목록을 순회하고 대응한 예측을 제거한다.
// 책임 아님: 공격 타입별 대응 규칙 상세, 일반공격 fallback 상세.
internal class EnemyPredictionReactionService
{
    // 역할: 예측 목록이 비었을 때 일반 대응 공격의 기본 대상을 찾는다.
    private readonly PlateController plateController;

    // 역할: 예측된 공격에 특수 대응을 시도할지 확률로 판단한다.
    private readonly EnemyActionPicker actionPicker = new EnemyActionPicker();

    // 역할: 예측 1개에 대해 특수 공격 대응 규칙을 실행한다.
    private readonly EnemySpecialAttackReactionService specialAttackReactionService;

    // 역할: 특수 대응이 없을 때 일반 대응 공격을 실행한다.
    private readonly EnemyNormalAttackReactionService normalAttackReactionService;

    public EnemyPredictionReactionService(PlateController plateController, BattleController battleController)
    {
        this.plateController = plateController;

        EnemyAttackExecutor attackExecutor = new EnemyAttackExecutor(plateController, battleController);
        EnemySpecialAttackPicker specialAttackPicker = new EnemySpecialAttackPicker();

        normalAttackReactionService = new EnemyNormalAttackReactionService(
            plateController,
            attackExecutor,
            actionPicker,
            specialAttackPicker);

        specialAttackReactionService = new EnemySpecialAttackReactionService(
            plateController,
            specialAttackPicker,
            normalAttackReactionService,
            attackExecutor);
    }

    public List<AttackPrediction> ReactToPredictions(
        Summon attacker,
        int attackerPlateIndex,
        List<AttackPrediction> playerAttackPredictions)
    {
        if (playerAttackPredictions.Count == 0)
        {
            normalAttackReactionService.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                plateController.GetClosestPlayerPlateIndex());
            Debug.Log("리스트가 비어서 일반공격 대응");
            return playerAttackPredictions;
        }

        Debug.Log("대응공격 중...");

        if (TryReactToAnyPrediction(
            attacker,
            attackerPlateIndex,
            playerAttackPredictions,
            out int reactedPredictionIndex))
        {
            playerAttackPredictions.RemoveAt(reactedPredictionIndex);
            Debug.Log("특수공격 대응 완료, 리스트에서 항목 제거");
            return playerAttackPredictions;
        }

        normalAttackReactionService.ExecuteNormalReaction(
            attacker,
            attackerPlateIndex,
            plateController.GetClosestPlayerPlateIndex());
        Debug.Log("특수공격에 대한 대응공격이 없거나 확률이 걸렸습니다. 일반공격으로 대응");
        return playerAttackPredictions;
    }

    private bool TryReactToAnyPrediction(
        Summon attacker,
        int attackerPlateIndex,
        List<AttackPrediction> playerAttackPredictions,
        out int reactedPredictionIndex)
    {
        // 성공한 예측 인덱스만 돌려주고, 실제 제거는 목록을 소유한 public 흐름에서 처리한다.
        reactedPredictionIndex = -1;

        for (int i = 0; i < playerAttackPredictions.Count; i++)
        {
            AttackPrediction playerPrediction = playerAttackPredictions[i];
            AttackProbability attackProbability = playerPrediction.GetAttackProbability();

            if (!actionPicker.CanReactWithSpecialAttack(attackProbability))
            {
                continue;
            }

            if (specialAttackReactionService.TryReactToPrediction(attacker, attackerPlateIndex, playerPrediction))
            {
                reactedPredictionIndex = i;
                return true;
            }
        }

        return false;
    }
}
