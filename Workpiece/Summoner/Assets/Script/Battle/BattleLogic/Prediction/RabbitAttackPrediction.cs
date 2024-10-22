using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitAttackPrediction : MonoBehaviour, IAttackPrediction
{
 

    public SummonType getPreSummonType()
    {
        return SummonType.Rabbit;
    }

    public AttackPrediction getAttackPrediction(Summon rabbit, int rabbitPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (GetIndexOfLowerHealthIfDifferenceOver30(playerPlates) != -1) //소환수 중 한쪽이 다른 쪽과 체력을 비교했을 때 30% 이상 낮은가?
        {
            attackIndex = GetIndexOfLowerHealthIfDifferenceOver30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "토끼 소환수중 한쪽이 다른 쪽과 비교할때 30% 낮음");
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (getIndexOfLowerHealthIfAllDown30(playerPlates) != -1) //소환수 모두의 체력이 30% 이하인가?
        {
            attackIndex = getIndexOfLowerHealthIfAllDown30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "토끼 소환수의 체력이 모두 30% 이하");
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (AllPlayerSummonOver70Percent(playerPlates))
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "토끼 모든 플레이어 소환수 체력이 70% 이상");
            attackIndex = getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (getIndexOfNormalAttackCanKill(rabbit, enermyPlates) != -1)
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "토끼 일반공격으로 처치가능");
            attackIndex = getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else// 모두 조건이 안맞으면 가장 낮은 체력 아군 힐
        {
            attackIndex = getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }


        return attackPrediction;
    }




    // 소환수의 체력 차이가 30% 이상 낮은지 확인하는 메소드
    // 아군 소환수 중 체력 차이가 30% 이상인 경우, 더 낮은 체력을 가진 소환수의 인덱스를 반환하는 메소드
    public int GetIndexOfLowerHealthIfDifferenceOver30(List<Plate> playerPlates)
    {
        if (playerPlates.Count < 2) return -1; // 아군이 2명 미만이면 비교할 수 없음

        double maxHealthRatio = double.MinValue;
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon enermySummon = playerPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i; // 체력이 더 낮은 소환수의 인덱스를 기록
                }
                if (healthRatio > maxHealthRatio)
                {
                    maxHealthRatio = healthRatio;
                }
            }
        }

        // 체력 차이가 30% 이상인 경우에만 인덱스 반환
        if ((maxHealthRatio - minHealthRatio) > 0.3f)
        {
            return indexOfMinHealth;
        }

        return -1; // 체력 차이가 30% 이상이 아니면 -1 반환
    }


    // 모든 플레이어 소환수 중 체력이 30% 이하인 소환수 중 가장 낮은 체력을 가진 소환수의 인덱스를 반환하는 메소드
    public int getIndexOfLowerHealthIfAllDown30(List<Plate> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].getCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.getNowHP() / playerSummon.getMaxHP();

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

    // 플레이어 소환수의 체력이 모두 70% 이상인지 확인
    public bool AllPlayerSummonOver70Percent(List<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.getNowHP() / playerSummon.getMaxHP();
                if (healthRatio < 0.7f)
                {
                    return false; // 하나라도 70% 이하이면 false 반환
                }
            }
        }
        return true;
    }

    // 가장 낮은 체력을 가진 소환수의 인덱스를 반환하는 메소드
    public int getIndexOfLowestHealthSummon(List<Plate> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].getCurrentSummon();
            if (summon != null)
            {
                double healthRatio = summon.getNowHP() / summon.getMaxHP();

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

    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하는 메소드
    public int getIndexOfNormalAttackCanKill(Summon rabbit, List<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && rabbit.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return closestIndex; // 공격으로 물리칠 수 있으면 인덱스 반환
            }
        }

        return -1; // 공격 가능한 적이 없으면 -1 반환
    }


    public int getClosestEnermyIndex(List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                return i; // 가장 가까운(첫 번째로 발견된) 적 소환수의 인덱스 반환
            }
        }
        return -1; // 적 소환수가 없으면 -1 반환
    }




    // 확률 값을 설정하고 조정하여 반환하는 메소드
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
