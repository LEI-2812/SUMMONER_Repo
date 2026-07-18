using System.Collections;
using System.Collections.Generic;

// 역할: FoxAttackPrediction의 책임을 정의한다.
public class FoxAttackPrediction : IAttackPrediction
{

    public bool CanPredict(Summon summon)
    {
        return summon is Fox;
    }

    public AttackPredictionData GetAttackPrediction(Summon fox, PredictionBoardData board)
    {
        int foxPlateIndex = board.FindPlayerPlateIndex(fox);
        IReadOnlyList<IPlateState> playerPlates = board.PlayerPlates;
        IReadOnlyList<IPlateState> enemyPlates = board.EnemyPlates;
        AttackProbabilityData AttackProbabilityData = new AttackProbabilityData(50f, 50f);
        int attackIndex = GetClosestEnemyIndex(enemyPlates);
        IReadOnlyList<IPlateState> targetPlate = enemyPlates;
        if (!fox.TryGetFirstAvailableSpecialAttack(out AttackData specialAttack, out int specialAttackIndex))
        {
            return new AttackPredictionData(fox, foxPlateIndex, fox.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }

        int cursedSummonIndex = GetIndexOfSummonWithCurseStatus(playerPlates);

        if (cursedSummonIndex != -1)
        {
            AttackProbabilityData = new AttackProbabilityData(
                0f,
                100f,
                "여우 저주 상태인 아군에게 특수 공격 사용");
            return new AttackPredictionData(fox, foxPlateIndex, specialAttack, specialAttackIndex, playerPlates, cursedSummonIndex, AttackProbabilityData);
        }

        else if (IsTwoOrMoreEnemies(enemyPlates))
        {
            if (AllEnemiesHealthOver50(enemyPlates))
            {
                if (AllSummonsLowOrMediumRank(playerPlates))
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "여우 아군 등급이 모두 하급 또는 중급");
                    targetPlate = playerPlates;
                    attackIndex = foxPlateIndex;
                }
            }
            else
            {
                int under30Index = IsAnyEnemyHealthDown30Percent(enemyPlates);

                if (under30Index != -1)
                {
                    int normalAttack30PerKillIndex = GetIndexOfNormalAttack30PerCanKill(fox, enemyPlates, under30Index);

                    if (normalAttack30PerKillIndex != -1)
                    {
                        attackIndex = under30Index;
                        AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "여우 체력 30% 이하 적을 일반 공격으로 처치 가능");
                    }
                }
            }
        }
        else if (IsOnlyOneEnemy(enemyPlates))
        {
            if (IsAnyEnemyHealthOver70Percent(enemyPlates))
            {
                if (AllSummonsLowOrMediumRank(playerPlates))
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "여우 적 1마리이고 아군 등급이 모두 하급 또는 중급");
                    targetPlate = playerPlates;
                    attackIndex = foxPlateIndex;
                }
            }
            else
            {
                int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(fox, enemyPlates);

                if (normalAttackKillIndex != -1)
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "여우 일반 공격으로 적 처치 가능");
                }
            }
        }
        else
        {
            targetPlate = playerPlates;
            attackIndex = GetIndexOfHighestAttackPower(playerPlates);
        }

        return new AttackPredictionData(fox, foxPlateIndex, specialAttack, specialAttackIndex, targetPlate, attackIndex, AttackProbabilityData);
    }

    public int GetIndexOfSummonWithCurseStatus(IReadOnlyList<IPlateState> playerPlates)
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].GetCurrentSummon();
            if (playerSummon != null && playerSummon.IsCursed())
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsTwoOrMoreEnemies(IReadOnlyList<IPlateState> enemyPlates)
    {
        int count = 0;
        foreach (IPlateState plate in enemyPlates)
        {
            if (plate.GetCurrentSummon() != null) count++;
        }
        return count >= 2;
    }


    public bool IsOnlyOneEnemy(IReadOnlyList<IPlateState> enemyPlates)
    {
        int count = 0;
        foreach (IPlateState plate in enemyPlates)
        {
            if (plate.GetCurrentSummon() != null) count++;
        }
        return count == 1;
    }

    public bool AllEnemiesHealthOver50(IReadOnlyList<IPlateState> enemyPlates)
    {
        foreach (IPlateState plate in enemyPlates)
        {
            Summon enemySummon = plate.GetCurrentSummon();
            if (enemySummon != null && enemySummon.GetNowHP() / enemySummon.GetMaxHP() < 0.5)
            {
                return false;
            }
        }
        return true;
    }

    public bool AllSummonsLowOrMediumRank(IReadOnlyList<IPlateState> playerPlates)
    {
        foreach (IPlateState plate in playerPlates)
        {
            Summon playerSummon = plate.GetCurrentSummon();
            if (playerSummon != null && playerSummon.GetSummonRank() == SummonRank.High)
            {
                return false;
            }
        }
        return true;
    }

    public int IsAnyEnemyHealthDown30Percent(IReadOnlyList<IPlateState> enemyPlates)
    {
        int index = -1;
        int count = 0;

        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].GetCurrentSummon();
            if (enemySummon != null && enemySummon.GetNowHP() / enemySummon.GetMaxHP() < 0.3)
            {
                count++;
                index = i;

                if (count > 1) return -1;
            }
        }

        return count == 1 ? index : -1;
    }

    public bool IsAnyEnemyHealthOver70Percent(IReadOnlyList<IPlateState> enemyPlates)
    {
        foreach (IPlateState plate in enemyPlates)
        {
            Summon enemySummon = plate.GetCurrentSummon();
            if (enemySummon != null && enemySummon.GetNowHP() / enemySummon.GetMaxHP() > 0.7)
            {
                return true;
            }
        }
        return false;
    }

    public int GetIndexOfHighestAttackPower(IReadOnlyList<IPlateState> playerPlates)
    {
        int highestAttackIndex = -1;
        double highestAttackPower = double.MinValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].GetCurrentSummon();
            if (summon != null)
            {
                double attackPower = summon.GetAttackPower();
                if (attackPower > highestAttackPower)
                {
                    highestAttackPower = attackPower;
                    highestAttackIndex = i;
                }
            }
        }

        return highestAttackIndex;
    }



    public int GetIndexOfNormalAttack30PerCanKill(Summon fox, IReadOnlyList<IPlateState> enemyPlates, int under30Index)
    {
        int closestIndex = GetClosestEnemyIndex(enemyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnemySummon = enemyPlates[closestIndex].GetCurrentSummon();
            if (closestEnemySummon != null && fox.GetAttackPower() >= closestEnemySummon.GetNowHP())
            {
                if(closestIndex == under30Index)
                    return under30Index;
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
    public int GetIndexOfNormalAttackCanKill(Summon fox, IReadOnlyList<IPlateState> enemyPlates)
    {
        int closestIndex = GetClosestEnemyIndex(enemyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnemySummon = enemyPlates[closestIndex].GetCurrentSummon();
            if (closestEnemySummon != null && fox.GetAttackPower() >= closestEnemySummon.GetNowHP())
            {
                return closestIndex;
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
