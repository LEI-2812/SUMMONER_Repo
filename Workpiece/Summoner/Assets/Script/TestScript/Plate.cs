using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    //plate를 프리팹시켜서 넣을것.
    public bool isInSummon = false; // 현재 소환수가 있는지 여부
    public Summon CurrentSummon;   // 플레이트 위에 있는 소환수

    // 소환수를 플레이트에 배치
    public void SummonPlaceOnPlate(Summon summon)
    {
        if (!isInSummon)
        {
            CurrentSummon = summon;
            isInSummon = true;
            Debug.Log($"소환수 {summon.summonName} 을 플레이트에 소환");
        }
        else
        {
            Debug.Log("이미 이 플레이트에 소환수가 있습니다.");
        }
    }

    // 소환수가 사망하거나 플레이트에서 떠날 때
    public void RemoveSummon()
    {
        if (isInSummon)
        {
            CurrentSummon = null;
            isInSummon = false;
            Debug.Log("소환수 제거.");
        }
    }

    // 체력 체크 (플레이트 위 소환수의 체력 확인)
    public void CheckHealth()
    {
        if (CurrentSummon != null)
        {
            Debug.Log($"소환수 {CurrentSummon.summonName} 의 체력: {CurrentSummon.health}");
        }
    }
}
