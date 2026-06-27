// 역할: HandlePlayerCommandUseCase의 책임을 정의한다.
public sealed class HandlePlayerCommandUseCase
{
    private readonly string playerName;
    private readonly StartSummonSelectionUseCase startSummonSelectionUseCase;
    private readonly ExecutePlayerAttackUseCase attackUseCase;
    private readonly IPlayerTurnProgress turnProgress;
    private readonly BattleResultController battleResultController;
    private readonly PlateBoardView plateBoardController;
    private readonly PlayerHudView hudView;
    private readonly ManaView manaView;
    private readonly PlayerFeedbackView feedbackView;
    private readonly PlayerActionStateMachine playerActionStateMachine;

    public HandlePlayerCommandUseCase(
        string playerName,
        PlayerActionStateMachine playerActionStateMachine,
        IPlayerTurnProgress turnProgress,
        BattleResultController battleResultController,
        PlateBoardView plateBoardController,
        StartSummonSelectionUseCase startSummonSelectionUseCase,
        ExecutePlayerAttackUseCase attackUseCase,
        PlayerHudView hudView,
        ManaView manaView,
        PlayerFeedbackView feedbackView)
    {
        this.playerName = playerName;
        this.turnProgress = turnProgress;
        this.battleResultController = battleResultController;
        this.plateBoardController = plateBoardController;
        this.startSummonSelectionUseCase = startSummonSelectionUseCase;
        this.attackUseCase = attackUseCase;
        this.hudView = hudView;
        this.manaView = manaView;
        this.feedbackView = feedbackView;
        this.playerActionStateMachine = playerActionStateMachine;
    }

    public void ResetPlayerSetting()
    {
        playerActionStateMachine.Reset();
        UpdateManaUI();
    }

    public void StartPlayerTurn()
    {
        feedbackView.Log("플레이어 턴을 시작했습니다.");
        feedbackView.Log($"{playerName} current mana: {playerActionStateMachine.Mana}");
        playerActionStateMachine.StartTurn();
        UpdateManaUI();
        TryStartFailResult();
    }

    public void ExecuteSummon()
    {
        RunPlayerTurnAction(
            playerActionStateMachine.TryStartSummon,
            startSummonSelectionUseCase.ExecuteSummon);
    }

    public void ExecuteRedraw()
    {
        RunPlayerTurnAction(
            playerActionStateMachine.TryStartRedraw,
            startSummonSelectionUseCase.ExecuteRedraw);
    }

    public void ExecuteNormalAttack()
    {
        RunPlayerTurnAction(
            playerActionStateMachine.TryStartNormalAttack,
            attackUseCase.ExecuteNormalAttack,
            ProcessAfterPlayerAttack);
    }

    public void ExecuteSpecialAttack()
    {
        ExecuteSpecialAttack(0);
    }

    public void ExecuteSpecialAttack(int specialAttackIndex)
    {
        RunPlayerTurnAction(
            playerActionStateMachine.TryStartSpecialAttack,
            () => attackUseCase.ExecuteSpecialAttack(
                specialAttackIndex,
                playerActionStateMachine.WaitTargetSelection,
                CompleteSelectedPlayerAttack,
                playerActionStateMachine.CompleteAction),
            ProcessAfterPlayerAttack);
    }

    public void ExecuteEndTurn()
    {
        if (!playerActionStateMachine.TryStartEndTurn())
        {
            feedbackView.PlayFail();
            return;
        }

        if (!TryEndPlayerTurnProgress())
        {
            playerActionStateMachine.CompleteAction();
            feedbackView.PlayFail();
            feedbackView.Log("플레이어 턴을 종료할 수 없습니다.");
            return;
        }

        feedbackView.Log("플레이어 턴을 종료했습니다.");
        feedbackView.PlayClick();
    }

    public void AddMana()
    {
        playerActionStateMachine.AddMana(10);
        UpdateManaUI();
    }

    public bool TryStopTurnForClearResult(bool isEnemyPlateClear)
    {
        return CheckPlayerClearResult(isEnemyPlateClear);
    }

    private void UpdateManaUI()
    {
        manaView.ShowMana(playerActionStateMachine.Mana);
        hudView.ShowSummonAvailable(playerActionStateMachine.CanShowSummonAvailable());
        hudView.ShowRedrawAvailable(playerActionStateMachine.CanRedraw());
    }

    private void TryStartFailResult()
    {
        battleResultController.TryStartFailResult(
            turnProgress.GetClearTurn(),
            turnProgress.GetTurnCount());
    }

    private void RunPlayerTurnAction(
        System.Func<bool> tryStartAction,
        System.Func<PlayerActionResult> executeAction,
        System.Action onCompleted = null)
    {
        if (!tryStartAction())
        {
            feedbackView.PlayFail();
            return;
        }

        CompletePlayerAction(executeAction(), onCompleted);
    }

    private void CompletePlayerAction(PlayerActionResult actionResult, System.Action onCompleted = null)
    {
        if (actionResult == PlayerActionResult.WaitingForTarget)
        {
            UpdateManaUI();
            return;
        }

        if (actionResult == PlayerActionResult.Completed)
        {
            onCompleted?.Invoke();
            UpdateManaUI();
        }

        playerActionStateMachine.CompleteAction();
    }

    private void CompleteSelectedPlayerAttack()
    {
        ProcessAfterPlayerAttack();
        playerActionStateMachine.CompleteAction();
    }

    private void ProcessAfterPlayerAttack()
    {
        CheckPlayerClearResult(plateBoardController.IsEnemyPlateClear());
        plateBoardController.CompactEnemyPlates();
        hudView.HideStatePanel();
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

    private bool CheckPlayerClearResult(bool isEnemyPlateClear)
    {
        return battleResultController.TryStartClearResult(
            isEnemyPlateClear,
            turnProgress.GetClearTurn(),
            turnProgress.GetTurnCount());
    }
}
