using System.Collections.Generic;

public enum PlayerAttackSound
{
    None,
    Click,
    Fail
}

public enum PlayerAttackTargetBoard
{
    None,
    Player,
    Enemy
}

public readonly struct PlayerAttackResult
{
    public PlayerAttackResult(
        PlayerActionResult actionResult,
        PlayerAttackSound sound,
        PlayerAttackTargetBoard targetBoard,
        IReadOnlyList<string> messages,
        IReadOnlyList<string> warnings)
    {
        ActionResult = actionResult;
        Sound = sound;
        TargetBoard = targetBoard;
        Messages = messages;
        Warnings = warnings;
    }

    public PlayerActionResult ActionResult { get; }
    public PlayerAttackSound Sound { get; }
    public PlayerAttackTargetBoard TargetBoard { get; }
    public IReadOnlyList<string> Messages { get; }
    public IReadOnlyList<string> Warnings { get; }
}

// 역할: 플레이어 공격 규칙을 실행하고 Controller가 처리할 결과를 반환한다.
public class ExecutePlayerAttackUseCase
{
    private readonly AttackStateMachine attackStateMachine;
    private readonly BattleBoardData board;
    private readonly ExecuteSpecialAttackUseCase executeSpecialAttackUseCase;

    public ExecutePlayerAttackUseCase(
        AttackStateMachine attackStateMachine,
        BattleBoardData board)
    {
        this.attackStateMachine = attackStateMachine;
        this.board = board;
        executeSpecialAttackUseCase = new ExecuteSpecialAttackUseCase(board);
    }

    public PlayerAttackResult ExecuteNormalAttack()
    {
        var messages = new List<string>();
        var warnings = new List<string>();
        Summon attacker = PrepareNormalAttackSummon(messages);
        if (attacker == null)
        {
            return CreateResult(PlayerActionResult.Failed, PlayerAttackSound.Fail, messages, warnings);
        }

        ExecuteNormalAttack(
            attacker,
            attackStateMachine.GetAttackingPlateIndex(),
            messages,
            warnings);
        attackStateMachine.CompleteAttack();
        ResetAttackState();
        return CreateResult(PlayerActionResult.Completed, PlayerAttackSound.Click, messages, warnings);
    }

    public PlayerAttackResult ExecuteSpecialAttack(int specialAttackIndex)
    {
        var messages = new List<string>();
        Summon attacker = PrepareSpecialAttackSummon(specialAttackIndex, messages);
        if (attacker == null)
        {
            return CreateResult(PlayerActionResult.Failed, PlayerAttackSound.Fail, messages);
        }

        int attackIndex = attackStateMachine.GetCurrentSpecialAttackInfoIndex();
        AttackData attackStrategy = attacker.GetSpecialAttackStrategy()[attackIndex];
        if (IsSpecialAttackCooldown(attackStrategy))
        {
            messages.Add("Special attack is cooling down.");
            ResetAttackState();
            return CreateResult(PlayerActionResult.Failed, PlayerAttackSound.Fail, messages);
        }

        if (attackStrategy.IsStrategy<TargetedAttackStrategy>())
        {
            attackStateMachine.StartTargetSelection();
            attackStateMachine.ClearTargetSelection();
            PlayerAttackTargetBoard targetBoard = attackStateMachine.DoesCurrentSpecialAttackTargetPlayerPlate()
                ? PlayerAttackTargetBoard.Player
                : PlayerAttackTargetBoard.Enemy;
            return new PlayerAttackResult(
                PlayerActionResult.WaitingForTarget,
                PlayerAttackSound.Click,
                targetBoard,
                messages,
                new List<string>());
        }

        if (!ExecuteImmediateSpecialAttack(attacker, attackIndex))
        {
            return CreateResult(PlayerActionResult.Failed, PlayerAttackSound.None, messages);
        }

        return CreateResult(PlayerActionResult.Completed, PlayerAttackSound.Click, messages);
    }

    public PlayerAttackResult ExecuteSelectedSpecialAttack(int selectedTargetPlateIndex)
    {
        Summon attacker = attackStateMachine.GetAttackingSummon();
        int attackIndex = attackStateMachine.GetCurrentSpecialAttackInfoIndex();
        attackStateMachine.ClearTargetSelection();

        bool attackExecuted = executeSpecialAttackUseCase.Execute(
            attacker,
            selectedTargetPlateIndex,
            attackIndex,
            isPlayerAttacker: true);

        if (attackExecuted)
        {
            attackStateMachine.CompleteAttack();
            ResetAttackState();
            return CreateResult(PlayerActionResult.Completed, PlayerAttackSound.None);
        }

        ResetAttackState();
        return CreateResult(PlayerActionResult.Failed, PlayerAttackSound.None);
    }

