using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlateView))]
public class PlateBoardView : MonoBehaviour
{
    [SerializeField] private List<BattleBoardInputController> playerPlates;
    [SerializeField] private List<BattleBoardInputController> enemyPlates;

    private PlateView plateView;
    private List<BattleBoardInputController> plates = new List<BattleBoardInputController>();
    private BattleBoardData battleBoardData;

    private void Awake()
    {
        EnsurePlateView();
        InitializePlates();
        EnsureBattleBoardData();
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
        plates.AddRange(enemyPlates);
    }

    private void EnsureBattleBoardData()
    {
        if (battleBoardData != null)
        {
            return;
        }

        List<PlateData> playerPlateData = CreatePlateData(playerPlates);
        List<PlateData> enemyPlateData = CreatePlateData(enemyPlates);
        battleBoardData = new BattleBoardData(playerPlateData, enemyPlateData);
    }

    private static List<PlateData> CreatePlateData(
        IReadOnlyList<BattleBoardInputController> boardInputs)
    {
        var plateDataList = new List<PlateData>();
        if (boardInputs == null)
        {
            return plateDataList;
        }

        for (int index = 0; index < boardInputs.Count; index++)
        {
            var plateData = new PlateData(index);
            plateDataList.Add(plateData);
            boardInputs[index]?.ConnectPlateData(plateData);
        }

        return plateDataList;
    }

    public bool IsEnemyPlateClear()
    {
        return GetBattleBoardData().AreEnemyPlatesClear();
    }

    public bool IsPlayerPlateClear()
    {
        return GetBattleBoardData().ArePlayerPlatesClear();
    }

    public int GetClosestPlayerPlateIndexExcept(Summon attackingSummon)
    {
        return GetBattleBoardData().FindClosestPlayerPlateIndex(attackingSummon);
    }

    public int GetClosestPlayerPlateIndex()
    {
        return GetBattleBoardData().FindClosestPlayerPlateIndex();
    }

    public int GetLowestHealthEnemyPlateIndex()
    {
        return GetBattleBoardData().FindLowestHealthEnemyPlateIndex();
    }

    public int GetFirstEmptyPlayerPlateIndex()
    {
        return GetBattleBoardData().FindFirstEmptyPlayerPlateIndex();
    }

    public void CompactEnemyPlates()
    {
        CompactPlates(enemyPlates);
    }

    private void CompactPlates(IReadOnlyList<BattleBoardInputController> targetPlates)
    {
        if (targetPlates == null)
        {
            return;
        }

        int nextAvailableIndex = 0;

        for (int index = 0; index < targetPlates.Count; index++)
        {
            Summon summon = targetPlates[index].GetCurrentSummon();
            if (summon == null)
            {
                continue;
            }

            if (index != nextAvailableIndex)
            {
                targetPlates[index].RemoveSummon();
                targetPlates[nextAvailableIndex].DirectMoveSummon(summon);
            }

            nextAvailableIndex++;
        }
    }

    public void DownTransparencyForWhoPlate(bool isPlayer)
    {
        plateView.DownTransparencyForOccupiedPlates(isPlayer ? playerPlates : enemyPlates);
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

    private void HideEnemyPlates()
    {
        plateView.HidePlates(enemyPlates);
    }

    private void ShowEnemyPlates()
    {
        plateView.ShowPlates(enemyPlates);
    }

    private void ResetEnemyPlateHighlight()
    {
        plateView.ResetOccupiedPlateHighlight(enemyPlates);
        ShowPlayerPlates();
    }

    public void ResetAllPlateHighlight()
    {
        ResetPlayerPlateHighlight();
        ResetEnemyPlateHighlight();
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

    public int GetPlayerPlateIndex(BattleBoardInputController plate)
    {
        return playerPlates == null ? -1 : playerPlates.IndexOf(plate);
    }

    public int GetEnemyPlateIndex(BattleBoardInputController plate)
    {
        return enemyPlates == null ? -1 : enemyPlates.IndexOf(plate);
    }

    private int GetAttackTargetPlateIndex(BattleBoardInputController plate, bool targetsPlayerPlate)
    {
        return targetsPlayerPlate
            ? GetPlayerPlateIndex(plate)
            : GetEnemyPlateIndex(plate);
    }

    private string GetAttackTargetPlateName(bool targetsPlayerPlate)
    {
        return targetsPlayerPlate ? "player" : "enemy";
    }

    public bool TryGetAttackTargetPlate(
        BattleBoardInputController plate,
        bool targetsPlayerPlate,
        out int plateIndex,
        out string plateName)
    {
        plateIndex = GetAttackTargetPlateIndex(plate, targetsPlayerPlate);
        plateName = GetAttackTargetPlateName(targetsPlayerPlate);

        return plateIndex >= 0;
    }

    public IReadOnlyList<BattleBoardInputController> GetPlayerPlates()
    {
        return playerPlates;
    }

    public IReadOnlyList<BattleBoardInputController> GetEnemyPlates()
    {
        return enemyPlates;
    }

    public BattleBoardData GetBattleBoardData()
    {
        EnsureBattleBoardData();
        return battleBoardData;
    }
}
