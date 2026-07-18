using System.Collections.Generic;

public class BattleBoardData
{
    public BattleBoardData(
        IReadOnlyList<PlateData> playerPlates,
        IReadOnlyList<PlateData> enemyPlates)
    {
        PlayerPlates = playerPlates;
        EnemyPlates = enemyPlates;
    }

    public IReadOnlyList<PlateData> PlayerPlates { get; }
    public IReadOnlyList<PlateData> EnemyPlates { get; }

    public IReadOnlyList<PlateData> GetTargetPlates(bool isPlayerAttack, bool targetsOwnPlates)
    {
        return isPlayerAttack == targetsOwnPlates
            ? PlayerPlates
            : EnemyPlates;
    }

    public bool AreEnemyPlatesClear()
    {
        return ArePlatesClear(EnemyPlates);
    }

    public bool ArePlayerPlatesClear()
    {
        return ArePlatesClear(PlayerPlates);
    }

    public int FindClosestPlayerPlateIndex(Summon exceptSummon = null)
    {
        for (int index = 0; index < PlayerPlates.Count; index++)
        {
            Summon summon = PlayerPlates[index].CurrentSummon;
            if (summon != null && summon != exceptSummon)
            {
                return index;
            }
        }

        return -1;
    }

    public int FindLowestHealthEnemyPlateIndex()
    {
        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int index = 0; index < EnemyPlates.Count; index++)
        {
            Summon summon = EnemyPlates[index].CurrentSummon;
            if (summon != null && summon.GetNowHP() < lowestHealth)
            {
                lowestHealth = summon.GetNowHP();
                lowestHealthIndex = index;
            }
        }

        return lowestHealthIndex;
    }

    public int FindFirstEmptyPlayerPlateIndex()
    {
        for (int index = 0; index < PlayerPlates.Count; index++)
        {
            if (PlayerPlates[index].CurrentSummon == null)
            {
                return index;
            }
        }

        return -1;
    }

    private static bool ArePlatesClear(IReadOnlyList<PlateData> plates)
    {
        if (plates == null)
        {
            return true;
        }

        for (int index = 0; index < plates.Count; index++)
        {
            if (plates[index].CurrentSummon != null)
            {
                return false;
            }
        }

        return true;
    }
}
