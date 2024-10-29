using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Wolf;
    }

    public AttackPrediction getAttackPrediction(Summon wolf, int wolfPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(wolf, wolfPlateIndex, wolf.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (IsEnermyCountTwoOrMore(enermyPlates)) //적이 2마리 이상인가?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "늑대 적이 2마리 이상");
            if (AllEnermyHealthOver50(enermyPlates)) //몬스터 체력이 모두 50% 이상인가?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "늑대 적 몬스터 체력이 모두 50% 이상");
            }
            else if (AllEnermyHealthDown50(enermyPlates)) //몬스터 체력이 모두 50% 이하인가?
            {
                if (HasSpecificAvailableSpecialAttack(wolf, wolfPlateIndex, playerPlates))
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "늑대 적 몬스터 체력이 모두 50% 아래고 소환수 중 공격형이 뒤에 존재");
                }
            }
            else if (IsEnermyHealthDifferenceOver30(enermyPlates) != -1) //몬스터 한쪽이 다른 쪽에비해 체력이 30% 이상 낮은가? 존재하면 가장 낮은 인덱스 반환
            {
                int lowestIndex = IsEnermyHealthDifferenceOver30(enermyPlates);
                if (IsLowestHealthEnermyClosest(wolf, enermyPlates, lowestIndex)) //낮은쪽의 인덱스가 근접공격하는 인덱스와 동일한가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 20f, true, "늑대 낮은쪽으로 공격하는 인덱스가 동일");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "늑대 일반공격으로 공격하는 인덱스가 불일치");
                }
            }
        }
        else if (IsEnermyCountOne(enermyPlates)) //적이 1마리 인가?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "늑대 적이 1마리뿐");

            if (getIndexOfNormalAttackCanKill(wolf, enermyPlates) != -1) //일반 공격으로 몬스터를 물리칠 수 있는가?
            {
                attackIndex = getIndexOfNormalAttackCanKill(wolf, enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "늑대 일반 공격으로 처치가능");
            }
            else
            {
                //일반 공격과 특수공격중 피해를 더 줄 수 있는 공격을 반환했을 때 일반 공격일경우
                if (getMostDamageAttack(wolf, enermyPlates) == AttackType.NormalAttack)
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "늑대 일반 공격이 더 많은 피해를 입힘");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "늑대 특수공격이 더 많은 피해를 입힘");
                }
            }
        }

        attackPrediction = new AttackPrediction(wolf, wolfPlateIndex, wolf.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }


    // 적의 체력이 모두 50% 이상인지 확인하는 메소드
    public bool AllEnermyHealthOver50(List<Plate> enermyPlates)
    {
        bool hasAliveEnermy = false;

        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();

            if (enermySummon != null) // 살아있는 소환수만 검사
            {
                hasAliveEnermy = true; // 살아있는 소환수가 있음
                double healthRatio = (double)enermySummon.getNowHP() / enermySummon.getMaxHP();

                if (healthRatio < 0.5) // 체력 비율이 50% 미만인 경우
                {
                    return false; // 하나라도 50% 미만인 소환수가 있으면 false 반환
                }
            }
        }

        return hasAliveEnermy; // 살아있는 소환수가 있고, 모두 50% 이상일 경우 true 반환
    }

    // 살아있는 적 소환수의 체력이 모두 50% 이하인지 확인하는 메소드
    public bool AllEnermyHealthDown50(List<Plate> enermyPlates)
    {
        bool hasAliveEnermy = false;

        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();

            if (enermySummon != null)
            {
                hasAliveEnermy = true; // 살아있는 소환수가 존재함을 확인
                if (enermySummon.getNowHP() / enermySummon.getMaxHP() > 0.5f)
                {
                    return false; // 체력이 50% 초과인 소환수가 있으면 false 반환
                }
            }
        }

        return hasAliveEnermy; // 살아있는 소환수가 모두 50% 이하일 경우 true 반환
    }

    // 적의 현재 체력 중 다른 소환수의 체력보다 30% 낮은 소환수 중 가장 체력이 낮은 소환수의 인덱스를 반환하는 메소드
    public int IsEnermyHealthDifferenceOver30(List<Plate> enermyPlates)
    {
        if (enermyPlates.Count < 2) return -1;

        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon currentSummon = enermyPlates[i].getCurrentSummon();
            if (currentSummon == null) continue;

            for (int j = 0; j < enermyPlates.Count; j++)
            {
                if (i == j) continue; // 자기 자신은 비교하지 않음

                Summon compareSummon = enermyPlates[j].getCurrentSummon();
                if (compareSummon == null) continue;

                // 현재 소환수의 체력 비율 계산 (자기 체력 / 비교 몬스터 체력)
                double healthRatio = currentSummon.getNowHP() / compareSummon.getNowHP();

                // 조건을 만족하는 경우 중에서 가장 낮은 체력을 가진 소환수의 인덱스를 추적
                if (healthRatio <= 0.7 && currentSummon.getNowHP() < lowestHealth)
                {
                    lowestHealth = currentSummon.getNowHP();
                    lowestHealthIndex = i;
                }
            }
        }

        return lowestHealthIndex; // 조건을 만족하는 가장 낮은 체력의 소환수 인덱스 반환, 없으면 -1 반환
    }


    //체력이 가장 낮은 몬스터의 인덱스를 가져와 근접한 인덱스와 비교하는 메소드
    public bool IsLowestHealthEnermyClosest(Summon attackingSummon, List<Plate> enermyPlates, int lowestIndex)
    {
        // 적 소환수가 2개 미만인 경우 비교할 수 없으므로 false 반환
        if (enermyPlates.Count < 2) return false;

        //가장 가까운 인덱스 반환
        int closetIndex = getClosestEnermyIndex(attackingSummon, enermyPlates);

        if (closetIndex == lowestIndex)
            return true;

        return false;  
    }

    // 가장 가까운 적 소환수의 인덱스를 반환하는 메소드
    public int getClosestEnermyIndex(Summon attackingSummon, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null && enermySummon != attackingSummon)
            {
                return i; // 첫 번째로 만나는 유효한 적 소환수의 인덱스를 반환
            }
        }

        return -1; // 적 소환수가 없는 경우 -1 반환
    }



    // 적이 2마리 이상인지 확인하는 메소드
    public bool IsEnermyCountTwoOrMore(List<Plate> enermyPlates)
    {
        int enermyCount = 0;

        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) // Plate에 소환수가 있는지 확인
            {
                enermyCount++;
                if (enermyCount >= 2) return true; // 2마리 이상이면 true 반환
            }
        }

        return false;
    }

    // 적이 1마리인지 확인하는 메소드
    public bool IsEnermyCountOne(List<Plate> enermyPlates)
    {
        int enermyCount = 0;

        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) // Plate에 소환수가 있는지 확인
            {
                enermyCount++;
                if (enermyCount > 1) return false; // 2마리 이상이면 false 반환
            }
        }

        return enermyCount == 1; // 최종적으로 1마리일 경우 true 반환
    }


    // 자기 자신을 제외하고 플레이어의 소환수에 특수공격이 공격형인 스킬이 있는지 확인하는 메소드
    public bool HasSpecificAvailableSpecialAttack(Summon self, int wolfPlateIndex, List<Plate> playerPlates)
    {
        // 검사할 특수 공격 상태 타입들 (None, Burn, Poison, LifeDrain)
        StatusType[] specificStatuses = new StatusType[] { StatusType.None, StatusType.Burn, StatusType.Poison, StatusType.LifeDrain };

        if (wolfPlateIndex < 2)
        {
            for(int i=wolfPlateIndex+1; i< playerPlates.Count; i++)
            {
                Summon playerSummon = playerPlates[i].getCurrentSummon();
                // 자기 자신을 제외하고 검사
                if (playerSummon != null && playerSummon != self)
                {
                    // 소환수의 사용 가능한 특수 공격들을 가져옴
                    IAttackStrategy[] availableSpecialAttacks = playerSummon.getAvailableSpecialAttacks();

                    // 사용 가능한 특수 공격 중 특정 상태가 있는지 확인
                    foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
                    {
                        if (specialAttack != null && specificStatuses.Contains(specialAttack.getStatusType()))
                        {
                            // 특정 상태가 있는 특수 공격이 있다면 true 반환
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }


    public AttackType getMostDamageAttack(Summon attackingSummon, List<Plate> enermyPlates)
    {
        double maxDamage = attackingSummon.getAttackPower(); // 기본값: 일반 공격의 데미지


        // 사용 가능한 특수 공격 목록 가져오기
        IAttackStrategy[] availableSpecialAttacks = attackingSummon.getAvailableSpecialAttacks();

        // 각 특수 공격 확인
        foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
        {
            double totalSpecialAttackDamage = 0;

            // 적 플레이트에 있는 모든 소환수에게 특수 공격 시 예상 피해 축적
            foreach (Plate plate in enermyPlates)
            {
                Summon enermySummon = plate.getCurrentSummon();
                if (enermySummon != null)
                {
                    totalSpecialAttackDamage += specialAttack.getSpecialDamage();
                }
            }

            // 특수 공격으로 총 피해가 일반 공격보다 크다면 업데이트
            if (totalSpecialAttackDamage > maxDamage)
            {
                maxDamage = totalSpecialAttackDamage;
                return AttackType.SpecialAttack;
            }
        }

        return AttackType.NormalAttack;
    }

    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하는 메소드
    public int getIndexOfNormalAttackCanKill(Summon wolf, List<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && wolf.getAttackPower() >= closestEnermySummon.getNowHP())
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
