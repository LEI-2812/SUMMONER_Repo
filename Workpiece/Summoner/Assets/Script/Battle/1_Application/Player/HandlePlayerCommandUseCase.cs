// 역할: HandlePlayerCommandUseCase의 책임을 정의한다.
public interface IPlayerCommandOutput
{
    void ShowPlayerActionState(int mana, bool canSummon, bool canRedraw);
    void PlayClick();
    void PlayFail();
    void Log(string message);
    void StartFailResult(int clearTurn, int currentTurn);
    bool TryStartClearResult(bool isEnemyBoardClear, int clearTurn, int currentTurn);
    bool IsEnemyBoardClear();
    void CompactEnemyBoard();
    void HideSummonStatePanel();
}

public class HandlePlayerCommandUseCase
{
    private readonly string playerName;
    private readonly StartSummonSelectionUseCase startSummonSelectionUseCase;
    private readonly ExecutePlayerAttackUseCase attackUseCase;
    private readonly IPlayerTurnProgress turnProgress;
    private readonly IPlayerCommandOutput output;
    private readonly PlayerActionStateMachine playerActionStateMachine;

    public HandlePlayerCommandUseCase(
        string playerName,
        PlayerActionStateMachine playerActionStateMachine,
        IPlayerTurnProgress turnProgress,
        StartSummonSelectionUseCase startSummonSelectionUseCase,
        ExecutePlayerAttackUseCase attackUseCase,
        IPlayerCommandOutput output)
    {
        this.playerName = playerName;
        this.turnProgress = turnProgress;
        this.startSummonSelectionUseCase = startSummonSelectionUseCase;
        this.attackUseCase = attackUseCase;
        this.output = output;
        this.playerActionStateMachine = playerActionStateMachine;
    }

    public void ResetPlayerSetting()
    {
        playerActionStateMachine.Reset();
        UpdateManaUI();
    }

    public void StartPlayerTurn()
    {
        output.Log("플레이어 턴을 시작했습니다.");
        output.Log($"{playerName} current mana: {playerActionStateMachine.Mana}");
        playerActionStateMachine.StartTurn();
        UpdateManaUI();
        TryStartFailResult();
    }

    public SummonSelectionStartResult ExecuteSummon()
    {
        SummonSelectionStartResult startResult = PrepareSummonSelection(
            playerActionStateMachine.TryStartSummon,
            startSummonSelectionUseCase.PrepareSummon);
        if (!startResult.DidStart)
        {
            output.Log("모든 플레이트에 소환수가 있거나 지금은 소환할 수 없습니다.");
        }

        return startResult;
    }

    public SummonSelectionStartResult ExecuteRedraw()
    {
        SummonSelectionStartResult startResult = PrepareSummonSelection(
            playerActionStateMachine.TryStartRedraw,
            startSummonSelectionUseCase.PrepareRedraw);
        if (!startResult.DidStart)
        {
            output.Log("다시 뽑기할 소환수가 없거나 지금은 다시 뽑을 수 없습니다.");
        }

        return startResult;
    }

    public void ConfirmSummonSelectionStarted(SummonSelectionStartResult startResult)
    {
        startSummonSelectionUseCase.ConfirmSelectionStarted(startResult);
        if (!startResult.IsRedraw)
        {
            output.Log(startResult.PlateIndex + "번째 플레이트에서 소환을 시작합니다.");
        }

        output.PlayClick();
        UpdateManaUI();
    }

    public void CancelSummonSelectionStart()
    {
        playerActionStateMachine.CompleteAction();
        output.PlayFail();
    }

    public void CompleteSummonSelection()
    {
        startSummonSelectionUseCase.CompleteSelection();
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
            output.PlayFail();
            return;
        }

        if (!TryEndPlayerTurnProgress())
        {
            playerActionStateMachine.CompleteAction();
            output.PlayFail();
            output.Log("플레이어 턴을 종료할 수 없습니다.");
            return;
        }

        output.Log("플레이어 턴을 종료했습니다.");
        output.PlayClick();
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
        output.ShowPlayerActionState(
            playerActionStateMachine.Mana,
            playerActionStateMachine.CanShowSummonAvailable(),
            playerActionStateMachine.CanRedraw());
    }

    private void TryStartFailResult()
    {
        output.StartFailResult(
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
            output.PlayFail();
            return;
        }

        CompletePlayerAction(executeAction(), onCompleted);
    }

    private SummonSelectionStartResult PrepareSummonSelection(
        System.Func<bool> tryStartAction,
        System.Func<SummonSelectionStartResult> prepareSelection)
    {
        if (!tryStartAction())
        {
            output.PlayFail();
            return SummonSelectionStartResult.Failed();
        }

        SummonSelectionStartResult startResult = prepareSelection();
        if (startResult.DidStart)
        {
            return startResult;
        }

        playerActionStateMachine.CompleteAction();
        output.PlayFail();
        return SummonSelectionStartResult.Failed();
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
        CheckPlayerClearResult(output.IsEnemyBoardClear());
        output.CompactEnemyBoard();
        output.HideSummonStatePanel();
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
        return output.TryStartClearResult(
            isEnemyPlateClear,
            turnProgress.GetClearTurn(),
            turnProgress.GetTurnCount());
    }
}
