using System.Collections.Generic;

// 역할: 소환수에 맞는 공격 예측 알고리즘을 선택한다.
public class PlayerAttackPrediction
{
    private readonly IReadOnlyList<IAttackPrediction> attackPredictions;

    public PlayerAttackPrediction()
    {
        attackPredictions = new IAttackPrediction[]
        {
            new CatAttackPrediction(),
            new EagleAttackPrediction(),
            new FoxAttackPrediction(),
            new RabbitAttackPrediction(),
            new SnakeAttackPrediction(),
            new WolfAttackPrediction()
        };
    }

    public List<AttackPredictionData> GetPlayerAttackPredictionList(PredictionBoardData board)
    {
        var playerPredictions = new List<AttackPredictionData>();

        foreach (PredictionPlateData plate in board.PlayerPlates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon == null)
            {
                continue;
            }

            AttackData[] availableSpecialAttacks = summon.GetAvailableSpecialAttacks();
            bool hasUsableSpecialAttack = availableSpecialAttacks != null
                && availableSpecialAttacks.Length > 0;

            if (!hasUsableSpecialAttack)
            {
                playerPredictions.Add(CreateNormalAttackPrediction(summon, plate, board));
                continue;
            }

            for (int index = 0; index < attackPredictions.Count; index++)
            {
                IAttackPrediction prediction = attackPredictions[index];
                if (!prediction.CanPredict(summon))
                {
                    continue;
                }

                AttackPredictionData result = prediction.GetAttackPrediction(summon, board);
                if (result != null)
                {
                    playerPredictions.Add(result);
                }

                break;
            }
        }

        return playerPredictions;
    }

    private static AttackPredictionData CreateNormalAttackPrediction(
        Summon summon,
        PredictionPlateData plate,
        PredictionBoardData board)
    {
        int attackIndex = GetClosestEnemyPlateIndexExcept(board.EnemyPlates, summon);
        return new AttackPredictionData(
            summon,
            plate.GetPlateIndex(),
            summon.GetAttackStrategy(),
            0,
            board.EnemyPlates,
            attackIndex,
            new AttackProbabilityData(
                100f,
                0f,
                "사용 가능한 특수 공격이 없어 일반 공격 사용"));
    }

    private static int GetClosestEnemyPlateIndexExcept(
        IReadOnlyList<PredictionPlateData> enemyPlates,
        Summon excludedSummon)
    {
        for (int index = 0; index < enemyPlates.Count; index++)
        {
            Summon summon = enemyPlates[index].GetCurrentSummon();
            if (summon != null && summon != excludedSummon)
            {
                return index;
            }
        }

        return -1;
    }
}
