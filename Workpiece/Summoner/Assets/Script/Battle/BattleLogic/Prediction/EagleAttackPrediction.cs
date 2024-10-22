using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Eagle;
    }


    //독수리 예측공격
    public AttackPrediction getAttackPrediction(Summon eagle, int eaglePlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates); //가장 가까운적의 인덱스 기본값


        if (isTwoOrMoreEnemies(enermyPlates)) //적이 2마리 이상인가?
        {
            if (isEnermyHealthDifferenceOver30(enermyPlates)) //몬스터 한 쪽이 다른쪽과 비교했을 때 30% 이상 낮은가?
            {
                if (getIndexOfNormalAttackCanKill(eagle, enermyPlates) != -1) //일반 공격으로 체력이 낮은 쪽을 공격할 수 있는가?
                {
                    attackIndex = getIndexOfNormalAttackCanKill(eagle, enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "독수리 일반공격으로 체력이 낮은 적 공격 가능");
                }
                else
                {
                    attackIndex = getSpecialAttackKillIndex(eagle, enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "독수리 일반공격으로 체력이 낮은쪽 공격 불가능");
                }
            }
            else if (AreEnermyHealthWithin10Percent(enermyPlates)) //몬스터의 체력이 서로 비슷한가?
            {
                if (getIndexOfMostHealthEnermy(enermyPlates) != -1) //가장 체력이 많은 몬스터의 인덱스
                {
                    attackIndex = getIndexOfMostHealthEnermy(enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "독수리 몬스터 체력이 10퍼이내로 비슷함");
                }
            }
        }
        else if (isOnlyOneEnemy(enermyPlates)) //적이 1마리 인가?
        {
            if (getIndexOfNormalAttackCanKill(eagle, enermyPlates) != -1)
            {
                attackIndex = getIndexOfNormalAttackCanKill(eagle, enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "독수리 일반공격으로 사냥가능");
            }
            else if (getSpecialAttackKillIndex(eagle, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "독수리 특수공격으로 사냥가능");
            }
            else
            {
                if (getTypeOfMoreAttackDamage(eagle, enermyPlates) == AttackType.NormalAttack)//일반공격과 특수공격 중 피해를 많이 줄 공격에 5%상승
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "독수리 일반공격이 더 큰 피해를 입힘");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "독수리 일반공격이 더 큰 피해를 입힘");
                }
            }
        }

        //소환수, 소환수의 플레이트 번호, 소환수의 특수공격첫번째, 특수공격배열 인덱스번호, 타겟플레이트, 타겟플레이트 변호, 확률
        AttackPrediction attackPrediction = new AttackPrediction(eagle, eaglePlateIndex, eagle.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }


    // 적이 2마리 이상인가?
    public bool isTwoOrMoreEnemies(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) count++;
        }
        return count >= 2;
    }

    // 적이 1마리인가?
    public bool isOnlyOneEnemy(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) count++;
        }
        return count == 1;
    }


    // 몬스터의 한쪽이 다른 쪽의 체력을 비교했을 때 30% 이상 낮은가?
    public bool isEnermyHealthDifferenceOver30(List<Plate> enermyPlates)
    {
        if (enermyPlates.Count < 2) return false;

        double maxHealthRatio = double.MinValue;
        double minHealthRatio = double.MaxValue;
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio > maxHealthRatio) maxHealthRatio = healthRatio;
                if (healthRatio < minHealthRatio) minHealthRatio = healthRatio;
            }
        }

        return (maxHealthRatio - minHealthRatio) > 0.3f;
    }

    // 몬스터들의 체력이 서로 10% 이내 차이인지 검사하는 메소드
    public bool AreEnermyHealthWithin10Percent(List<Plate> enermyPlates)
    {
        double minHealthRatio = double.MaxValue;
        double maxHealthRatio = double.MinValue;

        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null)
            {
                // 현재 체력 비율을 계산
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();

                // 최소 및 최대 체력 비율 업데이트
                if (healthRatio < minHealthRatio) minHealthRatio = healthRatio;
                if (healthRatio > maxHealthRatio) maxHealthRatio = healthRatio;
            }
        }

        // 최대와 최소 체력 비율의 차이가 10% 이내인지 검사
        return (maxHealthRatio - minHealthRatio) <= 0.1;
    }


    // 가장 체력이 많은 몬스터의 인덱스를 반환하는 메소드
    public int getIndexOfMostHealthEnermy(List<Plate> enermyPlates)
    {
        int maxHealthIndex = -1;
        double maxHealth = double.MinValue;

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double currentHealth = enermySummon.getNowHP();
                if (currentHealth > maxHealth)
                {
                    maxHealth = currentHealth;
                    maxHealthIndex = i; // 현재 최대 체력인 소환수의 인덱스 저장
                }
            }
        }

        return maxHealthIndex; // 가장 체력이 높은 소환수의 인덱스 반환, 없으면 -1
    }

    public AttackType getTypeOfMoreAttackDamage(Summon eagle, List<Plate> enermyPlates)
    {
        double maxDamage = eagle.getAttackPower(); // 기본값: 일반 공격의 데미지

        // 사용 가능한 특수 공격 목록 가져오기
        IAttackStrategy[] availableSpecialAttacks = eagle.getAvailableSpecialAttacks();

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


    // 특수 공격으로 공격할 수 있는지 확인하고, 공격 가능한 인덱스를 반환하는 메소드
    public int getSpecialAttackKillIndex(Summon eagle, List<Plate> enermyPlates)
    {
        for(int i=0; i< eagle.getSpecialAttackStrategy().Length; i++)
        {
            if (eagle.isSpecialAttackCool(eagle.getSpecialAttackStrategy()[i]))
            {
                continue;
            }
            for (int ii = 0; ii < enermyPlates.Count; ii++)
            {
                Summon enermySummon = enermyPlates[ii].getCurrentSummon();
                if (enermySummon != null && eagle.getSpecialAttackStrategy()[i].getSpecialDamage() >= enermySummon.getNowHP())
                {
                    return i; // 특수 공격으로 처치 가능한 인덱스 반환
                }
            }
        }
        return -1; // 처치할 수 없으면 -1 반환
    }

    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하고, 공격 가능한 인덱스를 반환하는 메소드
    public int getIndexOfNormalAttackCanKill(Summon eagle, List<Plate> enermyPlates)
    {
        int closestIndex = getClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            if (closestEnermySummon != null && eagle.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return closestIndex; // 가장 가까운 적을 일반 공격으로 처치할 수 있는 인덱스 반환
            }
        }
        return -1; // 처치할 수 없으면 -1 반환
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
