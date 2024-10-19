using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
    }



    // 적의 체력이 모두 50% 이상인지 확인하는 메소드
    public bool AllEnermyHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.5f)
            {
                return false;
            }
        }
        return true;
    }

    // 적의 체력이 모두 50% 이하인지 확인하는 메소드
    public bool AllEnermyHealthDown50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() > 0.5f)
            {
                return false;
            }
        }
        return true;
    }

    // 적의 체력 차이가 30% 이상인지 확인하는 메소드
    public bool IsEnermyHealthDifferenceOver30(List<Plate> enermyPlates)
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

    // 적의 체력 차이가 30% 이상인지 확인하고, 체력이 가장 낮은 몬스터의 인덱스를 가져와 근접한 인덱스와 비교하는 메소드
    public bool IsLowestHealthEnermyClosest(Summon attackingSummon, List<Plate> enermyPlates)
    {
        // 적 소환수가 2개 미만인 경우 비교할 수 없으므로 false 반환
        if (enermyPlates.Count < 2) return false;

        double maxHealthRatio = double.MinValue;
        double minHealthRatio = double.MaxValue;
        int lowestHealthIndex = -1;

        // 적 몬스터의 최대 체력 비율과 최소 체력 비율을 계산하고, 최소 체력을 가진 인덱스 저장
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio > maxHealthRatio) maxHealthRatio = healthRatio;
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    lowestHealthIndex = i;
                }
            }
        }

        // 체력 비율의 차이가 30% 이상인지 확인
        if ((maxHealthRatio - minHealthRatio) > 0.3f)
        {
            // 체력이 가장 낮은 적 소환수의 인덱스와 가장 가까운 적 소환수의 인덱스를 비교
            int closestIndex = getClosestEnermyIndex(attackingSummon , enermyPlates);
            return (lowestHealthIndex == closestIndex);
        }

        return false; // 체력 차이가 30% 이상이 아니거나 조건을 만족하지 않으면 false 반환
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


    // 적이 2마리 이상인지 확인하는 메소드
    public bool IsEnermyCountTwoOrMore(List<Plate> enermyPlates)
    {
        return enermyPlates.Count >= 2;
    }

    // 적이 1마리인지 확인하는 메소드
    public bool IsEnermyCountOne(List<Plate> enermyPlates)
    {
        return enermyPlates.Count == 1;
    }


    // 자기 자신을 제외하고 플레이어의 소환수에 특정 특수 공격 상태가 있는지 확인하는 메소드
    public bool HasSpecificAvailableSpecialAttack(Summon self, List<Plate> playerPlates)
    {
        // 검사할 특수 공격 상태 타입들 (None, Burn, Poison, LifeDrain)
        StatusType[] specificStatuses = new StatusType[] { StatusType.None, StatusType.Burn, StatusType.Poison, StatusType.LifeDrain };

        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
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
        return false; // 특정 상태가 없으면 false 반환
    }


    // 일반 공격으로 적을 물리칠 수 있는 가장 가까운 인덱스를 반환하는 메소드
    public int getIndexofNormalAttackCanKill(Summon attackingSummon, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null && attackingSummon.getAttackPower() >= enermySummon.getNowHP())
            {
                // 일반 공격으로 적의 체력을 0 이하로 만들 수 있으면 해당 인덱스 반환
                return i;
            }
        }
        return -1; // 공격 가능한 적이 없으면 -1 반환
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
}
