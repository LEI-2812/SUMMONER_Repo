// 역할: 플레이어 턴의 일반 공격과 특수 공격 시작 조건을 확인하고 실행 흐름을 조율한다.
public class PlayerAttackActions
{
    private readonly BattleController battleController;
    private readonly PlateController plateController;
    private readonly PlayerTurnProgressState turnProgressState;
    private readonly PlayerTurnActions turnActions;
    private readonly PlayerView playerView;
    private readonly PlayerFeedbackView feedbackView;
    private readonly PlayerActionExecutor actionExecutor;
    private readonly PlayerTargetSelectionActions targetSelectionActions;

    public PlayerAttackActions(
        PlayerController player,
        SummonController summonController,
        BattleController battleController,
        PlateController plateController,
        PlayerTurnProgressState turnProgressState,
        PlayerTurnActions turnActions,
        PlayerView playerView,
        PlayerFeedbackView feedbackView,
        PlayerActionExecutor actionExecutor)
    {
        this.battleController = battleController;
        this.plateController = plateController;
        this.turnProgressState = turnProgressState;
        this.turnActions = turnActions;
        this.playerView = playerView;
        this.feedbackView = feedbackView;
        this.actionExecutor = actionExecutor;
        targetSelectionActions = new PlayerTargetSelectionActions(
            player,
            summonController,
            battleController,
            plateController,
            feedbackView,
            ProcessAfterPlayerAttack);
    }

    public void TryExecuteNormalAttack()
    {
        Summon attackSummon = GetNormalAttackSummon();
        if (attackSummon == null)
        {
            return;
        }

        actionExecutor.ExecuteNormalAttack(
            attackSummon,
            plateController.GetEnermyPlates(),
            battleController.GetAttackingPlateIndex(),
            feedbackView);
        ProcessAfterPlayerAttack();
    }

    public void TryExecuteSpecialAttack()
    {
        Summon attackSummon = GetSpecialAttackSummon();
        if (attackSummon == null)
        {
            return;
        }

        if (!SpecialAttackExecuteOrTargetSelect(attackSummon))
        {
            return;
        }

        ProcessAfterPlayerAttack();
    }

    private Summon GetNormalAttackSummon()
    {
        Summon attackSummon = battleController.AttackStart(0);
        if (attackSummon == null)
        {
            feedbackView.PlayFail();
            return null;
        }

        if (!CanUseNormalAttack(attackSummon))
        {
            feedbackView.Log("공격할 수 없습니다. ");
            feedbackView.PlayFail();
            return null;
        }

        return attackSummon;
    }

    private bool CanUseNormalAttack(Summon attackSummon)
    {
        return attackSummon.GetIsAttack() && !attackSummon.IsStun();
    }

    private Summon GetSpecialAttackSummon()
    {
        Summon attackSummon = battleController.AttackStart(0);
        if (attackSummon == null)
        {
            feedbackView.PlayFail();
            return null;
        }

        if (!battleController.HasCurrentSpecialAttackInfo())
        {
            feedbackView.Log("사용 가능한 특수 공격이 없습니다.");
            feedbackView.PlayFail();
            return null;
        }

        if (!CanUseSpecialAttack(attackSummon))
        {
            feedbackView.Log("공격할 수 없습니다. ");
            feedbackView.PlayFail();
            return null;
        }

        return attackSummon;
    }

    private bool CanUseSpecialAttack(Summon attackSummon)
    {
        return attackSummon.GetIsAttack() && !attackSummon.IsStun();
    }

    private bool SpecialAttackExecuteOrTargetSelect(Summon attackSummon)
    {
        IAttackStrategy attackStrategy = attackSummon.GetSpecialAttackStrategy()[0];

        if (IsSpecialAttackCooldown(attackStrategy))
        {
            feedbackView.Log("특수 스킬이 쿨타임 중입니다. 사용할 수 없습니다.");
            feedbackView.PlayFail();
            return false;
        }

        if (attackStrategy is TargetedAttackStrategy)
        {
            TargetedSpecialAttackStart(attackSummon);
            return false;
        }

        return ImmediateSpecialAttackExecute(attackSummon);
    }

    private bool IsSpecialAttackCooldown(IAttackStrategy attackStrategy)
    {
        return attackStrategy.GetCurrentCooldown() > 0;
    }

    private void TargetedSpecialAttackStart(Summon attackSummon)
    {
        int specialAttackIndex = battleController.GetCurrentSpecialAttackInfoIndex();
        feedbackView.PlayClick();
        targetSelectionActions.StartTargetSelection(attackSummon, specialAttackIndex);
    }

    private bool ImmediateSpecialAttackExecute(Summon attackSummon)
    {
        return actionExecutor.ExecuteImmediateSpecialAttack(
            battleController,
            attackSummon,
            battleController.GetAttackingPlateIndex(),
            feedbackView);
    }

    private void ProcessAfterPlayerAttack()
    {
        turnProgressState.SetEnemyPlateClear(plateController.IsEnermyPlateClear());
        turnActions.CheckPlayerClearResult(turnProgressState);
        actionExecutor.ProcessAfterPlayerAttack(
            plateController,
            playerView);
    }
}
