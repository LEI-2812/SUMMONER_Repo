// 역할: EnemyTurnState의 책임을 정의한다.
internal sealed class EnemyTurnState
{
    private readonly EnemyTurnRuntime enemyTurnController;
    private readonly TurnSummonStateUpdater turnSummonStateUpdater;

    public EnemyTurnState(
        EnemyTurnRuntime enemyTurnController,
        TurnSummonStateUpdater turnSummonStateUpdater)
    {
        this.enemyTurnController = enemyTurnController;
        this.turnSummonStateUpdater = turnSummonStateUpdater;
    }

    public void Enter()
    {
        turnSummonStateUpdater.ApplyPlayerTurnStartEffects();
        turnSummonStateUpdater.UpdateEnemySpecialCooldowns();
        enemyTurnController.StartEnemyTurn();
    }

    public void Exit()
    {
    }
}
