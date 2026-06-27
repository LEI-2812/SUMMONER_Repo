using System.Collections.Generic;
using UnityEngine;

// 역할: PlateView의 책임을 정의한다.
public class PlateView : MonoBehaviour
{
    public void HidePlates(List<BattleBoardInputController> plates)
    {
        SetPlatesActive(plates, false);
    }

    public void ShowPlates(List<BattleBoardInputController> plates)
    {
        SetPlatesActive(plates, true);
    }

    public void DownTransparencyForOccupiedPlates(List<BattleBoardInputController> plates)
    {
        SetOccupiedPlateTransparency(plates, 0.5f);
    }

    public void HighlightOccupiedPlates(List<BattleBoardInputController> plates)
    {
        if (plates == null)
        {
            return;
        }

        foreach (BattleBoardInputController plate in plates)
        {
            if (!HasSummon(plate))
            {
                continue;
            }

            plate.Highlight();
            plate.SetSummonImageTransparency(0.5f);
        }
    }

    public void ResetOccupiedPlateHighlight(List<BattleBoardInputController> plates)
    {
        if (plates == null)
        {
            return;
        }

        foreach (BattleBoardInputController plate in plates)
        {
            if (!HasSummon(plate))
            {
                continue;
            }

            plate.Unhighlight();
            plate.SetSummonImageTransparency(1.0f);
        }
    }

    private void SetPlatesActive(List<BattleBoardInputController> plates, bool active)
    {
        if (plates == null)
        {
            return;
        }

        foreach (BattleBoardInputController plate in plates)
        {
            if (plate == null)
            {
                continue;
            }

            plate.gameObject.SetActive(active);
        }
    }

    private void SetOccupiedPlateTransparency(List<BattleBoardInputController> plates, float alpha)
    {
        if (plates == null)
        {
            return;
        }

        foreach (BattleBoardInputController plate in plates)
        {
            if (!HasSummon(plate))
            {
                continue;
            }

            plate.SetSummonImageTransparency(alpha);
        }
    }

    private bool HasSummon(BattleBoardInputController plate)
    {
        return plate != null && plate.GetCurrentSummon() != null;
    }
}
