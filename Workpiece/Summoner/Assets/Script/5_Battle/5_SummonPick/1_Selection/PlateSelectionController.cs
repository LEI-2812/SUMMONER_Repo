using UnityEngine;

// 역할: 재소환할 플레이어 플레이트 선택 가능 여부와 선택 표시를 관리한다.
public class PlateSelectionController
{
    private readonly PlateController plateController;

    public PlateSelectionController(PlateController plateController)
    {
        this.plateController = plateController;
    }

    public bool CanStartRedrawSelection()
    {
        if (plateController.IsPlayerPlateClear())
        {
            Debug.Log("플레이트에 소환수가 없습니다.");
            return false;
        }

        return true;
    }

    public void ShowRedrawSelectablePlates()
    {
        plateController.HighlightPlayerPlates();
    }

    public void RestorePlayerPlateSelectionView()
    {
        foreach (Plate plate in plateController.GetPlayerPlates())
        {
            if (!plate.GetIsInSummon())
            {
                continue;
            }

            plate.Unhighlight();
            plate.SetSummonImageTransparency(1.0f);
        }
    }

    public bool TryGetPlayerPlateIndex(Plate plate, out int selectedPlateIndex)
    {
        selectedPlateIndex = plateController.GetPlayerPlateIndex(plate);
        if (selectedPlateIndex >= 0)
        {
            return true;
        }

        Debug.Log("선택한 플레이트가 플레이어 플레이트가 아닙니다.");
        return false;
    }
}
