using System;
using System.Collections.Generic;

public sealed class SelectSummonUseCase
{
    public bool Execute(
        IReadOnlyList<BattleBoardInputController> playerPlates,
        int plateIndex,
        Summon selectedSummon,
        bool isRedraw,
        Action onSummonPlaced)
    {
        if (playerPlates == null || selectedSummon == null)
        {
            return false;
        }

        if (plateIndex < 0 || plateIndex >= playerPlates.Count)
        {
            return false;
        }

        playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon, isRedraw);
        ApplyPlayerStageMultiplier(playerPlates[plateIndex].GetCurrentSummon());
        onSummonPlaced?.Invoke();
        return true;
    }

    private void ApplyPlayerStageMultiplier(Summon placedSummon)
    {
        if (placedSummon == null)
        {
            return;
        }

        placedSummon.ApplayMultiple(Summon.GetStatMultiplier());
    }
}
