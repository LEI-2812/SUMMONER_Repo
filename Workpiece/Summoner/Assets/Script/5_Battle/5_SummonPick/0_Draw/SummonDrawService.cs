using System.Collections.Generic;
using UnityEngine;

// 역할: 소환 후보 3개를 등급 확률에 따라 만든다.
public class SummonDrawService
{
    private const int DrawOptionCount = 3;
    private const int MaxAttemptMultiplier = 10;
    private const float LowRankMaxChance = 50f;
    private const float MediumRankMaxChance = 85f;

    public List<Summon> CreateDrawOptions(List<Summon> summons)
    {
        List<Summon> selectedSummons = new List<Summon>();
        if (summons == null || summons.Count == 0)
        {
            return selectedSummons;
        }

        int optionCount = Mathf.Min(DrawOptionCount, summons.Count);
        int attempts = 0;
        int maxAttempts = summons.Count * MaxAttemptMultiplier;

        while (selectedSummons.Count < optionCount && attempts < maxAttempts)
        {
            attempts++;
            Summon summon = SelectDrawOptionByRank(summons);
            if (summon != null && !selectedSummons.Contains(summon))
            {
                selectedSummons.Add(summon);
            }
        }

        FillRemainingDrawOptions(summons, selectedSummons, optionCount);
        return selectedSummons;
    }

    private void FillRemainingDrawOptions(
        List<Summon> summons,
        List<Summon> selectedSummons,
        int optionCount)
    {
        foreach (Summon summon in summons)
        {
            if (selectedSummons.Count >= optionCount)
            {
                return;
            }

            if (summon != null && !selectedSummons.Contains(summon))
            {
                selectedSummons.Add(summon);
            }
        }
    }

    private Summon SelectDrawOptionByRank(List<Summon> summons)
    {
        float randomValue = Random.Range(0f, 100f);

        Summon summon = null;
        if (randomValue <= LowRankMaxChance)
        {
            summon = SelectRandomSummonByRank(summons, SummonRank.Low);
        }
        else if (randomValue <= MediumRankMaxChance)
        {
            summon = SelectRandomSummonByRank(summons, SummonRank.Medium);
        }
        else
        {
            summon = SelectRandomSummonByRank(summons, SummonRank.High);
        }

        if (summon == null)
        {
            summon = SelectRandomSummonByRank(summons, SummonRank.Low)
                ?? SelectRandomSummonByRank(summons, SummonRank.Medium)
                ?? SelectRandomSummonByRank(summons, SummonRank.High);
        }

        return summon;
    }

    private Summon SelectRandomSummonByRank(List<Summon> summons, SummonRank rank)
    {
        List<Summon> availableSummons = new List<Summon>();

        foreach (Summon summon in summons)
        {
            if (summon != null && summon.GetDrawRank() == rank)
            {
                availableSummons.Add(summon);
            }
        }

        if (availableSummons.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSummons.Count);
            return availableSummons[randomIndex];
        }

        return null;
    }
}
