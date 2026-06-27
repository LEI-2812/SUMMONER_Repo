public sealed class PlayerTargetSelectionView
{
    private readonly SummonSelectionController summonSelectionController;
    private readonly PlateBoardView plateBoardController;

    public PlayerTargetSelectionView(
        SummonSelectionController summonSelectionController,
        PlateBoardView plateBoardController)
    {
        this.summonSelectionController = summonSelectionController;
        this.plateBoardController = plateBoardController;
    }

    public void ShowTargetSelection(bool downTransparencyForPlayerPlate)
    {
        summonSelectionController.OnDarkBackground(true);
        plateBoardController.DownTransparencyForWhoPlate(downTransparencyForPlayerPlate);
    }

    public void HideTargetSelection()
    {
        summonSelectionController.OnDarkBackground(false);
    }

    public void ResetTargetSelection()
    {
        plateBoardController.ResetAllPlateHighlight();
    }
}
