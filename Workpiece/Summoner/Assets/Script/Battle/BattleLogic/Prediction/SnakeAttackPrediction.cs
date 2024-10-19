using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
    }




    // 적이 이미 중독 상태인지 확인하는 메소드
    public bool IsEnermyAlreadyPoisoned(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getAllStatusTypes().Contains(StatusType.Poison))
            {
                return true;
            }
        }
        return false;
    }


    // 특수 공격을 사용할 수 있는지 확인하는 메소드
    public bool canUseSpecialAttack(Summon snake)
    {
        var availableSpecialAttacks = snake.getAvailableSpecialAttacks();
        return availableSpecialAttacks.Length > 0;
    }



    // 모든 적의 체력이 50% 이상인지 확인하는 메소드
    public bool AllEnermyHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio < 0.5)
                {
                    return false; // 하나라도 50% 이하이면 false 반환
                }
            }
        }
        return true;
    }



    // 적 중에 공격 개수가 4개 이상인 소환수가 있는지 확인하는 메소드
    public bool hasMonsterWithMoreThan4Attacks(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getSpecialAttackCount() >= 4)
            {
                return true;
            }
        }
        return false;
    }


    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하는 메소드
    public bool CanNormalAttackKill(Summon snake, List<Plate> enermyPlates)
    {
        int closestIndex = getClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            if (closestEnermySummon != null && snake.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return true; // 가장 가까운 적을 일반 공격으로 처치할 수 있으면 true 반환
            }
        }
        return false; // 처치할 수 없으면 false 반환
    }


    // 적이 2마리 이상 있는지 확인하는 메소드
    public bool isEnermyCountOverTwo(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null)
            {
                count++;
                if (count >= 2) return true;
            }
        }
        return false;
    }

    // 적이 1마리 이상 있는지 확인하는 메소드
    public bool isEnermyCountOnlyOne(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null)
            {
                return true;
            }
        }
        return false;
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
