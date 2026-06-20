using UnityEngine;

// 역할: 현재 턴에 맞는 턴 시작/종료 실행 흐름만 담당한다.
internal sealed class TurnPhaseActions
{
    private readonly PlayerController player;
    private readonly Enermy enermy;
    private readonly PlateController plateController;
    private readonly BattleResultController battleResultController;
    private readonly int clearTurn;
    private readonly TurnProgressState turnProgressState;
    private readonly TurnSummonStateUpdater turnSummonStateUpdater;
    private readonly TurnView turnView;

    public TurnPhaseActions(
        PlayerController player,
        Enermy enermy,
        PlateController plateController,
        BattleResultController battleResultController,
        int clearTurn,
        TurnProgressState turnProgressState,
        TurnSummonStateUpdater turnSummonStateUpdater,
        TurnView turnView)
    {
        this.player = player;
        this.enermy = enermy;
        this.plateController = plateController;
        this.battleResultController = battleResultController;
        this.clearTurn = clearTurn;
        this.turnProgressState = turnProgressState;
        this.turnSummonStateUpdater = turnSummonStateUpdater;
        this.turnView = turnView;
    }

    public void StartCurrentTurn()
    {
        if (turnProgressState.CurrentTurn == TurnController.Turn.PlayerTurn)
        {
            StartPlayerTurn();
            return;
        }

        if (turnProgressState.CurrentTurn == TurnController.Turn.EnermyTurn)
        {
            StartEnemyTurn();
        }
    }

    public void EndCurrentTurn()
    {
        if (turnProgressState.CurrentTurn == TurnController.Turn.PlayerTurn)
        {
            EndPlayerTurn();
            return;
        }

        if (turnProgressState.CurrentTurn == TurnController.Turn.EnermyTurn)
        {
            EndEnemyTurn();
        }
    }

    private void StartPlayerTurn()
    {
        turnSummonStateUpdater.ApplyEnemyTurnStartEffects();
        plateController.CompactEnermyPlates();
        if (TryClearBattle())
        {
            return;
        }

        turnSummonStateUpdater.UpdatePlayerSpecialCooldowns();
        player.PlayerTurnStart();
    }

    private void StartEnemyTurn()
    {
        turnSummonStateUpdater.ApplyPlayerTurnStartEffects();
        turnSummonStateUpdater.UpdateEnemySpecialCooldowns();
        enermy.EnermyTurnStart();
    }

    private void EndPlayerTurn()
    {
        turnProgressState.ChangeToEnemyTurn();
        turnSummonStateUpdater.UpdatePlayerUpgradeStatus();
        StartCurrentTurn();
    }

    private void EndEnemyTurn()
    {
        turnProgressState.ChangeToPlayerTurn();
        turnProgressState.IncreaseTurnCount();
        turnView.ShowTurnCount(turnProgressState.TurnCount);
        player.AddMana();
        turnSummonStateUpdater.UpdateEnemyUpgradeStatus();
        turnSummonStateUpdater.ResetPlayerAttackReady();
        Debug.Log("현재 턴: " + turnProgressState.TurnCount);
        StartCurrentTurn();
    }

    private bool TryClearBattle()
    {
        return battleResultController.ClearResultTry(
            plateController.IsEnermyPlateClear(),
            clearTurn,
            turnProgressState.TurnCount);
    }
}
