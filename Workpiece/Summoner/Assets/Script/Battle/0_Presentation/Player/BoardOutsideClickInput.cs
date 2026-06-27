using System.Collections.Generic;
using UnityEngine;

public sealed class BoardOutsideClickInput
{
    public bool IsOutsideClick(IReadOnlyList<BattleBoardInputController> targetPlates)
    {
        return Input.GetMouseButtonDown(0) && !MousePositionInsidePlates(targetPlates);
    }

    private bool MousePositionInsidePlates(IReadOnlyList<BattleBoardInputController> targetPlates)
    {
        Vector2 mousePosition = Input.mousePosition;

        foreach (BattleBoardInputController plate in targetPlates)
        {
            RectTransform plateRect = plate.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(plateRect, mousePosition))
            {
                return true;
            }
        }

        return false;
    }
}
