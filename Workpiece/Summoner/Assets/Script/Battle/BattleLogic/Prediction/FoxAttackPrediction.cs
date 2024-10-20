using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
    }

    // 소환수 중 저주 상태 이상에 걸려있는 몹이 존재하는가?
    public int getIndexOfSummonWithCurseStatus(List<Plate> playerPlates)
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].getCurrentSummon();
            if (playerSummon != null && playerSummon.IsCursed())
            {
                return i; // 저주 상태에 걸린 소환수의 인덱스 반환
            }
        }
        return -1; // 저주 상태에 걸린 소환수가 없으면 -1 반환
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

    // 적의 체력이 모두 50% 이상인가?
    public bool AllEnemiesHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.5)
            {
                return false;
            }
        }
        return true;
    }

    // 아군의 등급이 모두 하급과 중급인가?
    public bool AllSummonsLowOrMediumRank(List<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
            if (playerSummon != null && (playerSummon.getSummonRank() != SummonRank.Low && playerSummon.getSummonRank() != SummonRank.Medium))
            {
                return false;
            }
        }
        return true;
    }

    // 적의 체력이 하나만 30% 아래인가?
    public bool isAnyEnemyHealthDown30Percent(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.3)
            {
                return true;
            }
        }
        return false;
    }

    // 적의 체력이 70% 이상인가?
    public bool isAnyEnemyHealthOver70Percent(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() > 0.7)
            {
                return true;
            }
        }
        return false;
    }

    // 가장 공격력이 높은 소환수의 플레이트 인덱스를 반환하는 메소드
    public int getIndexOfHighestAttackPower(List<Plate> playerPlates)
    {
        int highestAttackIndex = -1;
        double highestAttackPower = double.MinValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].getCurrentSummon();
            if (summon != null)
            {
                double attackPower = summon.getAttackPower();
                if (attackPower > highestAttackPower)
                {
                    highestAttackPower = attackPower;
                    highestAttackIndex = i; // 가장 높은 공격력을 가진 소환수의 인덱스를 기록
                }
            }
        }

        return highestAttackIndex; // 가장 높은 공격력의 소환수 인덱스 반환
    }



    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하는 메소드
    public bool CanNormalAttackKill(Summon fox, List<Plate> enermyPlates)
    {
        int closestIndex = getClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            if (closestEnermySummon != null && fox.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return true; // 가장 가까운 적을 일반 공격으로 처치할 수 있으면 true 반환
            }
        }
        return false; // 처치할 수 없으면 false 반환
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
