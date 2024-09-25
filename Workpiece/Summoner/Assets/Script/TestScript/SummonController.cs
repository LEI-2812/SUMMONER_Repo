using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Burst.Intrinsics;

public class SummonController : MonoBehaviour
{
    public static SummonController Instance; //싱글톤
    public List<Summon> summons; //인스펙터에 넣을 소환수 오브젝트들
    public GameObject TakeSummonPanel;

    private Summon selectedSummon; // 선택된 소환수

    public List<PickSummonPanel> SelectSummonPanels; //판넬에 띄울 소환수
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void randomTakeSummon()
    {

        // 50%, 35%, 15% 확률로 소환수 선택 후 패널에 할당
        List<Summon> randomSelectedSummons = SummonRandomly();

        // 선택된 소환수를 각 패널에 할당
        for (int i = 0; i < SelectSummonPanels.Count && i < randomSelectedSummons.Count; i++)
        {
            Summon summon = randomSelectedSummons[i];
            SelectSummonPanels[i].assignedSummon = summon; // 소환수를 패널에 할당
                                                           // 이미지가 null인지 체크
            if (summon.image != null && summon.image.sprite != null)
            {
                SelectSummonPanels[i].SetSummonImage(summon.image); // 소환수의 스프라이트를 패널에 할당
            }
            else
            {
                Debug.LogWarning($"소환수 {summon.summonName}의 이미지가 설정되지 않았습니다.");
            }
        }
        // 이전 선택된 소환수를 초기화
        selectedSummon = null;

    }

    // 3마리의 소환수를 확률에 따라 선택하는 메소드
    private List<Summon> SummonRandomly()
    {
        List<Summon> selectedSummons = new List<Summon>(); // 소환 판넬에 보이게 할 소환수들

        // Low, Medium, High 확률에 맞춰 3마리의 소환수 선택
        for (int i = 0; i < 3; i++)
        {
            Summon summon = SelectSummonByRank();
            if (summon != null)
            {
                selectedSummons.Add(summon);
            }
        }

        return selectedSummons;
    }

    // 등급에 따른 확률로 소환수를 뽑음
    private Summon SelectSummonByRank()
    {
        float randomValue = Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값

        if (randomValue <= 50) // Low 등급 (50%)
        {
            selectedSummon = GetSummonByRank(SummonRank.Low);
        }
        else if (randomValue <= 85) // Medium 등급 (35%)
        {
            selectedSummon = GetSummonByRank(SummonRank.Medium);
        }
        else // High 등급 (15%)
        {
            selectedSummon = GetSummonByRank(SummonRank.High);
        }

        return selectedSummon;
    }

    // 특정 등급의 소환수 중 하나를 무작위로 선택하는 메소드
    private Summon GetSummonByRank(SummonRank rank)
    {
        List<Summon> availableSummons = new List<Summon>();

        // 소환수 리스트에서 해당 등급의 소환수들만 필터링
        foreach (Summon summon in summons)
        {
            if (summon.summonRank == rank)
            {
                availableSummons.Add(summon);
            }
        }

        // 해당 등급의 소환수가 존재할 경우, 그 중에서 무작위로 하나 선택
        if (availableSummons.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSummons.Count);
            return availableSummons[randomIndex];
        }

        return null; // 해당 등급의 소환수가 없을 경우 null 반환
    }

    // 소환수를 선택하는 메소드
    public void OnSelectSummon(Summon summon)
    {
        selectedSummon = summon; // 선택된 소환수를 저장
        Debug.Log($"{selectedSummon.summonName} 소환수를 선택했습니다.");
        TakeSummonPanel.SetActive(false);
    }

    // 선택된 소환수를 반환하는 메소드 (Player 클래스에서 사용)
    public Summon GetSelectedSummon()
    {
        return selectedSummon;
    }
}
