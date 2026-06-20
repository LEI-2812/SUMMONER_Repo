// 역할: 플레이어 턴 흐름에서 TurnController와 BattleResultController 호출을 담당한다.
public class PlayerTurnActions
{
    private readonly TurnController turnController;
    private readonly BattleResultController battleResultController;

    public PlayerTurnActions(
        TurnController turnController,
        BattleResultController battleResultController)
    {
        this.turnController = turnController;
        this.battleResultController = battleResultController;
    }

    public int GetPlayerClearTurn()
    {
        return turnController.GetClearTurn();
    }

    public void StartPlayerTurn(PlayerTurnProgressState turnProgressState)
    {
        turnProgressState.SetTurnProgress(
            turnController.GetClearTurn(),
            turnController.GetTurnCount());
    }

    public bool CanEndPlayerTurn()
    {
        return turnController.GetCurrentTurn() == TurnController.Turn.PlayerTurn;
    }

    public bool TryEndPlayerTurn()
    {
        if (!CanEndPlayerTurn())
        {
            return false;
        }

        turnController.EndCurrentTurn();
        return true;
    }

    public void CheckPlayerFailResult(PlayerTurnProgressState turnProgressState)
    {
        battleResultController.FailResultTry(
            turnProgressState.ClearTurn,
            turnProgressState.CurrentTurn);
    }

    public void CheckPlayerClearResult(PlayerTurnProgressState turnProgressState)
    {
        battleResultController.ClearResultTry(
            turnProgressState.IsEnemyPlateClear,
            turnProgressState.ClearTurn,
            turnProgressState.CurrentTurn);
    }
}
