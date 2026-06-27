using System;
using UnityEngine;

// 역할: PlayerTurnState의 책임을 정의한다.
internal sealed class PlayerTurnState
{
    private readonly HandlePlayerCommandUseCase handlePlayerCommandUseCase;
    private readonly IPlayerTurnBoard turnBoard;
    private readonly Func<int> getTurnCount;
    private readonly TurnSummonStateUpdater turnSummonStateUpdater;
    private readonly AttackStateMachine attackStateMachine;

    public PlayerTurnState(
        HandlePlayerCommandUseCase handlePlayerCommandUseCase,
        IPlayerTurnBoard turnBoard,
        Func<int> getTurnCount,
        TurnSummonStateUpdater turnSummonStateUpdater,
        AttackStateMachine attackStateMachine)
    {
        this.handlePlayerCommandUseCase = handlePlayerCommandUseCase;
        this.turnBoard = turnBoard;
        this.getTurnCount = getTurnCount;
        this.turnSummonStateUpdater = turnSummonStateUpdater;
        this.attackStateMachine = attackStateMachine;
    }

    public void Enter()
    {
        turnSummonStateUpdater.ApplyEnemyTurnStartEffects();
        turnBoard.CompactEnemyPlates();
        if (TryStopTurnForClearResult())
        {
            return;
        }

        turnSummonStateUpdater.UpdatePlayerSpecialCooldowns();
        handlePlayerCommandUseCase.StartPlayerTurn();
    }

    public void Exit()
    {
        attackStateMachine.Reset();
        turnBoard.ResetPlateHighlight();
        turnSummonStateUpdater.UpdatePlayerUpgradeStatus();
    }

    public void EnterNewRound()
    {
        handlePlayerCommandUseCase.AddMana();
        turnSummonStateUpdater.UpdateEnemyUpgradeStatus();
        turnSummonStateUpdater.ResetPlayerAttackReady();
        Debug.Log("현재 턴: " + getTurnCount());
    }

    private bool TryStopTurnForClearResult()
    {
        return handlePlayerCommandUseCase.TryStopTurnForClearResult(turnBoard.IsEnemyPlateClear());
    }
}
