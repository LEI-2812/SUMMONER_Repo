using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType GetPreSummonType()
    {
        return SummonType.Eagle;
    }


    //독수리 예측공격
    public AttackPrediction GetAttackPrediction(Summon eagle, int eaglePlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = GetClosestEnermyIndex(enermyPlates); //가장 가까운적의 인덱스 기본값

        if (IsTwoOrMoreEnemies(enermyPlates)) //적이 2마리 이상인가?
        {
            int lowestHealthDifferenceIndex = IsEnermyHealthDifferenceOver30(enermyPlates);

            if (lowestHealthDifferenceIndex != -1) //몬스터 한 쪽이 다른쪽과 비교했을 때 30% 이상 낮은가?
            {
                int normalAttackLowestIndex = CanNormalAttack(eagle, enermyPlates, lowestHealthDifferenceIndex);

                if (normalAttackLowestIndex != -1) //일반 공격으로 체력이 낮은 쪽을 공격할 수 있는가?
                {
                    attackIndex = normalAttackLowestIndex;
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "독수리 일반공격으로 체력이 낮은 적 공격 가능");
                }
                else
                {
                    attackIndex = lowestHealthDifferenceIndex;
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "독수리 일반공격으로 체력이 낮은쪽 공격 불가능");
                }
            }
            else
            {
                int healthWithin10PercentIndex = AreEnermyHealthWithin10Percent(eagle, enermyPlates);

                if (healthWithin10PercentIndex != -1) //몬스터의 체력이 서로 비슷한가? (10%이내)
                {
                    attackIndex = healthWithin10PercentIndex;
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "독수리 몬스터 체력이 10퍼이내로 비슷함");
                }
            }
        }
        else if (IsOnlyOneEnemy(enermyPlates)) //적이 1마리 인가?
        {
            int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(eagle, enermyPlates);

            if (normalAttackKillIndex != -1)
            {
                attackIndex = normalAttackKillIndex;
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "독수리 일반공격으로 사냥가능");
            }
            else
            {
                int specialAttackKillIndex = GetSpecialAttackKillIndex(eagle, enermyPlates);

                if (specialAttackKillIndex != -1)
                {
                    attackIndex = specialAttackKillIndex;
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "독수리 특수공격으로 사냥가능");
                }
                else
                {
                    if (GetTypeOfMoreAttackDamage(eagle, enermyPlates) == AttackType.NormalAttack)//일반공격과 특수공격 중 피해를 많이 줄 공격에 5%상승
                    {
                        attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "독수리 일반공격이 더 큰 피해를 입힘");
                    }
                    else
                    {
                        attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "독수리 일반공격이 더 큰 피해를 입힘");
                    }
                }
            }
        }

        Debug.Log("독수리 겨냥: " + attackIndex);
        return new AttackPrediction(eagle, eaglePlateIndex, eagle.GetSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
    }


    // 적이 2마리 이상인가?
    public bool IsTwoOrMoreEnemies(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.GetCurrentSummon() != null) count++;
        }
        return count >= 2;
    }

    // 적이 1마리인가?
    public bool IsOnlyOneEnemy(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.GetCurrentSummon() != null) count++;
        }
        return count == 1;
    }


    // 적의 현재 체력 중 다른 소환수의 체력보다 30% 낮은 소환수 중 가장 체력이 낮은 소환수의 인덱스를 반환하는 메소드
    public int IsEnermyHealthDifferenceOver30(List<Plate> enermyPlates)
    {
        if (enermyPlates.Count < 2) return -1;

        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon currentSummon = enermyPlates[i].GetCurrentSummon();
            if (currentSummon == null) continue;

            for (int j = 0; j < enermyPlates.Count; j++)
            {
                if (i == j) continue; // 자기 자신은 비교하지 않음

                Summon compareSummon = enermyPlates[j].GetCurrentSummon();
                if (compareSummon == null) continue;

                // 현재 소환수의 체력 비율 계산 (자기 체력 / 비교 몬스터 체력)
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


    //체력이 가장 낮은 몬스터의 인덱스를 가져와 근접한 인덱스와 비교하는 메소드
    public bool IsLowestHealthEnermyClosest(Summon attackingSummon, List<Plate> enermyPlates, int lowestIndex)
    {
        // 적 소환수가 2개 미만인 경우 비교할 수 없으므로 false 반환
        if (enermyPlates.Count < 2) return false;

        //가장 가까운 인덱스 반환
        int closetIndex = GetClosestEnermyIndex(attackingSummon, enermyPlates);

        if (closetIndex == lowestIndex)
            return true;

        return false;
    }

    // 가장 가까운 적 소환수의 인덱스를 반환하는 메소드
    public int GetClosestEnermyIndex(Summon attackingSummon, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].GetCurrentSummon();
            if (enermySummon != null && enermySummon != attackingSummon)
            {
                return i; // 첫 번째로 만나는 유효한 적 소환수의 인덱스를 반환
            }
        }

        return -1; // 적 소환수가 없는 경우 -1 반환
    }


    // 몬스터들의 체력이 서로 10% 이내 차이인지 검사하는 메소드
    public int AreEnermyHealthWithin10Percent(Summon eagle,List<Plate> enermyPlates)
    {
        if (enermyPlates.Count < 2) return -1;

        // 사용 가능한 특수 공격이 있는지 먼저 검사
        bool hasAvailableSpecialAttack = false;
        for (int i = 0; i < eagle.GetSpecialAttackStrategy().Length; i++)
        {
            if (!eagle.IsSpecialAttackCool(eagle.GetSpecialAttackStrategy()[i]))
            {
                hasAvailableSpecialAttack = true;
                break;
            }
        }

        // 사용 가능한 특수 공격이 없으면 -1 반환
        if (!hasAvailableSpecialAttack)
        {
            return -1;
        }

        int maxHealthIndex = -1;
        double highestHealth = double.MinValue;

        // 최대 현재 체력을 가진 소환수를 찾음
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon currentSummon = enermyPlates[i].GetCurrentSummon();
            if (currentSummon == null) continue;

            double currentHealth = currentSummon.GetNowHP();

            if (currentHealth > highestHealth)
            {
                highestHealth = currentHealth;
                maxHealthIndex = i;
            }
        }

        // 현재 체력이 가장 높은 소환수가 없다면 -1 반환
        if (maxHealthIndex == -1) return -1;

        // 반환할 인덱스 초기화 (-1로 설정, 조건 만족 시 maxHealthIndex로 설정)
        int resultIndex = maxHealthIndex;
        double maxHealth = highestHealth;

        // 최대 현재 체력을 가진 소환수를 제외하고 나머지 소환수들이 10% 이내 차이인지 검사
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (i == maxHealthIndex) continue; // 최대 현재 체력 소환수는 비교에서 제외

            Summon compareSummon = enermyPlates[i].GetCurrentSummon();
            if (compareSummon == null) continue;

            double healthDifference = Math.Abs(maxHealth - compareSummon.GetNowHP());

            // 10% 이상의 차이가 나면 조건을 만족하지 않으므로 resultIndex를 -1로 설정하고 종료
            if (healthDifference > maxHealth * 0.1)
            {
                resultIndex = -1;
                break;
            }
        }

        // 모든 소환수가 10% 이내 차이를 만족할 경우에만 최대 체력 소환수의 인덱스를 반환
        return resultIndex;
    }

    private int CanNormalAttack(Summon attackingSummon ,List<Plate> enermyPlates , int lowestIndex)
    {
        // 적 소환수가 2개 미만인 경우 비교할 수 없으므로 false 반환
        if (enermyPlates.Count < 2) return -1;

        //가장 가까운 인덱스 반환
        int closetIndex = GetClosestEnermyIndex(attackingSummon, enermyPlates);

        if (closetIndex == lowestIndex)
            return closetIndex;

        return -1;
    }


    // 가장 체력이 많은 몬스터의 인덱스를 반환하는 메소드
    public int GetIndexOfMostHealthEnermy(List<Plate> enermyPlates)
    {
        int maxHealthIndex = -1;
        double maxHealth = double.MinValue;

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].GetCurrentSummon();
            if (enermySummon != null)
            {
                double currentHealth = enermySummon.GetNowHP();
                if (currentHealth > maxHealth)
                {
                    maxHealth = currentHealth;
                    maxHealthIndex = i; // 현재 최대 체력인 소환수의 인덱스 저장
                }
            }
        }

        return maxHealthIndex; // 가장 체력이 높은 소환수의 인덱스 반환, 없으면 -1
    }

    public AttackType GetTypeOfMoreAttackDamage(Summon eagle, List<Plate> enermyPlates)
    {
        double normalAttackDamage = eagle.GetAttackPower(); // 기본값: 일반 공격의 데미지

        // 사용 가능한 특수 공격 목록 가져오기
        IAttackStrategy[] availableSpecialAttacks = eagle.GetAvailableSpecialAttacks();

        // 각 특수 공격 확인
        foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
        {
            double totalSpecialAttackDamage = 0;

            // 적 플레이트에 있는 모든 소환수에게 특수 공격 시 예상 피해 축적
            foreach (Plate plate in enermyPlates)
            {
                Summon enermySummon = plate.GetCurrentSummon();
                if (enermySummon != null)
                {
                    totalSpecialAttackDamage += specialAttack.GetSpecialDamage();
                }
            }

            // 특수 공격 총 피해가 일반 공격보다 크면 특수 공격 선택
            if (totalSpecialAttackDamage > normalAttackDamage)
            {
                return AttackType.SpecialAttack;
            }
        }

        return AttackType.NormalAttack;
    }


    // 특수 공격으로 공격할 수 있는지 확인하고, 공격 가능한 인덱스를 반환하는 메소드
    public int GetSpecialAttackKillIndex(Summon eagle, List<Plate> enermyPlates)
    {
        // 사용 가능한 특수 공격이 있는지 먼저 검사
        bool hasAvailableSpecialAttack = false;
        for (int i = 0; i < eagle.GetSpecialAttackStrategy().Length; i++)
        {
            if (!eagle.IsSpecialAttackCool(eagle.GetSpecialAttackStrategy()[i]))
            {
                hasAvailableSpecialAttack = true;
                break;
            }
        }

        // 사용 가능한 특수 공격이 없으면 -1 반환
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
            for (int ii = 0; ii < enermyPlates.Count; ii++)
            {
                Summon enermySummon = enermyPlates[ii].GetCurrentSummon();
                if (enermySummon != null && eagle.GetSpecialAttackStrategy()[i].GetSpecialDamage() >= enermySummon.GetNowHP())
                {
                    return i; // 특수 공격으로 처치 가능한 인덱스 반환
                }
            }
        }
        return -1; // 처치할 수 없으면 -1 반환
    }

    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하고, 공격 가능한 인덱스를 반환하는 메소드
    public int GetIndexOfNormalAttackCanKill(Summon eagle, List<Plate> enermyPlates)
    {
        int closestIndex = GetClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].GetCurrentSummon();
            if (closestEnermySummon != null && eagle.GetAttackPower() >= closestEnermySummon.GetNowHP())
            {
                return closestIndex; // 가장 가까운 적을 일반 공격으로 처치할 수 있는 인덱스 반환
            }
        }
        return -1; // 처치할 수 없으면 -1 반환
    }

    public int GetClosestEnermyIndex(List<Plate> enermyPlates)
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
