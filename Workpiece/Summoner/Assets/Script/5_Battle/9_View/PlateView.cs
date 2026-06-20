using System.Collections.Generic;
using UnityEngine;

// 역할: 플레이트 강조, 숨김, 투명도 같은 화면 표시를 담당한다.
public class PlateView : MonoBehaviour
{
    public void HidePlates(List<Plate> plates)
    {
        SetPlatesActive(plates, false);
    }

    public void ShowPlates(List<Plate> plates)
    {
        SetPlatesActive(plates, true);
    }

    public void DownTransparencyForOccupiedPlates(List<Plate> plates)
    {
        SetOccupiedPlateTransparency(plates, 0.5f);
    }

    public void HighlightOccupiedPlates(List<Plate> plates)
    {
        if (plates == null)
        {
            return;
        }

        foreach (Plate plate in plates)
        {
            if (!HasSummon(plate))
            {
                continue;
            }

            plate.Highlight();
            plate.SetSummonImageTransparency(0.5f);
        }
    }

    public void ResetOccupiedPlateHighlight(List<Plate> plates)
    {
        if (plates == null)
        {
            return;
        }

        foreach (Plate plate in plates)
        {
            if (!HasSummon(plate))
            {
                continue;
            }

            plate.Unhighlight();
            plate.SetSummonImageTransparency(1.0f);
        }
    }

    private void SetPlatesActive(List<Plate> plates, bool active)
    {
        if (plates == null)
        {
            return;
        }

        foreach (Plate plate in plates)
        {
            if (plate == null)
            {
                continue;
            }

            plate.gameObject.SetActive(active);
        }
    }

    private void SetOccupiedPlateTransparency(List<Plate> plates, float alpha)
    {
        if (plates == null)
        {
            return;
        }

        foreach (Plate plate in plates)
        {
            if (!HasSummon(plate))
            {
                continue;
            }

            plate.SetSummonImageTransparency(alpha);
        }
    }

    private bool HasSummon(Plate plate)
    {
        return plate != null && plate.GetCurrentSummon() != null;
    }
}
