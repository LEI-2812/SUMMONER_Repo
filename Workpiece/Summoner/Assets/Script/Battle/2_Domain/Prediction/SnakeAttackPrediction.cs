using System.Collections;
using System.Collections.Generic;

// 역할: SnakeAttackPrediction의 책임을 정의한다.
public class SnakeAttackPrediction : IAttackPrediction
{

    public bool CanPredict(Summon summon)
    {
        return summon is Snake;
    }

    public AttackPredictionData GetAttackPrediction(Summon snake, PredictionBoardData board)
    {
        int snakePlateIndex = board.FindPlayerPlateIndex(snake);
        IReadOnlyList<IPlateState> playerPlates = board.PlayerPlates;
        IReadOnlyList<IPlateState> enemyPlates = board.EnemyPlates;
        AttackProbabilityData AttackProbabilityData = new AttackProbabilityData(50f, 50f);
        int attackIndex = GetClosestEnemyIndex(enemyPlates);

        if (IsEnemyAlreadyPoisoned(enemyPlates) || !CanUseSpecialAttack(snake))
        {
            AttackProbabilityData = new AttackProbabilityData(
                100f,
                0f,
                "뱀 적이 이미 중독 상태이거나 특수 공격 사용 불가");
            return new AttackPredictionData(snake, snakePlateIndex, snake.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }

        if (IsEnemyCountOverTwo(enemyPlates))
        {
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "뱀 적이 2마리 이상");
            if (AllEnemyHealthOver50(enemyPlates))
            {
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "뱀 적의 체력이 모두 50% 이상");
            }
            else
            {
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "뱀 적의 체력이 모두 50%가 아님");
            }
            if (HasMonsterWithMoreThan3Attacks(enemyPlates))
            {
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "뱀 적 몬스터 중 공격력 3 이상인 대상 존재");
            }
        }
        else
        {
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "뱀 적이 2마리 미만");
            if (AllEnemyHealthOver50(enemyPlates))
            {
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "뱀 적의 체력이 50% 이상");
            }
            if (GetIndexOfNormalAttackCanKill(snake, enemyPlates) != -1)
            {
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "뱀 일반 공격으로 적 처치 가능");
            }
        }

        if (!snake.TryGetFirstAvailableSpecialAttack(out AttackData specialAttack, out int specialAttackIndex))
        {
            return new AttackPredictionData(snake, snakePlateIndex, snake.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }

        return new AttackPredictionData(snake, snakePlateIndex, specialAttack, specialAttackIndex, enemyPlates, attackIndex, AttackProbabilityData);
    }



    public bool IsEnemyAlreadyPoisoned(IReadOnlyList<IPlateState> enemyPlates)
    {
        foreach (IPlateState plate in enemyPlates)
        {
            Summon enemySummon = plate.GetCurrentSummon();
            if (enemySummon != null && enemySummon.GetAllStatusTypes().Contains(StatusType.Poison))
            {
                return true;
            }
        }
        return false;
    }


    public bool CanUseSpecialAttack(Summon snake)
    {
        var availableSpecialAttacks = snake.GetAvailableSpecialAttacks();
        return availableSpecialAttacks.Length > 0;
    }



    public bool AllEnemyHealthOver50(IReadOnlyList<IPlateState> enemyPlates)
    {
        foreach (IPlateState plate in enemyPlates)
        {
            Summon enemySummon = plate.GetCurrentSummon();
            if (enemySummon != null)
            {
                double healthRatio = enemySummon.GetNowHP() / enemySummon.GetMaxHP();
                if (healthRatio < 0.5)
                {
                    return false;
                }
            }
        }
        return true;
    }



    public bool HasMonsterWithMoreThan3Attacks(IReadOnlyList<IPlateState> enemyPlates)
    {
        foreach (IPlateState plate in enemyPlates)
        {
            Summon enemySummon = plate.GetCurrentSummon();
            if (enemySummon != null && enemySummon.GetSpecialAttackCount() >= 3)
            {
                return true;
            }
        }
        return false;
    }


    public bool IsEnemyCountOverTwo(IReadOnlyList<IPlateState> enemyPlates)
    {
        int count = 0;
        foreach (IPlateState plate in enemyPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                count++;
                if (count >= 2) return true;
            }
        }
        return false;
    }

    public bool IsEnemyCountOnlyOne(IReadOnlyList<IPlateState> enemyPlates)
    {
        foreach (IPlateState plate in enemyPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                return true;
            }
        }
        return false;
    }


    public int GetIndexOfNormalAttackCanKill(Summon snake, IReadOnlyList<IPlateState> enemyPlates)
    {
        int closestIndex = GetClosestEnemyIndex(enemyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnemySummon = enemyPlates[closestIndex].GetCurrentSummon();
            if (closestEnemySummon != null && snake.GetAttackPower() >= closestEnemySummon.GetNowHP())
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
