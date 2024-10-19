using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleAttackPrediction : MonoBehaviour
{

    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
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
        foreach (IAttackStrategy specialAttack in eagle.getAvailableSpecialAttacks())
        {
            for (int i = 0; i < enermyPlates.Count; i++)
            {
                Summon enermySummon = enermyPlates[i].getCurrentSummon();
                if (enermySummon != null && specialAttack.getSpecialDamage() >= enermySummon.getNowHP())
                {
                    return i; // 특수 공격으로 처치 가능한 인덱스 반환
                }
            }
        }
        return -1; // 처치할 수 없으면 -1 반환
    }

    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하고, 공격 가능한 인덱스를 반환하는 메소드
    public int getNormalAttackKillIndex(Summon eagle, List<Plate> enermyPlates)
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
}
