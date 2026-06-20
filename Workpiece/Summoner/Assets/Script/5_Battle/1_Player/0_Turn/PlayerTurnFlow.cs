// 역할: 플레이어 턴에서 소환, 재소환, 공격, 턴 종료 흐름을 조율한다.
public class PlayerTurnFlow
{
    private readonly PlayerController player;
    private readonly SummonController summonController;
    private readonly BattleController battleController;
    private readonly PlayerTurnActions turnActions;
    private readonly PlayerSummonActions summonActions;
    private readonly PlayerAttackActions attackActions;
    private readonly PlayerView playerView;
    private readonly PlayerFeedbackView feedbackView;
    private readonly PlayerTurnResourceState resourceState;
    private readonly PlayerTurnProgressState turnProgressState;

    public PlayerTurnFlow(
        PlayerController player,
        SummonController summonController,
        BattleController battleController,
        PlayerTurnResourceState resourceState,
        PlayerTurnProgressState turnProgressState,
        PlayerTurnActions turnActions,
        PlayerSummonActions summonActions,
        PlayerAttackActions attackActions,
        PlayerView playerView,
        PlayerFeedbackView feedbackView)
    {
        this.player = player;
        this.summonController = summonController;
        this.battleController = battleController;
        this.resourceState = resourceState;
        this.turnProgressState = turnProgressState;
        this.turnActions = turnActions;
        this.summonActions = summonActions;
        this.attackActions = attackActions;
        this.playerView = playerView;
        this.feedbackView = feedbackView;
    }

    public int GetClearTurn()
    {
        int clearTurn = turnActions.GetPlayerClearTurn();
        turnProgressState.SetTurnProgress(clearTurn, player.currentTurn);
        return clearTurn;
    }

    public void ResetPlayerSetting()
    {
        resourceState.Reset();
        UpdateManaUI();
    }

    public void UpdateRedrawButtonState()
    {
        if (!resourceState.CanRedraw())
        {
            playerView.ShowRedrawButtonDisabled();
            return;
        }

        playerView.ShowRedrawButtonEnabled();
    }

    public void StartPlayerTurn()
    {
        feedbackView.Log("플레이어 턴 시작");
        feedbackView.Log($"{player.gameObject.name} 의 마나: {resourceState.Mana}");
        turnActions.StartPlayerTurn(turnProgressState);
        player.clearTurn = turnProgressState.ClearTurn;
        player.currentTurn = turnProgressState.CurrentTurn;
        resourceState.StartTurn();
        UpdateManaUI();
        BattleFailResultTry();
    }

    public void TryStartSummon()
    {
        if (PlayerActionBlockedCheck()) return;

        summonActions.TryStartSummon();
    }

    public void TryStartRedraw()
    {
        if (PlayerActionBlockedCheck()) return;

        summonActions.TryStartRedraw();
    }

    public void TryExecuteNormalAttack()
    {
        if (PlayerActionBlockedCheck()) return;

        attackActions.TryExecuteNormalAttack();
    }

    public void TryExecuteSpecialAttack()
    {
        if (PlayerActionBlockedCheck()) return;

        attackActions.TryExecuteSpecialAttack();
    }

    public void TryEndPlayerTurn()
    {
        if (PlayerActionBlockedCheck()) return;

        if (!turnActions.TryEndPlayerTurn())
        {
            feedbackView.PlayFail();
            feedbackView.Log("플레이어 턴이 아닙니다.");
            return;
        }

        feedbackView.Log("플레이어 턴 종료");
        feedbackView.PlayClick();
    }

    public void SetHasSummonedThisTurn(bool value)
    {
        resourceState.SetHasSummonedThisTurn(value);
    }

    public bool HasSummonedThisTurn()
    {
        return resourceState.HasSummonedThisTurn;
    }

    public void UpdateManaUI()
    {
        playerView.UpdateMana(resourceState.Mana, resourceState.CanShowSummonAvailable());
    }

    public void AddMana()
    {
        resourceState.AddMana(10);
        UpdateManaUI();
    }

    public void BattleFailResultTry()
    {
        turnActions.CheckPlayerFailResult(turnProgressState);
    }

    private bool PlayerActionBlockedCheck()
    {
        return summonController.IsSummoning() || battleController.IsSpecialAttackTargetSelectionActive();
    }
}
