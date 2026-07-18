using System;
using System.Collections;
using System.Collections.Generic;

// 역할: EagleAttackPrediction의 책임을 정의한다.
public class EagleAttackPrediction : IAttackPrediction
{

    public bool CanPredict(Summon summon)
    {
        return summon is Eagle;
    }


    public AttackPredictionData GetAttackPrediction(Summon eagle, PredictionBoardData board)
    {
        int eaglePlateIndex = board.FindPlayerPlateIndex(eagle);
        IReadOnlyList<IPlateState> playerPlates = board.PlayerPlates;
        IReadOnlyList<IPlateState> enemyPlates = board.EnemyPlates;
        AttackProbabilityData AttackProbabilityData = new AttackProbabilityData(50f, 50f);
        int attackIndex = GetClosestEnemyIndex(enemyPlates);

        if (IsTwoOrMoreEnemies(enemyPlates))
        {
            int lowestHealthDifferenceIndex = IsEnemyHealthDifferenceOver30(enemyPlates);

            if (lowestHealthDifferenceIndex != -1)
            {
                int normalAttackLowestIndex = CanNormalAttack(eagle, enemyPlates, lowestHealthDifferenceIndex);

                if (normalAttackLowestIndex != -1)
                {
                    attackIndex = normalAttackLowestIndex;
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "독수리 체력 차이가 큰 적을 일반 공격으로 처치 가능");
                }
                else
                {
                    attackIndex = lowestHealthDifferenceIndex;
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "독수리 체력 차이가 큰 적에게 특수 공격 사용");
                }
            }
            else
            {
                int healthWithin10PercentIndex = AreEnemyHealthWithin10Percent(eagle, enemyPlates);

                if (healthWithin10PercentIndex != -1)
                {
                    attackIndex = healthWithin10PercentIndex;
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 20f, false, "독수리 적들의 체력 차이가 10% 이내");
                }
            }
        }
        else if (IsOnlyOneEnemy(enemyPlates))
        {
            int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(eagle, enemyPlates);

            if (normalAttackKillIndex != -1)
            {
                attackIndex = normalAttackKillIndex;
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "독수리 일반 공격으로 적 처치 가능");
            }
            else
            {
                int specialAttackKillIndex = GetSpecialAttackKillIndex(eagle, enemyPlates);

                if (specialAttackKillIndex != -1)
                {
                    attackIndex = specialAttackKillIndex;
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "독수리 특수 공격으로 적 처치 가능");
                }
                else
                {
                    if (GetTypeOfMoreAttackDamage(eagle, enemyPlates) == AttackType.NormalAttack)
                    {
                        AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 5f, true, "독수리 일반 공격이 더 많은 피해를 줌");
                    }
                    else
                    {
                        AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 5f, false, "독수리 특수 공격이 더 많은 피해를 줌");
                    }
                }
            }
        }

        if (!eagle.TryGetFirstAvailableSpecialAttack(out AttackData specialAttack, out int specialAttackIndex))
        {
            return new AttackPredictionData(eagle, eaglePlateIndex, eagle.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }

        return new AttackPredictionData(eagle, eaglePlateIndex, specialAttack, specialAttackIndex, enemyPlates, attackIndex, AttackProbabilityData);
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


    public int IsEnemyHealthDifferenceOver30(IReadOnlyList<IPlateState> enemyPlates)
    {
        if (enemyPlates.Count < 2) return -1;

        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon currentSummon = enemyPlates[i].GetCurrentSummon();
            if (currentSummon == null) continue;

            for (int j = 0; j < enemyPlates.Count; j++)
            {
                if (i == j) continue;

                Summon compareSummon = enemyPlates[j].GetCurrentSummon();
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


    public bool IsLowestHealthEnemyClosest(Summon attackingSummon, IReadOnlyList<IPlateState> enemyPlates, int lowestIndex)
    {
        if (enemyPlates.Count < 2) return false;

        int closestIndex = GetClosestEnemyIndex(attackingSummon, enemyPlates);

        if (closestIndex == lowestIndex)
            return true;

        return false;
    }

    public int GetClosestEnemyIndex(Summon attackingSummon, IReadOnlyList<IPlateState> enemyPlates)
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].GetCurrentSummon();
            if (enemySummon != null && enemySummon != attackingSummon)
            {
                return i;
            }
        }

        return -1;
    }


    public int AreEnemyHealthWithin10Percent(Summon eagle,IReadOnlyList<IPlateState> enemyPlates)
    {
        if (enemyPlates.Count < 2) return -1;

        bool hasAvailableSpecialAttack = false;
        for (int i = 0; i < eagle.GetSpecialAttackStrategy().Length; i++)
        {
            if (!eagle.IsSpecialAttackCool(eagle.GetSpecialAttackStrategy()[i]))
            {
                hasAvailableSpecialAttack = true;
                break;
            }
        }

        if (!hasAvailableSpecialAttack)
        {
            return -1;
        }

        int maxHealthIndex = -1;
        double highestHealth = double.MinValue;

        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon currentSummon = enemyPlates[i].GetCurrentSummon();
            if (currentSummon == null) continue;

            double currentHealth = currentSummon.GetNowHP();

            if (currentHealth > highestHealth)
            {
                highestHealth = currentHealth;
                maxHealthIndex = i;
            }
        }

        if (maxHealthIndex == -1) return -1;

        int resultIndex = maxHealthIndex;
        double maxHealth = highestHealth;

        for (int i = 0; i < enemyPlates.Count; i++)
        {
            if (i == maxHealthIndex) continue;

            Summon compareSummon = enemyPlates[i].GetCurrentSummon();
            if (compareSummon == null) continue;

            double healthDifference = Math.Abs(maxHealth - compareSummon.GetNowHP());

            if (healthDifference > maxHealth * 0.1)
            {
                resultIndex = -1;
                break;
            }
        }

        return resultIndex;
    }

    private int CanNormalAttack(Summon attackingSummon ,IReadOnlyList<IPlateState> enemyPlates , int lowestIndex)
    {
        if (enemyPlates.Count < 2) return -1;

        int closestIndex = GetClosestEnemyIndex(attackingSummon, enemyPlates);

        if (closestIndex == lowestIndex)
            return closestIndex;

        return -1;
    }


    public int GetIndexOfMostHealthEnemy(IReadOnlyList<IPlateState> enemyPlates)
    {
        int maxHealthIndex = -1;
        double maxHealth = double.MinValue;

        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].GetCurrentSummon();
            if (enemySummon != null)
            {
                double currentHealth = enemySummon.GetNowHP();
                if (currentHealth > maxHealth)
                {
                    maxHealth = currentHealth;
                    maxHealthIndex = i;
                }
            }
        }

        return maxHealthIndex;
    }

    public AttackType GetTypeOfMoreAttackDamage(Summon eagle, IReadOnlyList<IPlateState> enemyPlates)
    {
        double normalAttackDamage = eagle.GetAttackPower();

        AttackData[] availableSpecialAttacks = eagle.GetAvailableSpecialAttacks();

        foreach (AttackData specialAttack in availableSpecialAttacks)
        {
            double totalSpecialAttackDamage = 0;

            foreach (IPlateState plate in enemyPlates)
            {
                Summon enemySummon = plate.GetCurrentSummon();
                if (enemySummon != null)
                {
                    totalSpecialAttackDamage += specialAttack.GetSpecialDamage();
                }
            }

            if (totalSpecialAttackDamage > normalAttackDamage)
            {
                return AttackType.SpecialAttack;
            }
        }

        return AttackType.NormalAttack;
    }


    public int GetSpecialAttackKillIndex(Summon eagle, IReadOnlyList<IPlateState> enemyPlates)
    {
        bool hasAvailableSpecialAttack = false;
        for (int i = 0; i < eagle.GetSpecialAttackStrategy().Length; i++)
        {
            if (!eagle.IsSpecialAttackCool(eagle.GetSpecialAttackStrategy()[i]))
            {
                hasAvailableSpecialAttack = true;
                break;
            }
        }

        if (!hasAvailableSpecialAttack)
        {
            return -1;
        }


        for (int i=0; i< eagle.GetSpecialAttackStrategy().Length; i++)
        {
            if (eagle.IsSpecialAttackCool(eagle.GetSpecialAttackStrategy()[i]))
            {
                continue;
            }
            for (int ii = 0; ii < enemyPlates.Count; ii++)
            {
                Summon enemySummon = enemyPlates[ii].GetCurrentSummon();
                if (enemySummon != null && eagle.GetSpecialAttackStrategy()[i].GetSpecialDamage() >= enemySummon.GetNowHP())
                {
                    return ii;
                }
            }
        }
        return -1;
    }

    public int GetIndexOfNormalAttackCanKill(Summon eagle, IReadOnlyList<IPlateState> enemyPlates)
    {
        int closestIndex = GetClosestEnemyIndex(enemyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnemySummon = enemyPlates[closestIndex].GetCurrentSummon();
            if (closestEnemySummon != null && eagle.GetAttackPower() >= closestEnemySummon.GetNowHP())
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
