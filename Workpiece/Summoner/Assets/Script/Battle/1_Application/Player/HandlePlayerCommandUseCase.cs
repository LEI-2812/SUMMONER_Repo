public readonly struct PlayerCommandResult
{
    private PlayerCommandResult(
        bool didStart,
        PlayerActionResult actionResult,
        PlayerAttackResult attackResult)
    {
        DidStart = didStart;
        ActionResult = actionResult;
        AttackResult = attackResult;
    }

    public bool DidStart { get; }
    public PlayerActionResult ActionResult { get; }
    public PlayerAttackResult AttackResult { get; }

    public static PlayerCommandResult Blocked()
    {
        return new PlayerCommandResult(false, PlayerActionResult.Failed, default);
    }

    public static PlayerCommandResult Started(PlayerActionResult actionResult)
    {
        return new PlayerCommandResult(true, actionResult, default);
    }

    public static PlayerCommandResult Started(PlayerAttackResult attackResult)
    {
        return new PlayerCommandResult(true, attackResult.ActionResult, attackResult);
    }
}

// 역할: HandlePlayerCommandUseCase의 책임을 정의한다.
public class HandlePlayerCommandUseCase
{
    private readonly StartSummonSelectionUseCase startSummonSelectionUseCase;
    private readonly ExecutePlayerAttackUseCase attackUseCase;
    private readonly IPlayerTurnProgress turnProgress;
    private readonly PlayerActionStateMachine playerActionStateMachine;

    public HandlePlayerCommandUseCase(
        PlayerActionStateMachine playerActionStateMachine,
        IPlayerTurnProgress turnProgress,
        StartSummonSelectionUseCase startSummonSelectionUseCase,
        ExecutePlayerAttackUseCase attackUseCase)
    {
        this.turnProgress = turnProgress;
        this.startSummonSelectionUseCase = startSummonSelectionUseCase;
        this.attackUseCase = attackUseCase;
        this.playerActionStateMachine = playerActionStateMachine;
    }

    public void ResetPlayerSetting()
    {
        playerActionStateMachine.Reset();
    }

    public void StartPlayerTurn()
    {
        playerActionStateMachine.StartTurn();
    }

    public SummonSelectionStartResult ExecuteSummon()
    {
        return PrepareSummonSelection(
            playerActionStateMachine.TryStartSummon,
            startSummonSelectionUseCase.PrepareSummon);
    }

    public SummonSelectionStartResult ExecuteRedraw()
    {
        return PrepareSummonSelection(
            playerActionStateMachine.TryStartRedraw,
            startSummonSelectionUseCase.PrepareRedraw);
    }

    public void ConfirmSummonSelectionStarted(SummonSelectionStartResult startResult)
    {
        startSummonSelectionUseCase.ConfirmSelectionStarted(startResult);
    }

    public void CancelSummonSelectionStart()
    {
        playerActionStateMachine.CompleteAction();
    }

    public void CompleteSummonSelection()
    {
        startSummonSelectionUseCase.CompleteSelection();
    }

    public PlayerCommandResult ExecuteNormalAttack()
    {
        return RunPlayerAttack(
            playerActionStateMachine.TryStartNormalAttack,
            attackUseCase.ExecuteNormalAttack);
    }

    public PlayerCommandResult ExecuteSpecialAttack()
    {
        return ExecuteSpecialAttack(0);
    }

    public PlayerCommandResult ExecuteSpecialAttack(int specialAttackIndex)
    {
        return RunPlayerAttack(
            playerActionStateMachine.TryStartSpecialAttack,
            () => attackUseCase.ExecuteSpecialAttack(specialAttackIndex));
    }

    public PlayerCommandResult ExecuteEndTurn()
    {
        if (!playerActionStateMachine.TryStartEndTurn())
        {
            return PlayerCommandResult.Blocked();
        }

        if (!TryEndPlayerTurnProgress())
        {
            playerActionStateMachine.CompleteAction();
            return PlayerCommandResult.Started(PlayerActionResult.Failed);
        }

        return PlayerCommandResult.Started(PlayerActionResult.Completed);
    }

    public void AddMana()
    {
        playerActionStateMachine.AddMana(10);
    }

    public int GetMana()
    {
        return playerActionStateMachine.Mana;
    }

    public bool CanSummon()
    {
        return playerActionStateMachine.CanShowSummonAvailable();
    }

    public bool CanRedraw()
    {
        return playerActionStateMachine.CanRedraw();
    }

    public PlayerCommandResult CompleteSelectedPlayerAttack(int selectedTargetPlateIndex)
    {
        PlayerAttackResult attackResult = attackUseCase.ExecuteSelectedSpecialAttack(selectedTargetPlateIndex);
        playerActionStateMachine.CompleteAction();
        return PlayerCommandResult.Started(attackResult);
    }

    public void CancelSelectedPlayerAttack()
    {
        attackUseCase.CancelTargetSelection();
        playerActionStateMachine.CompleteAction();
    }

    public bool IsTargetSelectionActive()
    {
        return attackUseCase.IsTargetSelectionActive();
    }

    public int GetSelectedTargetPlateIndex()
    {
        return attackUseCase.GetSelectedTargetPlateIndex();
    }

    private PlayerCommandResult RunPlayerAttack(
        System.Func<bool> tryStartAction,
        System.Func<PlayerAttackResult> executeAttack)
    {
        if (!tryStartAction())
        {
            return PlayerCommandResult.Blocked();
        }

        PlayerAttackResult attackResult = executeAttack();
        CompletePlayerAction(attackResult.ActionResult);
        return PlayerCommandResult.Started(attackResult);
    }

    private SummonSelectionStartResult PrepareSummonSelection(
        System.Func<bool> tryStartAction,
        System.Func<SummonSelectionStartResult> prepareSelection)
    {
        if (!tryStartAction())
        {
            return SummonSelectionStartResult.Failed();
        }

        SummonSelectionStartResult startResult = prepareSelection();
        if (startResult.DidStart)
        {
            return startResult;
        }

        playerActionStateMachine.CompleteAction();
        return SummonSelectionStartResult.Failed();
    }

    private void CompletePlayerAction(PlayerActionResult actionResult)
    {
        if (actionResult == PlayerActionResult.WaitingForTarget)
        {
            playerActionStateMachine.WaitTargetSelection();
            return;
        }

        playerActionStateMachine.CompleteAction();
    }

    private bool TryEndPlayerTurnProgress()
    {
        if (!turnProgress.IsPlayerTurn())
        {
            return false;
        }

        turnProgress.EndPlayerTurn();
        return true;
    }
}
