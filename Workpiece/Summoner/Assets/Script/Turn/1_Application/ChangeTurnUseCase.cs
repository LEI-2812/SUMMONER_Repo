using System;

internal sealed class ChangeTurnUseCase
{
    private readonly TurnStateMachine turnStateMachine;
    private readonly Func<PlayerTurnState> createPlayerTurnState;
    private readonly Func<EnemyTurnState> createEnemyTurnState;

    public ChangeTurnUseCase(
        TurnStateMachine turnStateMachine,
        Func<PlayerTurnState> createPlayerTurnState,
        Func<EnemyTurnState> createEnemyTurnState)
    {
        this.turnStateMachine = turnStateMachine;
        this.createPlayerTurnState = createPlayerTurnState;
        this.createEnemyTurnState = createEnemyTurnState;
    }

    public void StartPlayerTurn()
    {
        turnStateMachine.StartPlayerTurn();
        createPlayerTurnState().Enter();
    }

    public bool TryEndPlayerTurn()
    {
        if (!turnStateMachine.IsPlayerTurn())
        {
            return false;
        }

        createPlayerTurnState().Exit();
        turnStateMachine.ChangeToEnemyTurn();
        createEnemyTurnState().Enter();
        return true;
    }

    public bool TryCompleteEnemyTurn()
    {
        if (!turnStateMachine.IsEnemyTurn())
        {
            return false;
        }

        createEnemyTurnState().Exit();
        turnStateMachine.ChangeToPlayerTurn();
        createPlayerTurnState().EnterNewRound();
        createPlayerTurnState().Enter();
        return true;
    }
}
