using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAttackPrediction : MonoBehaviour
{
    private PlateController plateController;
    private void Awake()
    {
        plateController = GetComponent<PlateController>();
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

    //특수스킬로 죽일 수 있는 인덱스 반환
    public int getIndexofSpecialCanKill(Summon attackingSummon, List<Plate> enermyPlates)
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
