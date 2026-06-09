using UnityEngine;

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

    public void ShowRedrawSelectablePlates(GameObject darkBackground)
    {
        darkBackground.SetActive(true);
        plateController.HighlightPlayerPlates();
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
