using System.Collections.Generic;

// 역할: 플레이트 목록에서 소환수와 타겟 인덱스를 조회한다.
public class PlateQueryService
{
    public List<Summon> GetSummonsFromPlates(List<Plate> targetPlates)
    {
        List<Summon> summons = new List<Summon>();

        if (targetPlates == null)
        {
            return summons;
        }

        foreach (Plate plate in targetPlates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon != null)
            {
                summons.Add(summon);
            }
        }

        return summons;
    }

    public bool ArePlatesClear(List<Plate> targetPlates)
    {
        if (targetPlates == null)
        {
            return true;
        }

        foreach (Plate plate in targetPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                return false;
            }
        }

        return true;
    }

    public int FindClosestOccupiedPlateIndex(List<Plate> targetPlates, Summon exceptSummon)
    {
        if (targetPlates == null)
        {
            return -1;
        }

        for (int i = 0; i < targetPlates.Count; i++)
        {
            Summon currentSummon = targetPlates[i].GetCurrentSummon();
            if (currentSummon != null && currentSummon != exceptSummon)
            {
                return i;
            }
        }

        return -1;
    }

    public int CountSummonsOnPlates(List<Plate> targetPlates)
    {
        if (targetPlates == null)
        {
            return 0;
        }

        int summonCount = 0;

        foreach (Plate plate in targetPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                summonCount++;
            }
        }

        return summonCount;
    }

    public int FindLowestHealthPlateIndex(List<Plate> targetPlates)
    {
        if (targetPlates == null)
        {
            return -1;
        }

        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < targetPlates.Count; i++)
        {
            Summon currentSummon = targetPlates[i].GetCurrentSummon();
            if (currentSummon != null)
            {
                double currentHealth = currentSummon.GetNowHP();
                if (currentHealth < lowestHealth)
                {
                    lowestHealth = currentHealth;
                    lowestHealthIndex = i;
                }
            }
        }

        return lowestHealthIndex;
    }

    public int FindFirstEmptyPlateIndex(List<Plate> targetPlates)
    {
        if (targetPlates == null)
        {
            return -1;
        }

        for (int i = 0; i < targetPlates.Count; i++)
        {
            Plate plate = targetPlates[i];
            if (plate != null && !plate.GetIsInSummon())
            {
                return i;
            }
        }

        return -1;
    }
}
