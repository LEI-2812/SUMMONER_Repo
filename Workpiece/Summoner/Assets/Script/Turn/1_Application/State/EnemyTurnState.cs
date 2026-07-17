using System;

class EnemyTurnState
{
    private readonly Action startEnemyTurn;
    private readonly TurnSummonStateUpdater turnSummonStateUpdater;

    public EnemyTurnState(
        Action startEnemyTurn,
        TurnSummonStateUpdater turnSummonStateUpdater)
    {
        this.startEnemyTurn = startEnemyTurn;
        this.turnSummonStateUpdater = turnSummonStateUpdater;
    }

    public void Enter()
    {
        turnSummonStateUpdater.ApplyPlayerTurnStartEffects();
        turnSummonStateUpdater.UpdateEnemySpecialCooldowns();
        startEnemyTurn();
    }

    public void Exit()
    {
    }
}
