using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlateView))]
public class PlateController : MonoBehaviour
{

    [SerializeField] private List<Plate> playerPlates;
    [SerializeField] private List<Plate> enermyPlates;
    private PlateView plateView;

    private List<Plate> plates = new List<Plate>();

    // [Header("(외부 오브젝트)컨트롤러")]

    private void Awake()
    {
        EnsurePlateView();
        InitializePlates();
    }

    // 플레이어의 플레이트에 있는 모든 소환수들을 반환하는 메소드
    public List<Summon> GetPlayerSummons()
    {
        return GetSummonsFromPlates(playerPlates);
    }

    // 적의 플레이트에 있는 모든 소환수들을 반환하는 메소드
    public List<Summon> GetEnermySummons()
    {
        return GetSummonsFromPlates(enermyPlates);
    }

    //적 플레이트에 소환수가 존재하는지
    public bool IsEnermyPlateClear()
    {
        return ArePlatesClear(enermyPlates);
    }

    public bool IsPlayerPlateClear()
    {
        return ArePlatesClear(playerPlates);
    }

    // 적 소환수의 빈 플레이트를 앞당기는 로직
    public void CompactEnermyPlates()
    {
        int nextAvailableIndex = 0; // 채워야 할 인덱스 위치

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon summon = enermyPlates[i].GetCurrentSummon();
            if (summon != null)
            {
                // 만약 현재 인덱스와 nextAvailableIndex가 다르면 소환수를 앞으로 옮긴다.
                if (i != nextAvailableIndex)
                {
                    // 현재 소환수를 nextAvailableIndex 위치로 직접 이동
                    enermyPlates[nextAvailableIndex].DirectMoveSummon(summon);
                    enermyPlates[i].RemoveSummon(); // 원래 위치의 소환수를 제거
                }
                nextAvailableIndex++; // 다음 위치로 이동
            }
        }
    }

    public void DownTransparencyForWhoPlate(bool isPlayer)
    {
        plateView.DownTransparencyForOccupiedPlates(isPlayer ? playerPlates : enermyPlates);
    }



    // 플레이어의 소환수가 있는 플레이트만 강조 및 투명도 설정
    public void HighlightPlayerPlates()
    {
        plateView.HighlightOccupiedPlates(playerPlates);

        // 적 플레이트는 숨기기
        HideEnemyPlates();
    }

    // 강조를 해제하고 투명도를 기본값으로 되돌리기
    public void ResetPlayerPlateHighlight()
    {
        plateView.ResetOccupiedPlateHighlight(playerPlates);

        // 적 플레이트를 다시 보이게 하기
        ShowEnemyPlates();
    }

    // 적의 플레이트를 숨기는 메서드
    public void HideEnemyPlates()
    {
        plateView.HidePlates(enermyPlates);
    }

    // 적의 플레이트를 다시 보이게 하는 메서드
    private void ShowEnemyPlates()
    {
        plateView.ShowPlates(enermyPlates);
    }

    // 플레이어의 소환수가 있는 플레이트만 강조 및 투명도 설정
    public void HighlightEnermyPlates()
    {
        plateView.HighlightOccupiedPlates(enermyPlates);

        // 적 플레이트는 숨기기
        HidePlayerPlates();
    }

    // 강조를 해제하고 투명도를 기본값으로 되돌리기
    public void ResetEnermyPlateHighlight()
    {
        plateView.ResetOccupiedPlateHighlight(enermyPlates);

        // 적 플레이트를 다시 보이게 하기
        ShowPlayerPlates();
    }

    public void ResetAllPlateHighlight()
    {
        ResetPlayerPlateHighlight();
        ResetEnermyPlateHighlight();
    }

    // 적의 플레이트를 숨기는 메서드
    public void HidePlayerPlates()
    {
        plateView.HidePlates(playerPlates);
    }

    // 적의 플레이트를 다시 보이게 하는 메서드
    private void ShowPlayerPlates()
    {
        plateView.ShowPlates(playerPlates);
    }

    public void HideAllPlates()
    {
        plateView.HidePlates(plates);
    }

    public void ShowAllPlates()
    {
        plateView.ShowPlates(plates);
    }

    public int GetClosestPlayerPlateIndexExcept(Summon attackingSummon) //플레이어 플레이트중 가장 가까이 있는 소환수의 인덱스를 반환
    {
        return FindClosestOccupiedPlateIndex(playerPlates, attackingSummon);
    }

    public int GetClosestPlayerPlateIndex()
    {
        return FindClosestOccupiedPlateIndex(playerPlates, null);
    }

    public int GetClosestEnermyPlateIndexExcept(Summon attackingSummon) //적 플레이트중 가장 가까이 있는 소환수의 인덱스를 반환
    {
        return FindClosestOccupiedPlateIndex(enermyPlates, attackingSummon);
    }

    public int GetPlayerSummonCount()
    {
        return CountSummonsOnPlates(playerPlates);
    }

    public int GetEnermySummonCount()
    {
        return CountSummonsOnPlates(enermyPlates);
    }



    // 특정 플레이트의 인덱스를 반환하는 메소드
    public int GetPlateIndex(Plate plate)
    {
        // 플레이어 플레이트에서 탐색
        int index = playerPlates.IndexOf(plate);
        if (index != -1)
        {
            return index; // 해당 플레이트의 인덱스 반환
        }

        // 적 플레이트에서 탐색
        index = enermyPlates.IndexOf(plate);
        if (index != -1)
        {
            return index; // 해당 플레이트의 인덱스 반환
        }

        return -1; // 플레이트가 목록에 없을 경우 -1 반환
    }



    // 아군 플레이트 중 가장 체력이 낮은 소환수의 인덱스를 반환하는 메소드
    public int GetLowestHealthPlayerPlateIndex()
    {
        return FindLowestHealthPlateIndex(playerPlates);
    }

    // 적 플레이트 중 가장 체력이 낮은 소환수의 인덱스를 반환하는 메소드
    public int GetLowestHealthEnermyPlateIndex()
    {
        return FindLowestHealthPlateIndex(enermyPlates);
    }

    private List<Summon> GetSummonsFromPlates(List<Plate> targetPlates)
    {
        List<Summon> summons = new List<Summon>();

        foreach (Plate plate in targetPlates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon != null)
            {
                summons.Add(summon);
            }
        }

        return summons;
    }

    private bool ArePlatesClear(List<Plate> targetPlates)
    {
        foreach (Plate plate in targetPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                return false;
            }
        }

        return true;
    }

    private int FindClosestOccupiedPlateIndex(List<Plate> targetPlates, Summon exceptSummon)
    {
        for (int i = 0; i < targetPlates.Count; i++)
        {
            Summon currentSummon = targetPlates[i].GetCurrentSummon();
            if (currentSummon != null && currentSummon != exceptSummon)
            {
                return i;
            }
        }

        return -1;
    }

    private int CountSummonsOnPlates(List<Plate> targetPlates)
    {
        int summonCount = 0;

        foreach (Plate plate in targetPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                summonCount++;
            }
        }

        return summonCount;
    }

    private int FindLowestHealthPlateIndex(List<Plate> targetPlates)
    {
        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < targetPlates.Count; i++)
        {
            Summon currentSummon = targetPlates[i].GetCurrentSummon();
            if (currentSummon != null)
            {
                double currentHealth = currentSummon.GetNowHP();
                if (currentHealth < lowestHealth)
                {
                    lowestHealth = currentHealth;
                    lowestHealthIndex = i;
                }
            }
        }

        return lowestHealthIndex;
    }



    private void InitializePlates()
    {
        // playerPlates와 EnermyPlates의 원본 리스트의 내용을 plates 리스트에 추가
        plates.AddRange(playerPlates);
        plates.AddRange(enermyPlates);
    }

    private void EnsurePlateView()
    {
        if (plateView != null)
        {
            return;
        }

        plateView = GetComponent<PlateView>();
        if (plateView == null)
        {
            plateView = gameObject.AddComponent<PlateView>();
        }
    }

    public List<Plate> GetPlayerPlates()
    {
        return playerPlates;
    }

    public bool ContainsPlayerPlate(Plate plate)
    {
        return playerPlates != null && playerPlates.Contains(plate);
    }

    public bool ContainsEnermyPlate(Plate plate)
    {
        return enermyPlates != null && enermyPlates.Contains(plate);
    }

    public int GetPlayerPlateIndex(Plate plate)
    {
        return playerPlates == null ? -1 : playerPlates.IndexOf(plate);
    }

    public int GetEnermyPlateIndex(Plate plate)
    {
        return enermyPlates == null ? -1 : enermyPlates.IndexOf(plate);
    }

    public bool CanSelectAttackTargetPlate(Plate plate, bool targetsPlayerPlate)
    {
        return targetsPlayerPlate
            ? ContainsPlayerPlate(plate)
            : ContainsEnermyPlate(plate);
    }

    public int GetAttackTargetPlateIndex(Plate plate, bool targetsPlayerPlate)
    {
        return targetsPlayerPlate
            ? GetPlayerPlateIndex(plate)
            : GetEnermyPlateIndex(plate);
    }

    public string GetAttackTargetPlateName(bool targetsPlayerPlate)
    {
        return targetsPlayerPlate ? "아군" : "적";
    }

    public bool TryGetAttackTargetPlate(
        Plate plate,
        bool targetsPlayerPlate,
        out int plateIndex,
        out string plateName)
    {
        plateIndex = GetAttackTargetPlateIndex(plate, targetsPlayerPlate);
        plateName = GetAttackTargetPlateName(targetsPlayerPlate);

        return plateIndex >= 0;
    }

    public List<Plate> GetEnermyPlates()
    {
        return enermyPlates;
    }
    // 플레이어 플레이트 리스트를 설정하는 메서드
    public void SetPlayerPlates(List<Plate> plates)
    {
        playerPlates = plates;
    }

    // 적 플레이트 리스트를 설정하는 메서드
    public void SetEnermyPlates(List<Plate> plates)
    {
        enermyPlates = plates;
    }
}
