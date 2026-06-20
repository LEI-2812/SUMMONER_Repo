// 역할: 현재 턴과 턴 수 진행 상태만 보관한다.
internal sealed class TurnProgressState
{
    public TurnController.Turn CurrentTurn { get; private set; }
    public int TurnCount { get; private set; }

    public void InitializeFirstTurn()
    {
        CurrentTurn = TurnController.Turn.PlayerTurn;
        TurnCount = 1;
    }

    public void ChangeToEnemyTurn()
    {
        CurrentTurn = TurnController.Turn.EnermyTurn;
    }

    public void ChangeToPlayerTurn()
    {
        CurrentTurn = TurnController.Turn.PlayerTurn;
    }

    public void IncreaseTurnCount()
    {
        TurnCount++;
    }
}