    public void CancelTargetSelection()
    {
        attackStateMachine.CancelTargetSelection();
        ResetAttackState();
    }

    public bool IsTargetSelectionActive()
    {
        return attackStateMachine.IsSpecialAttackTargetSelectionActive();
    }

    public int GetSelectedTargetPlateIndex()
    {
        return attackStateMachine.GetSelectedSpecialAttackTargetPlateIndex();
    }

    private Summon PrepareNormalAttackSummon(List<string> messages)
    {
        Summon attacker = attackStateMachine.StartAttack(0);
        if (attacker == null)
        {
            ResetAttackState();
            return null;
        }

        if (!CanSelectedSummonAttack(attacker))
        {
            messages.Add("선택한 소환수는 공격할 수 없습니다.");
            ResetAttackState();
            return null;
        }

        return attacker;
    }

    private Summon PrepareSpecialAttackSummon(int attackIndex, List<string> messages)
    {
        Summon attacker = attackStateMachine.StartAttack(attackIndex);
        if (attacker == null)
        {
            ResetAttackState();
            return null;
        }

        if (!attackStateMachine.HasCurrentSpecialAttackInfo())
        {
            messages.Add("Selected summon has no special attack data.");
            ResetAttackState();
            return null;
        }

        if (!CanSelectedSummonAttack(attacker))
        {
            messages.Add("선택한 소환수는 공격할 수 없습니다.");
            ResetAttackState();
            return null;
        }

        return attacker;
    }

    private bool ExecuteImmediateSpecialAttack(Summon attacker, int attackIndex)
    {
        if (!executeSpecialAttackUseCase.Execute(
                attacker,
                attackStateMachine.GetAttackingPlateIndex(),
                attackIndex,
                isPlayerAttacker: true))
        {
            ResetAttackState();
            return false;
        }

        attackStateMachine.CompleteAttack();
        ResetAttackState();
        return true;
    }

    private void ResetAttackState()
    {
        attackStateMachine.Reset();
    }

    private bool CanSelectedSummonAttack(Summon attacker)
    {
        return attacker.GetIsAttack()
            && !attacker.IsStun();
    }

    private bool IsSpecialAttackCooldown(AttackData attackStrategy)
    {
        return attackStrategy.GetCurrentCooldown() > 0;
    }

    private void ExecuteNormalAttack(
        Summon attackSummon,
        int selectedPlateIndex,
        List<string> messages,
        List<string> warnings)
    {
        AttackData attackStrategy = attackSummon.GetAttackStrategy();
        if (attackStrategy == null)
        {
            messages.Add("일반 공격 데이터가 없습니다.");
            return;
        }

        if (!attackSummon.CanUseAttackStrategy(attackStrategy))
        {
            messages.Add("일반 공격이 쿨타임 중이거나 사용할 수 없습니다.");
            return;
        }

        List<Summon> targets = attackStrategy.SelectTargets(
            new AttackTargetInput(attackSummon, board, selectedPlateIndex, true));
        if (targets.Count == 0)
        {
            messages.Add("일반 공격 대상이 없습니다.");
        }

        foreach (Summon target in targets)
        {
            ApplyNormalAttackEffect(attackSummon, attackStrategy, target, messages, warnings);
        }

        attackSummon.PlayAttackMotion(true);
        attackSummon.ApplyAttackCooldown(attackStrategy);
        attackSummon.SetAttackAvailable(false);
    }

    private void ApplyNormalAttackEffect(
        Summon attacker,
        AttackData attackStrategy,
        Summon target,
        List<string> messages,
        List<string> warnings)
    {
        if (attackStrategy.GetStatusType() != StatusType.None)
        {
            warnings.Add($"일반 공격으로 적용할 수 없는 상태입니다: {attackStrategy.GetStatusType()}");
            return;
        }

        double damage = attacker.GetAttackPower();
        messages.Add($"{attacker.GetSummonName()}이 {target.GetSummonName()}을 공격했습니다.");
        target.TakeDamage(damage);
    }

    private static PlayerAttackResult CreateResult(
        PlayerActionResult actionResult,
        PlayerAttackSound sound,
        IReadOnlyList<string> messages = null,
        IReadOnlyList<string> warnings = null)
    {
        return new PlayerAttackResult(
            actionResult,
            sound,
            PlayerAttackTargetBoard.None,
            messages ?? new List<string>(),
            warnings ?? new List<string>());
    }
}
