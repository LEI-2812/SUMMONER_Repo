// 역할: 전투 턴의 현재 상태와 턴 수 전이를 관리한다.
enum TurnPhase
{
    PlayerTurn,
    EnemyTurn
}

class TurnStateMachine
{
    public TurnPhase CurrentTurn { get; private set; }
    public int TurnCount { get; private set; }

    public void StartPlayerTurn()
    {
        CurrentTurn = TurnPhase.PlayerTurn;
        TurnCount = 1;
    }

    public void ChangeToEnemyTurn()
    {
        CurrentTurn = TurnPhase.EnemyTurn;
    }

    public void ChangeToPlayerTurn()
    {
        CurrentTurn = TurnPhase.PlayerTurn;
        TurnCount++;
    }

    public bool IsPlayerTurn()
    {
        return CurrentTurn == TurnPhase.PlayerTurn;
    }

    public bool IsEnemyTurn()
    {
        return CurrentTurn == TurnPhase.EnemyTurn;
    }
}
