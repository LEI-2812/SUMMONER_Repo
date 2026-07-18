using System.Collections.Generic;

// 역할: CatAttackPrediction의 책임을 정의한다.
public class CatAttackPrediction : IAttackPrediction
{
    public bool CanPredict(Summon summon)
    {
        return summon is Cat;
    }

    public AttackPredictionData GetAttackPrediction(Summon cat, PredictionBoardData board)
    {
        int catPlateIndex = board.FindPlayerPlateIndex(cat);
        IReadOnlyList<IPlateState> playerPlates = board.PlayerPlates;
        IReadOnlyList<IPlateState> enemyPlates = board.EnemyPlates;
        AttackProbabilityData AttackProbabilityData = new AttackProbabilityData(50f, 50f);
        int attackIndex = GetClosestEnemyIndex(enemyPlates);

        int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(cat, enemyPlates);
        int specialAttackKillIndex = GetIndexOfSpecialCanKill(cat, enemyPlates);

        if (normalAttackKillIndex != -1)
        {
            AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, true, "고양이 일반 공격으로 적 처치 가능");
            attackIndex = normalAttackKillIndex;
        }
        else
        {
            if (specialAttackKillIndex != -1)
            {
                AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 10f, false, "고양이 특수 공격으로 적 처치 가능");
                attackIndex = specialAttackKillIndex;
            }
            else
            {
                if (GetMostDamageAttack(cat) == AttackType.NormalAttack)
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 5f, true, "고양이 일반 공격이 더 많은 피해를 줌");
                }
                else
                {
                    AttackProbabilityData = AdjustAttackProbabilities(AttackProbabilityData, 5f, false, "고양이 특수 공격이 더 많은 피해를 줌");
                }
                attackIndex = GetClosestEnemyIndex(enemyPlates);
            }
        }

        if (!cat.TryGetFirstAvailableSpecialAttack(out AttackData specialAttack, out int specialAttackIndex))
        {
            return new AttackPredictionData(cat, catPlateIndex, cat.GetAttackStrategy(), 0, enemyPlates, attackIndex, AttackProbabilityData);
        }

        return new AttackPredictionData(cat, catPlateIndex, specialAttack, specialAttackIndex, enemyPlates, attackIndex, AttackProbabilityData);
    }

    public AttackType GetMostDamageAttack(Summon attackingSummon)
    {
        AttackData[] availableSpecialAttacks = attackingSummon.GetAvailableSpecialAttacks();

        foreach (AttackData specialAttack in availableSpecialAttacks)
        {
            if (attackingSummon.GetAttackPower() > specialAttack.GetSpecialDamage())
                return AttackType.NormalAttack;
            else
                return AttackType.SpecialAttack;
        }

        return AttackType.NormalAttack;
    }

    public int GetIndexOfNormalAttackCanKill(Summon cat, IReadOnlyList<IPlateState> enemyPlates)
    {
        int closestIndex = GetClosestEnemyIndex(enemyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnemySummon = enemyPlates[closestIndex].GetCurrentSummon();
            if (closestEnemySummon != null && cat.GetAttackPower() >= closestEnemySummon.GetNowHP())
            {
                return closestIndex;
            }
        }

        return -1;
    }

    public int GetIndexOfSpecialCanKill(Summon cat, IReadOnlyList<IPlateState> enemyPlates)
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].GetCurrentSummon();
            if (enemySummon != null && cat.GetHeavyAttackPower() >= enemySummon.GetNowHP())
            {
                return i;
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
