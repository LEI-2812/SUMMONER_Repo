using System;

class EnemyTurnState
{
    private readonly Action startEnemyTurn;
    private readonly UpdateTurnSummonStateUseCase updateTurnSummonStateUseCase;

    public EnemyTurnState(
        Action startEnemyTurn,
        UpdateTurnSummonStateUseCase updateTurnSummonStateUseCase)
    {
        this.startEnemyTurn = startEnemyTurn;
        this.updateTurnSummonStateUseCase = updateTurnSummonStateUseCase;
    }

    public void Enter()
    {
        updateTurnSummonStateUseCase.ApplyPlayerTurnStartEffects();
        updateTurnSummonStateUseCase.UpdateEnemySpecialCooldowns();
        startEnemyTurn();
    }

    public void Exit()
    {
    }
}
