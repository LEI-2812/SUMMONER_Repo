using System;

// 역할: PlayerTurnState의 책임을 정의한다.
class PlayerTurnState
{
    private readonly Action startPlayerTurn;
    private readonly Action addMana;
    private readonly Func<bool, bool> tryStopTurnForClearResult;
    private readonly IPlayerTurnBoard turnBoard;
    private readonly UpdateTurnSummonStateUseCase updateTurnSummonStateUseCase;
    private readonly AttackStateMachine attackStateMachine;

    public PlayerTurnState(
        Action startPlayerTurn,
        Action addMana,
        Func<bool, bool> tryStopTurnForClearResult,
        IPlayerTurnBoard turnBoard,
        UpdateTurnSummonStateUseCase updateTurnSummonStateUseCase,
        AttackStateMachine attackStateMachine)
    {
        this.startPlayerTurn = startPlayerTurn;
        this.addMana = addMana;
        this.tryStopTurnForClearResult = tryStopTurnForClearResult;
        this.turnBoard = turnBoard;
        this.updateTurnSummonStateUseCase = updateTurnSummonStateUseCase;
        this.attackStateMachine = attackStateMachine;
    }

    public void Enter()
    {
        updateTurnSummonStateUseCase.ApplyEnemyTurnStartEffects();
        turnBoard.CompactEnemyPlates();
        if (TryStopTurnForClearResult())
        {
            return;
        }

        updateTurnSummonStateUseCase.UpdatePlayerSpecialCooldowns();
        startPlayerTurn();
    }

    public void Exit()
    {
        attackStateMachine.Reset();
        turnBoard.ResetPlateHighlight();
        updateTurnSummonStateUseCase.UpdatePlayerUpgradeStatus();
    }

    public void EnterNewRound()
    {
        addMana();
        updateTurnSummonStateUseCase.UpdateEnemyUpgradeStatus();
        updateTurnSummonStateUseCase.ResetPlayerAttackReady();
    }

    private bool TryStopTurnForClearResult()
    {
        return tryStopTurnForClearResult(turnBoard.IsEnemyPlateClear());
    }
}
