using System.Collections.Generic;

// 역할: PredictionBoardData의 책임을 정의한다.
public class PredictionBoardData
{
    public IReadOnlyList<PredictionPlateData> PlayerPlates { get; }
    public IReadOnlyList<PredictionPlateData> EnemyPlates { get; }

    public PredictionBoardData(
        BattleBoardData board)
    {
        PlayerPlates = CollectPlayerPlates(board.PlayerPlates);
        EnemyPlates = CollectEnemyPlatesWithPredictedStatus(board.EnemyPlates);
    }

    public int FindPlayerPlateIndex(Summon summon)
    {
        for (int index = 0; index < PlayerPlates.Count; index++)
        {
            if (PlayerPlates[index].GetCurrentSummon() == summon)
            {
                return PlayerPlates[index].GetPlateIndex();
            }
        }

        return -1;
    }

    private List<PredictionPlateData> CollectPlayerPlates(IReadOnlyList<PlateData> playerPlates)
    {
        List<PredictionPlateData> playerPlateStates = new List<PredictionPlateData>();

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].GetCurrentSummon();
            if (summon == null)
            {
                continue;
            }

            playerPlateStates.Add(new PredictionPlateData(i, summon));
        }

        return playerPlateStates;
    }

    private List<PredictionPlateData> CollectEnemyPlatesWithPredictedStatus(IReadOnlyList<PlateData> sourceEnemyPlates)
    {
        List<PredictionPlateData> adjustedEnemyPlates = new List<PredictionPlateData>();

        for (int i = 0; i < sourceEnemyPlates.Count; i++)
        {
            Summon originSummon = sourceEnemyPlates[i].GetCurrentSummon();
            if (originSummon == null)
            {
                adjustedEnemyPlates.Add(new PredictionPlateData(i, null));
                continue;
            }

            Summon clonedSummon = originSummon.Clone();
            Summon adjustedSummon = ApplyEnemyStatus(clonedSummon);
            adjustedEnemyPlates.Add(new PredictionPlateData(i, adjustedSummon));
        }

        return adjustedEnemyPlates;
    }

    private Summon ApplyEnemyStatus(Summon clonedSummon)
    {
        foreach (StatusData status in clonedSummon.GetActiveStatuses())
        {
            if (!IsDamagePredictionStatus(status))
            {
                continue;
            }

            clonedSummon.SetNowHP(clonedSummon.GetNowHP() - status.damagePerTurn);
            if (clonedSummon.GetNowHP() <= 0)
            {
                return null;
            }
        }

        return clonedSummon;
    }

    private bool IsDamagePredictionStatus(StatusData status)
    {
        if (status == null)
        {
            return false;
        }

        return status.statusType == StatusType.Poison
            || status.statusType == StatusType.Burn
            || status.statusType == StatusType.LifeDrain;
    }
}
