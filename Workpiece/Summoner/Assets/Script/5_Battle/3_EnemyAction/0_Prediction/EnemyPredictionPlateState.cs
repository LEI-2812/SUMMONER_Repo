using System.Collections.Generic;
using UnityEngine;

// 역할: 적 공격 예측 중 사용할 플레이트 상태를 준비하고 원래 상태로 복구한다.
internal class EnemyPredictionPlateState
{
    private readonly List<Plate> enemyPlates;
    private readonly List<Summon> originEnemySummons;

    public List<Plate> PlayerPlates { get; }
    public List<Plate> EnemyPlates { get; }

    public EnemyPredictionPlateState(PlateController plateController)
    {
        PlayerPlates = CollectPlayerPlates(plateController.GetPlayerPlates());
        originEnemySummons = new List<Summon>();
        enemyPlates = ApplyEnemyStatusToPlates(plateController.GetEnermyPlates(), originEnemySummons);
        EnemyPlates = enemyPlates;
    }

    public void Restore()
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            enemyPlates[i].SetCurrentSummon(originEnemySummons[i]);
        }
    }

    private List<Plate> CollectPlayerPlates(List<Plate> playerPlates)
    {
        List<Plate> playerPlateStates = new List<Plate>();

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Plate plate = playerPlates[i];
            Summon summon = plate.GetCurrentSummon();

            if (summon != null)
            {
                Debug.Log($"{summon.GetSummonName()}이 리스트로 들어감");
                playerPlateStates.Add(plate);
            }
        }

        return playerPlateStates;
    }

    private List<Plate> ApplyEnemyStatusToPlates(List<Plate> sourceEnemyPlates, List<Summon> originSummons)
    {
        List<Plate> adjustedEnemyPlates = new List<Plate>();

        foreach (Plate plate in sourceEnemyPlates)
        {
            Summon originSummon = plate.GetCurrentSummon();
            originSummons.Add(originSummon);

            if (originSummon != null)
            {
                Summon clonedSummon = originSummon.Clone();
                Summon adjustedSummon = ApplyEnemyStatus(clonedSummon);

                plate.SetCurrentSummon(adjustedSummon);
                adjustedEnemyPlates.Add(plate);
            }
            else
            {
                adjustedEnemyPlates.Add(plate);
            }
        }

        return adjustedEnemyPlates;
    }

    private Summon ApplyEnemyStatus(Summon clonedSummon)
    {
        foreach (StatusEffect statusEffect in clonedSummon.GetActiveStatusEffects())
        {
            if (!IsDamagePredictionStatus(statusEffect))
            {
                continue;
            }

            clonedSummon.SetNowHP(clonedSummon.GetNowHP() - statusEffect.damagePerTurn);
            if (clonedSummon.GetNowHP() <= 0)
            {
                return null;
            }
        }

        return clonedSummon;
    }

    private bool IsDamagePredictionStatus(StatusEffect statusEffect)
    {
        if (statusEffect == null)
        {
            return false;
        }

        return statusEffect.statusType == StatusType.Poison
            || statusEffect.statusType == StatusType.Burn
            || statusEffect.statusType == StatusType.LifeDrain;
    }
}
