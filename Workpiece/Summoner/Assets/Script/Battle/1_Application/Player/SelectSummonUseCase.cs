public readonly struct SelectSummonResult
{
    private SelectSummonResult(
        bool didSelect,
        int plateIndex,
        Summon selectedSummon,
        bool isRedraw)
    {
        DidSelect = didSelect;
        PlateIndex = plateIndex;
        SelectedSummon = selectedSummon;
        IsRedraw = isRedraw;
    }

    public bool DidSelect { get; }
    public int PlateIndex { get; }
    public Summon SelectedSummon { get; }
    public bool IsRedraw { get; }

    public static SelectSummonResult Failed()
    {
        return new SelectSummonResult(false, -1, null, false);
    }

    public static SelectSummonResult Selected(
        int plateIndex,
        Summon selectedSummon,
        bool isRedraw)
    {
        return new SelectSummonResult(true, plateIndex, selectedSummon, isRedraw);
    }
}

public class SelectSummonUseCase
{
    public SelectSummonResult Execute(
        int playerPlateCount,
        int plateIndex,
        Summon selectedSummon,
        bool isRedraw)
    {
        if (selectedSummon == null || plateIndex < 0 || plateIndex >= playerPlateCount)
        {
            return SelectSummonResult.Failed();
        }

        return SelectSummonResult.Selected(plateIndex, selectedSummon, isRedraw);
    }
}
