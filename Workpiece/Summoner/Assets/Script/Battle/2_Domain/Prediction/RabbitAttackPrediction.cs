using System.Collections;
using System.Collections.Generic;

// 역할: RabbitAttackPrediction의 책임을 정의한다.
public class RabbitAttackPrediction : IAttackPrediction
{
 

    public bool CanPredict(Summon summon)
    {
        return summon is Rabbit;
    }

    public AttackPredictionData GetAttackPrediction(Summon rabbit, PredictionBoardData board)
    {
        int rabbitPlateIndex = board.FindPlayerPlateIndex(rabbit);
        IReadOnlyList<IPlateState> playerPlates = board.PlayerPlates;
        IReadOnlyList<IPlateState> enemyPlates = board.EnemyPlates;
        AttackProbabilityData AttackProbabilityData = new AttackProbabilityData(50f, 50f);
        int attackIndex = GetClosestEnemyIndex(enemyPlates);
        if (!rabbit.TryGetFirstAvailableSpecialAttack(out AttackData specialAttack, out int specialAttackIndex))
        {
            return new AttackPredictionData(rabbit, rabbitPlateIndex, rabbit.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }

        int lowerHealthDifferenceIndex = GetIndexOfLowerHealthIfDifferenceOver30(playerPlates, rabbitPlateIndex);
        int allDownHealthIndex = GetIndexOfLowerHealthIfAllDown30(playerPlates);

        if (lowerHealthDifferenceIndex != -1)
        {
            attackIndex = lowerHealthDifferenceIndex;
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "토끼 소환수 중 한쪽 체력이 다른 쪽보다 30% 이상 낮음");
            return new AttackPredictionData(rabbit, rabbitPlateIndex, specialAttack, specialAttackIndex, playerPlates, attackIndex, AttackProbabilityData);
        }
        else if (allDownHealthIndex != -1)
        {
            attackIndex = allDownHealthIndex;
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "토끼 소환수의 체력이 모두 30% 이하");
            return new AttackPredictionData(rabbit, rabbitPlateIndex, specialAttack, specialAttackIndex, playerPlates, attackIndex, AttackProbabilityData);
        }

        int lowestHealthSummonIndex = GetIndexOfLowestHealthSummon(playerPlates);

        if (AllPlayerSummonOver70Percent(playerPlates))
        {
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "토끼 모든 플레이어 소환수 체력이 70% 이상");
            return new AttackPredictionData(rabbit, rabbitPlateIndex, rabbit.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }
        else
        {
            int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(rabbit, enemyPlates);

            if (normalAttackKillIndex != -1)
            {
                attackIndex = normalAttackKillIndex;
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "토끼 일반 공격으로 적 처치 가능");
                return new AttackPredictionData(rabbit, rabbitPlateIndex, rabbit.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
            }
        }

        attackIndex = lowestHealthSummonIndex;
        return new AttackPredictionData(rabbit, rabbitPlateIndex, specialAttack, specialAttackIndex, playerPlates, attackIndex, AttackProbabilityData);
    }




    public int GetIndexOfLowerHealthIfDifferenceOver30(IReadOnlyList<IPlateState> playerPlates,int rabbitIndex)
    {
        if (playerPlates.Count < 2) return -1;

        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (i == rabbitIndex) continue;

            Summon currentSummon = playerPlates[i].GetCurrentSummon();
            if (currentSummon == null) continue;

            for (int j = 0; j < playerPlates.Count; j++)
            {
                if (i == j || j == rabbitIndex) continue;

                Summon compareSummon = playerPlates[j].GetCurrentSummon();
                if (compareSummon == null) continue;

                double healthRatio = currentSummon.GetNowHP() / compareSummon.GetNowHP();

                if (healthRatio <= 0.7 && currentSummon.GetNowHP() < lowestHealth)
                {
                    lowestHealth = currentSummon.GetNowHP();
                    lowestHealthIndex = i;
                }
            }
        }

        return lowestHealthIndex;
    }


    public int GetIndexOfLowerHealthIfAllDown30(IReadOnlyList<IPlateState> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].GetCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.GetNowHP() / playerSummon.GetMaxHP();

                if (healthRatio > 0.3f)
                {
                    return -1;
                }

                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i;
                }
            }
        }

        return indexOfMinHealth;
    }

    public bool AllPlayerSummonOver70Percent(IReadOnlyList<IPlateState> playerPlates)
    {
        foreach (IPlateState plate in playerPlates)
        {
            Summon playerSummon = plate.GetCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.GetNowHP() / playerSummon.GetMaxHP();
                if (healthRatio < 0.7f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetIndexOfLowestHealthSummon(IReadOnlyList<IPlateState> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].GetCurrentSummon();
            if (summon != null)
            {
                double healthRatio = summon.GetNowHP() / summon.GetMaxHP();

                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i;
                }
            }
        }

        return indexOfMinHealth;
    }

    public int GetIndexOfNormalAttackCanKill(Summon rabbit, IReadOnlyList<IPlateState> enemyPlates)
    {
        int closestIndex = GetClosestEnemyIndex(enemyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnemySummon = enemyPlates[closestIndex].GetCurrentSummon();
            if (closestEnemySummon != null && rabbit.GetAttackPower() >= closestEnemySummon.GetNowHP())
            {
                return closestIndex;
            }
        }

        return -1;
    }


    public int GetClosestEnemyIndex(IReadOnlyList<IPlateState> enemyPlates)
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].GetCurrentSummon();
            if (enemySummon != null)
            {
                return i;
            }
        }
        return -1;
    }




    private AttackProbabilityData AdjustAttackProbabilities(AttackProbabilityData currentProbabilities, float AttackChange, bool isNormalAttack, string reason)
    {
        if (isNormalAttack)
        {
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
        }
        else
        {
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
        }
        return currentProbabilities.AddPredictionReason(reason);
    }
}
