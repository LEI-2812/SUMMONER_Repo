using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
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
    public bool CanNormalAttackKill(Summon rabbit, List<Plate> enermyPlates)
    {
        int closestIndex = getClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            if (closestEnermySummon != null && rabbit.getAttackPower() >= closestEnermySummon.getNowHP())
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
