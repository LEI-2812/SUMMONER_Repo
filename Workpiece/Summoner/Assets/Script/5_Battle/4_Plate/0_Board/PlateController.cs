using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlateView))]
// 역할: 플레이어/적 플레이트 목록을 관리하고 타겟 탐색과 플레이트 표시 제어를 제공한다.
public class PlateController : MonoBehaviour
{
    [SerializeField] private List<Plate> playerPlates;
    [SerializeField] private List<Plate> enermyPlates;

    private PlateView plateView;
    private readonly PlateCompactExecutor compactExecutor = new PlateCompactExecutor();
    private readonly PlateQueryService queryService = new PlateQueryService();
    private List<Plate> plates = new List<Plate>();

    private void Awake()
    {
        EnsurePlateView();
        InitializePlates();
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

    private void InitializePlates()
    {
        plates.AddRange(playerPlates);
        plates.AddRange(enermyPlates);
    }

    // 판 상태 조회
    public List<Summon> GetPlayerSummons()
    {
        return queryService.GetSummonsFromPlates(playerPlates);
    }

    public List<Summon> GetEnermySummons()
    {
        return queryService.GetSummonsFromPlates(enermyPlates);
    }

    public bool IsEnermyPlateClear()
    {
        return queryService.ArePlatesClear(enermyPlates);
    }

    public bool IsPlayerPlateClear()
    {
        return queryService.ArePlatesClear(playerPlates);
    }

    public int GetClosestPlayerPlateIndexExcept(Summon attackingSummon)
    {
        return queryService.FindClosestOccupiedPlateIndex(playerPlates, attackingSummon);
    }

    public int GetClosestPlayerPlateIndex()
    {
        return queryService.FindClosestOccupiedPlateIndex(playerPlates, null);
    }

    public int GetClosestEnermyPlateIndexExcept(Summon attackingSummon)
    {
        return queryService.FindClosestOccupiedPlateIndex(enermyPlates, attackingSummon);
    }

    public int GetPlayerSummonCount()
    {
        return queryService.CountSummonsOnPlates(playerPlates);
    }

    public int GetEnermySummonCount()
    {
        return queryService.CountSummonsOnPlates(enermyPlates);
    }

    public int GetLowestHealthPlayerPlateIndex()
    {
        return queryService.FindLowestHealthPlateIndex(playerPlates);
    }

    public int GetLowestHealthEnermyPlateIndex()
    {
        return queryService.FindLowestHealthPlateIndex(enermyPlates);
    }

    public int GetFirstEmptyPlayerPlateIndex()
    {
        return queryService.FindFirstEmptyPlateIndex(playerPlates);
    }

    // 판 표시 제어
    public void CompactEnermyPlates()
    {
        compactExecutor.CompactPlates(enermyPlates);
    }

    public void DownTransparencyForWhoPlate(bool isPlayer)
    {
        plateView.DownTransparencyForOccupiedPlates(isPlayer ? playerPlates : enermyPlates);
    }

    public void HighlightPlayerPlates()
    {
        plateView.HighlightOccupiedPlates(playerPlates);
        HideEnemyPlates();
    }

    public void ResetPlayerPlateHighlight()
    {
        plateView.ResetOccupiedPlateHighlight(playerPlates);
        ShowEnemyPlates();
    }

    public void HideEnemyPlates()
    {
        plateView.HidePlates(enermyPlates);
    }

    private void ShowEnemyPlates()
    {
        plateView.ShowPlates(enermyPlates);
    }

    public void HighlightEnermyPlates()
    {
        plateView.HighlightOccupiedPlates(enermyPlates);
        HidePlayerPlates();
    }

    public void ResetEnermyPlateHighlight()
    {
        plateView.ResetOccupiedPlateHighlight(enermyPlates);
        ShowPlayerPlates();
    }

    public void ResetAllPlateHighlight()
    {
        ResetPlayerPlateHighlight();
        ResetEnermyPlateHighlight();
    }

    public void HidePlayerPlates()
    {
        plateView.HidePlates(playerPlates);
    }

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

    // 공격 대상 판정
    public int GetPlateIndex(Plate plate)
    {
        int index = playerPlates.IndexOf(plate);
        if (index != -1)
        {
            return index;
        }

        index = enermyPlates.IndexOf(plate);
        if (index != -1)
        {
            return index;
        }

        return -1;
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

    // 리스트 접근
    public IReadOnlyList<Plate> GetPlayerPlates()
    {
        return playerPlates;
    }

    public IReadOnlyList<Plate> GetEnermyPlates()
    {
        return enermyPlates;
    }
}
