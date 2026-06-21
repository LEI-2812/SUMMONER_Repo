using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 토끼 소환수의 공격 선택 가능성을 예측한다.
public class RabbitAttackPrediction : MonoBehaviour, IAttackPrediction
{
 

    public bool CanPredict(Summon summon)
    {
        return summon is Rabbit;
    }

    // 역할: 토끼가 아군을 회복할지 적을 일반 공격할지 현재 체력 상태로 예측한다.
    public AttackPrediction GetAttackPrediction(Summon rabbit, int rabbitPlateIndex, IReadOnlyList<Plate> playerPlates, IReadOnlyList<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = GetClosestEnermyIndex(enermyPlates);

        int lowerHealthDifferenceIndex = GetIndexOfLowerHealthIfDifferenceOver30(playerPlates, rabbitPlateIndex);
        int allDownHealthIndex = GetIndexOfLowerHealthIfAllDown30(playerPlates);

        if (lowerHealthDifferenceIndex != -1) //소환수 중 한쪽이 다른 쪽과 체력을 비교했을 때 30% 이상 낮은가?
        {
            attackIndex = lowerHealthDifferenceIndex;
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "토끼 소환수중 한쪽이 다른 쪽과 비교할때 30% 낮음");
            return new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.GetSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (allDownHealthIndex != -1) //소환수 모두의 체력이 30% 이하인가?
        {
            attackIndex = allDownHealthIndex;
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "토끼 소환수의 체력이 모두 30% 이하");
            return new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.GetSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }

        int lowestHealthSummonIndex = GetIndexOfLowestHealthSummon(playerPlates);

        if (AllPlayerSummonOver70Percent(playerPlates))
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "토끼 모든 플레이어 소환수 체력이 70% 이상");
            return new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.GetAttackStrategy(), 0, enermyPlates, attackIndex, attackProbability);
        }
        else
        {
            int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(rabbit, enermyPlates);

            if (normalAttackKillIndex != -1)
            {
                attackIndex = normalAttackKillIndex;
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "토끼 일반공격으로 처치가능");
                return new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.GetAttackStrategy(), 0, enermyPlates, attackIndex, attackProbability);
            }
        }

        attackIndex = lowestHealthSummonIndex;
        return new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.GetSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
    }




    // 역할: 토끼 자신을 빼고 비교했을 때, 다른 아군보다 체력이 30% 이상 낮은 아군 위치를 찾는다.
    public int GetIndexOfLowerHealthIfDifferenceOver30(IReadOnlyList<Plate> playerPlates,int rabbitIndex)
    {
        if (playerPlates.Count < 2) return -1;

        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (i == rabbitIndex) continue; // rabbitIndex는 제외

            Summon currentSummon = playerPlates[i].GetCurrentSummon();
            if (currentSummon == null) continue;

            for (int j = 0; j < playerPlates.Count; j++)
            {
                if (i == j || j == rabbitIndex) continue; // 자기 자신과 rabbitIndex는 제외

                Summon compareSummon = playerPlates[j].GetCurrentSummon();
                if (compareSummon == null) continue;

                // 현재 소환수의 체력 비율 계산 (자기 체력 / 비교 소환수 체력)
                double healthRatio = currentSummon.GetNowHP() / compareSummon.GetNowHP();

                // 조건을 만족하는 경우 중에서 가장 낮은 체력을 가진 소환수의 인덱스를 추적
                if (healthRatio <= 0.7 && currentSummon.GetNowHP() < lowestHealth)
                {
                    lowestHealth = currentSummon.GetNowHP();
                    lowestHealthIndex = i;
                }
            }
        }

        return lowestHealthIndex; // 조건을 만족하는 가장 낮은 체력의 소환수 인덱스 반환, 없으면 -1 반환
    }


    // 역할: 모든 아군 체력이 30% 이하일 때 가장 체력이 낮은 아군 위치를 찾는다.
    public int GetIndexOfLowerHealthIfAllDown30(IReadOnlyList<Plate> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].GetCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.GetNowHP() / playerSummon.GetMaxHP();

                // 모든 소환수가 체력 30% 이하인지 확인
                if (healthRatio > 0.3f)
                {
                    return -1; // 하나라도 체력이 30%를 넘으면 -1 반환
                }

                // 가장 낮은 체력을 가진 소환수 인덱스를 기록
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i;
                }
            }
        }

        return indexOfMinHealth; // 모든 소환수가 30% 이하인 경우 가장 낮은 체력의 인덱스를 반환
    }

    // 역할: 아군 소환수들의 체력이 모두 70% 이상인지 확인한다.
    public bool AllPlayerSummonOver70Percent(IReadOnlyList<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.GetCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.GetNowHP() / playerSummon.GetMaxHP();
                if (healthRatio < 0.7f)
                {
                    return false; // 하나라도 70% 이하이면 false 반환
                }
            }
        }
        return true;
    }

    // 역할: 아군 중 현재 체력 비율이 가장 낮은 소환수 위치를 찾는다.
    public int GetIndexOfLowestHealthSummon(IReadOnlyList<Plate> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].GetCurrentSummon();
            if (summon != null)
            {
                double healthRatio = summon.GetNowHP() / summon.GetMaxHP();

                // 가장 낮은 체력을 가진 소환수의 인덱스 기록
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i;
                }
            }
        }

        return indexOfMinHealth; // 가장 낮은 체력의 소환수 인덱스 반환
    }

    // 역할: 가장 가까운 적을 일반 공격 한 번으로 처치할 수 있으면 그 적 위치를 돌려준다.
    public int GetIndexOfNormalAttackCanKill(Summon rabbit, IReadOnlyList<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = GetClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].GetCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && rabbit.GetAttackPower() >= closestEnermySummon.GetNowHP())
            {
                return closestIndex; // 공격으로 물리칠 수 있으면 인덱스 반환
            }
        }

        return -1; // 공격 가능한 적이 없으면 -1 반환
    }


    // 역할: 적 플레이트를 앞에서부터 확인해 가장 먼저 만나는 적 위치를 찾는다.
    public int GetClosestEnermyIndex(IReadOnlyList<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].GetCurrentSummon();
            if (enermySummon != null)
            {
                return i; // 가장 가까운(첫 번째로 발견된) 적 소환수의 인덱스 반환
            }
        }
        return -1; // 적 소환수가 없으면 -1 반환
    }




    // 역할: 판단 이유에 따라 일반 공격 또는 특수 공격 확률을 한쪽으로 조금 이동시킨다.
    private AttackProbability AdjustAttackProbabilities(AttackProbability currentProbabilities, float AttackChange, bool isNormalAttack, string reason)
    {
        if (isNormalAttack)
        {
            // 일반 공격 확률을 증가시키고, 특수 공격 확률을 그만큼 감소
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
            Debug.Log($"일반 공격 확률이 {AttackChange}% 증가하였습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        else
        {
            // 특수 공격 확률을 증가시키고, 일반 공격 확률을 그만큼 감소
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
            Debug.Log($"특수 공격 확률이 {AttackChange}% 증가하였습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        return currentProbabilities;
    }
}
