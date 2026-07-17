class PlayerTurnBoard : IPlayerTurnBoard
{
    private readonly PlateBoardView plateBoardController;

    public PlayerTurnBoard(PlateBoardView plateBoardController)
    {
        this.plateBoardController = plateBoardController;
    }

    public void CompactEnemyPlates()
    {
        plateBoardController.CompactEnemyPlates();
    }

    public bool IsEnemyPlateClear()
    {
        return plateBoardController.IsEnemyPlateClear();
    }

    public void ResetPlateHighlight()
    {
        plateBoardController.ResetAllPlateHighlight();
    }
}
