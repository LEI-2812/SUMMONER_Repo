using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// 역할: WolfAttackPrediction의 책임을 정의한다.
public class WolfAttackPrediction : IAttackPrediction
{

    public bool CanPredict(Summon summon)
    {
        return summon is Wolf;
    }

    public AttackPredictionData GetAttackPrediction(Summon wolf, PredictionBoardData board)
    {
        int wolfPlateIndex = board.FindPlayerPlateIndex(wolf);
        IReadOnlyList<IPlateState> playerPlates = board.PlayerPlates;
        IReadOnlyList<IPlateState> enemyPlates = board.EnemyPlates;
        AttackProbabilityData AttackProbabilityData = new AttackProbabilityData(50f, 50f);
        int attackIndex = GetClosestEnemyIndex(enemyPlates);

        if (IsEnemyCountTwoOrMore(enemyPlates))
        {
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "늑대 적이 2마리 이상");
            if (AllEnemyHealthOver50(enemyPlates))
            {
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "늑대 적 몬스터 체력이 모두 50% 이상");
            }
            else if (AllEnemyHealthDown50(enemyPlates))
            {
                if (HasSpecificAvailableSpecialAttack(wolf, wolfPlateIndex, playerPlates))
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "늑대 적 몬스터 체력이 모두 50% 이하이고 공격 가능한 소환수가 존재");
                }
            }
            else
            {
                int lowestHealthDifferenceIndex = IsEnemyHealthDifferenceOver30(enemyPlates);

                if (lowestHealthDifferenceIndex != -1 && IsLowestHealthEnemyClosest(wolf, enemyPlates, lowestHealthDifferenceIndex))
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 20f, true, "늑대 가장 약한 쪽을 공격하는 인덱스가 동일");
                }
                else if (lowestHealthDifferenceIndex != -1)
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "늑대 가장 약한 적이 가장 가까운 대상이 아님");
                }
            }
        }
        else if (IsEnemyCountOne(enemyPlates))
        {
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "늑대 적이 1마리");

            int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(wolf, enemyPlates);

            if (normalAttackKillIndex != -1)
            {
                attackIndex = normalAttackKillIndex;
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "늑대 일반 공격으로 적 처치 가능");
            }
            else
            {
                if (GetMostDamageAttack(wolf, enemyPlates) == AttackType.NormalAttack)
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 5f, true, "늑대 일반 공격이 더 많은 피해를 줌");
                }
                else
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 5f, false, "늑대 특수 공격이 더 많은 피해를 줌");
                }
            }
        }

        if (!wolf.TryGetFirstAvailableSpecialAttack(out AttackData specialAttack, out int specialAttackIndex))
        {
            return new AttackPredictionData(wolf, wolfPlateIndex, wolf.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }

        return new AttackPredictionData(wolf, wolfPlateIndex, specialAttack, specialAttackIndex, enemyPlates, attackIndex, AttackProbabilityData);
    }


    public bool AllEnemyHealthOver50(IReadOnlyList<IPlateState> enemyPlates)
    {
        bool hasAliveEnemy = false;

        foreach (IPlateState plate in enemyPlates)
        {
            Summon enemySummon = plate.GetCurrentSummon();

            if (enemySummon != null)
            {
                hasAliveEnemy = true;
                double healthRatio = (double)enemySummon.GetNowHP() / enemySummon.GetMaxHP();

                if (healthRatio < 0.5) // 체력 비율이 50% 미만인 경우
                {
                    return false;
                }
            }
        }

        return hasAliveEnemy;
    }

    public bool AllEnemyHealthDown50(IReadOnlyList<IPlateState> enemyPlates)
    {
        bool hasAliveEnemy = false;

        foreach (IPlateState plate in enemyPlates)
        {
            Summon enemySummon = plate.GetCurrentSummon();

            if (enemySummon != null)
            {
                hasAliveEnemy = true;
                if (enemySummon.GetNowHP() / enemySummon.GetMaxHP() > 0.5f)
                {
                    return false;
                }
            }
        }

        return hasAliveEnemy;
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



    public bool IsEnemyCountTwoOrMore(IReadOnlyList<IPlateState> enemyPlates)
    {
        int enemyCount = 0;

        foreach (IPlateState plate in enemyPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                enemyCount++;
                if (enemyCount >= 2) return true;
            }
        }

        return false;
    }

    public bool IsEnemyCountOne(IReadOnlyList<IPlateState> enemyPlates)
    {
        int enemyCount = 0;

        foreach (IPlateState plate in enemyPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                enemyCount++;
                if (enemyCount > 1) return false;
            }
        }

        return enemyCount == 1;
    }


    public bool HasSpecificAvailableSpecialAttack(Summon self, int wolfPlateIndex, IReadOnlyList<IPlateState> playerPlates)
    {
        StatusType[] specificStatuses = new StatusType[] { StatusType.None, StatusType.Burn, StatusType.Poison, StatusType.LifeDrain };

        if (wolfPlateIndex < 2)
        {
            for(int i=wolfPlateIndex+1; i< playerPlates.Count; i++)
            {
                Summon playerSummon = playerPlates[i].GetCurrentSummon();
                if (playerSummon != null && playerSummon != self)
                {
                    AttackData[] availableSpecialAttacks = playerSummon.GetAvailableSpecialAttacks();

                    foreach (AttackData specialAttack in availableSpecialAttacks)
                    {
                        if (specialAttack != null && specificStatuses.Contains(specialAttack.GetStatusType()))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }


    public AttackType GetMostDamageAttack(Summon attackingSummon, IReadOnlyList<IPlateState> enemyPlates)
    {
        double normalAttackDamage = attackingSummon.GetAttackPower();


        AttackData[] availableSpecialAttacks = attackingSummon.GetAvailableSpecialAttacks();

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

    public int GetIndexOfNormalAttackCanKill(Summon wolf, IReadOnlyList<IPlateState> enemyPlates)
    {
        int closestIndex = GetClosestEnemyIndex(enemyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnemySummon = enemyPlates[closestIndex].GetCurrentSummon();
            if (closestEnemySummon != null && wolf.GetAttackPower() >= closestEnemySummon.GetNowHP())
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
