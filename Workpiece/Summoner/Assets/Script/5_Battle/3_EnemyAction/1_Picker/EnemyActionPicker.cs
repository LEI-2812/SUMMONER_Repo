using System.Collections.Generic;
using UnityEngine;

// 역할: 적 행동 흐름에서 사용할 확률 판정과 조건 기반 선택을 담당한다.
// 책임 아님: 선택된 행동 실행, 예측 목록 수정.
internal class EnemyActionPicker
{
    public int PickHealSpecialAttackIndexForDamageStatus(Summon summon)
    {
        // 피해 상태가 있을 때만 회복 특수공격 선사용 후보를 찾는다.
        if (!HasDamageStatus(summon))
        {
            return -1;
        }

        IAttackStrategy[] specialAttackStrategies = summon.GetSpecialAttackStrategy();
        if (specialAttackStrategies == null)
        {
            return -1;
        }

        for (int i = 0; i < specialAttackStrategies.Length; i++)
        {
            IAttackStrategy attackStrategy = specialAttackStrategies[i];
            if (attackStrategy == null)
            {
                continue;
            }

            if (attackStrategy.GetStatusType() == StatusType.Heal && attackStrategy.GetCurrentCooldown() <= 0)
            {
                return i;
            }
        }

        return -1;
    }

    public bool CanContinueAttack(Summon summon)
    {
        // 연속공격 확률은 소환수 등급에 따라 달라진다.
        float randomValue = Random.Range(0f, 100f);

        if (summon.GetSummonRank() == SummonRank.Special)
        {
            return randomValue <= 20f;
        }

        if (summon.GetSummonRank() == SummonRank.Boss)
        {
            return randomValue <= 30f;
        }

        return false;
    }

    public bool CanReactWithSpecialAttack(AttackProbability attackProbability)
    {
        // 플레이어 예측별 특수 대응 확률을 판정한다.
        float randomValue = Random.Range(0f, 100f);
        if (randomValue < attackProbability.specialAttackProbability)
        {
            Debug.Log("특수공격확률 당첨");
        }

        return randomValue < attackProbability.specialAttackProbability;
    }

    public bool ShouldUseHeavyNormalAttack()
    {
        // 일반공격 중 강공격으로 바뀔지 판정한다.
        return Random.Range(0f, 100f) < 30f;
    }

    public int PickHighHealthPlayerPlateIndexWithLowAlly(IReadOnlyList<Plate> playerPlates)
    {
        // 흡혈 대응용으로 체력이 가장 높은 플레이어 소환수 위치를 고른다.
        if (playerPlates.Count < 2)
        {
            return -1;
        }

        int highestHealthIndex = -1;
        double highestHealth = double.MinValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon currentSummon = playerPlates[i].GetCurrentSummon();
            if (currentSummon == null)
            {
                continue;
            }

            if (currentSummon.GetNowHP() > highestHealth)
            {
                highestHealth = currentSummon.GetNowHP();
                highestHealthIndex = i;
            }
        }

        if (highestHealthIndex == -1)
        {
            return -1;
        }

        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (i == highestHealthIndex)
            {
                continue;
            }

            Summon compareSummon = playerPlates[i].GetCurrentSummon();
            if (compareSummon == null)
            {
                continue;
            }

            if (compareSummon.GetNowHP() <= highestHealth * 0.7)
            {
                return highestHealthIndex;
            }
        }

        return -1;
    }

    public bool HasPlayerSummonOverMediumRank(IReadOnlyList<Plate> plates)
    {
        // 중급 이상 소환수가 있으면 저주/스턴/쉴드 대응 후보가 생긴다.
        foreach (Plate plate in plates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon != null && (summon.GetSummonRank() == SummonRank.Medium || summon.GetSummonRank() == SummonRank.High))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasDamageStatus(Summon summon)
    {
        foreach (StatusType statusType in summon.GetAllStatusTypes())
        {
            if (statusType == StatusType.Burn || statusType == StatusType.LifeDrain || statusType == StatusType.Poison)
            {
                return true;
            }
        }

        return false;
    }
}
