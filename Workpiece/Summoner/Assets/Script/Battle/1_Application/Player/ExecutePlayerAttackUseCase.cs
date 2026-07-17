using System;
using System.Collections;
using System.Collections.Generic;

public interface IPlayerAttackOutput
{
    void PlayClick();
    void PlayFail();
    void Log(string message);
    void LogWarning(string message);
    void ShowTargetSelection(bool dimPlayerPlates);
    void HideTargetSelection();
    void ResetTargetSelection();
    bool IsOutsidePlayerPlatesClick();
    bool IsOutsideEnemyPlatesClick();
}

// 역할: ExecutePlayerAttackUseCase의 책임을 정의한다.
public class ExecutePlayerAttackUseCase
{
    private readonly ICoroutineRunner coroutineRunner;
    private readonly AttackStateMachine attackStateMachine;
    private readonly IReadOnlyList<BattleBoardInputController> playerPlates;
    private readonly IReadOnlyList<BattleBoardInputController> enemyPlates;
    private readonly IPlayerAttackOutput output;
    private readonly SpecialAttackExecution specialAttackExecution;

    public ExecutePlayerAttackUseCase(
        ICoroutineRunner coroutineRunner,
        AttackStateMachine attackStateMachine,
        IReadOnlyList<BattleBoardInputController> playerPlates,
        IReadOnlyList<BattleBoardInputController> enemyPlates,
        IPlayerAttackOutput output)
    {
        this.coroutineRunner = coroutineRunner;
        this.attackStateMachine = attackStateMachine;
        this.playerPlates = playerPlates;
        this.enemyPlates = enemyPlates;
        this.output = output;
        specialAttackExecution = new SpecialAttackExecution(playerPlates, enemyPlates);
    }

    public PlayerActionResult ExecuteNormalAttack()
    {
        Summon attacker = PrepareNormalAttackSummon();
        if (attacker == null)
        {
            return PlayerActionResult.Failed;
        }

        ExecuteNormalAttack(
            attacker,
            enemyPlates,
            attackStateMachine.GetAttackingPlateIndex());
        attackStateMachine.CompleteAttack();
        ResetAttackState();
        output.PlayClick();
        return PlayerActionResult.Completed;
    }

    public PlayerActionResult ExecuteSpecialAttack(
        int specialAttackIndex,
        Action onWaitTargetSelection,
        Action onAttackCompleted,
        Action onTargetSelectionCanceled)
    {
        Summon attacker = PrepareSpecialAttackSummon(specialAttackIndex);
        if (attacker == null)
        {
            return PlayerActionResult.Failed;
        }

        int attackIndex = attackStateMachine.GetCurrentSpecialAttackInfoIndex();
        AttackData attackStrategy = attacker.GetSpecialAttackStrategy()[attackIndex];
        if (IsSpecialAttackCooldown(attackStrategy))
        {
            output.Log("Special attack is cooling down.");
            ResetAttackState();
            output.PlayFail();
            return PlayerActionResult.Failed;
        }

        if (attackStrategy.IsStrategy<TargetedAttackStrategy>())
        {
            StartTargetedSpecialAttack(
                attacker,
                attackIndex,
                onWaitTargetSelection,
                onAttackCompleted,
                onTargetSelectionCanceled);
            return PlayerActionResult.WaitingForTarget;
        }

        if (!ExecuteImmediateSpecialAttack(attacker, attackIndex))
        {
            return PlayerActionResult.Failed;
        }

        return PlayerActionResult.Completed;
    }

    private Summon PrepareNormalAttackSummon()
    {
        Summon attacker = attackStateMachine.StartAttack(0);
        if (attacker == null)
        {
            ResetAttackState();
            output.PlayFail();
            return null;
        }

        if (!CanSelectedSummonAttack(attacker))
        {
            output.Log("선택한 소환수는 공격할 수 없습니다.");
            ResetAttackState();
            output.PlayFail();
            return null;
        }

        return attacker;
    }

    private Summon PrepareSpecialAttackSummon(int attackIndex)
    {
        Summon attacker = attackStateMachine.StartAttack(attackIndex);
        if (attacker == null)
        {
            ResetAttackState();
            output.PlayFail();
            return null;
        }

        if (!attackStateMachine.HasCurrentSpecialAttackInfo())
        {
            output.Log("Selected summon has no special attack data.");
            ResetAttackState();
            output.PlayFail();
            return null;
        }

        if (!CanSelectedSummonAttack(attacker))
        {
            output.Log("선택한 소환수는 공격할 수 없습니다.");
            ResetAttackState();
            output.PlayFail();
            return null;
        }

        return attacker;
    }

    private bool ExecuteImmediateSpecialAttack(Summon attacker, int attackIndex)
    {
        if (!specialAttackExecution.Execute(
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
        output.PlayClick();
        return true;
    }

    private void StartTargetedSpecialAttack(
        Summon attacker,
        int attackIndex,
        Action onWaitTargetSelection,
        Action onCompleted,
        Action onCanceled)
    {
        output.PlayClick();
        onWaitTargetSelection?.Invoke();
        StartTargetSelection(
            selectedTargetPlateIndex => ExecuteSelectedSpecialAttack(
                attacker,
                attackIndex,
                selectedTargetPlateIndex,
                onCompleted,
                onCanceled),
            () => CancelTargetSelection(onCanceled));
    }

    private void StartTargetSelection(Action<int> onTargetSelected, Action onTargetSelectionCanceled)
    {
        if (attackStateMachine.DoesCurrentSpecialAttackTargetPlayerPlate())
        {
            StartPlayerPlateSelection(onTargetSelected, onTargetSelectionCanceled);
            return;
        }

        StartEnemyPlateSelection(onTargetSelected, onTargetSelectionCanceled);
    }

    private void StartPlayerPlateSelection(Action<int> onTargetSelected, Action onTargetSelectionCanceled)
    {
        output.Log("플레이어 플레이트를 선택하세요.");
        coroutineRunner.StartCoroutine(WaitForTargetPlateSelection(
            true,
            false,
            "플레이어 플레이트 선택을 기다립니다.",
            "대상 선택이 취소되었습니다.",
            "특수 공격 대상으로 플레이어 플레이트 {0}번을 선택했습니다.",
            onTargetSelected,
            onTargetSelectionCanceled));
    }

    private void StartEnemyPlateSelection(Action<int> onTargetSelected, Action onTargetSelectionCanceled)
    {
        output.Log("적 플레이트를 선택하세요.");
        coroutineRunner.StartCoroutine(WaitForTargetPlateSelection(
            false,
            true,
            "적 플레이트 선택을 기다립니다.",
            "대상 선택이 취소되었습니다.",
            "특수 공격 대상으로 적 플레이트 {0}번을 선택했습니다.",
            onTargetSelected,
            onTargetSelectionCanceled));
    }

    private IEnumerator WaitForTargetPlateSelection(
        bool targetsPlayerPlates,
        bool downTransparencyForPlayerPlate,
        string waitLog,
        string outsideClickLog,
        string executeLogFormat,
        Action<int> onTargetSelected,
        Action onTargetSelectionCanceled)
    {
        attackStateMachine.StartTargetSelection();
        output.ShowTargetSelection(downTransparencyForPlayerPlate);
        attackStateMachine.ClearTargetSelection();
        output.Log(waitLog);

        while (attackStateMachine.GetSelectedSpecialAttackTargetPlateIndex() < 0)
        {
            if (attackStateMachine.IsSpecialAttackTargetSelectionActive()
                && IsOutsideTargetPlatesClick(targetsPlayerPlates))
            {
                output.Log(outsideClickLog);
                output.HideTargetSelection();
                attackStateMachine.CancelTargetSelection();
                onTargetSelectionCanceled?.Invoke();
                yield break;
            }

            yield return null;
        }

        int selectedTargetPlateIndex = attackStateMachine.GetSelectedSpecialAttackTargetPlateIndex();
        output.Log(string.Format(executeLogFormat, selectedTargetPlateIndex));
        output.HideTargetSelection();
        attackStateMachine.ClearTargetSelection();
        onTargetSelected?.Invoke(selectedTargetPlateIndex);
    }

    private bool IsOutsideTargetPlatesClick(bool targetsPlayerPlates)
    {
        return targetsPlayerPlates
            ? output.IsOutsidePlayerPlatesClick()
            : output.IsOutsideEnemyPlatesClick();
    }

    private void ExecuteSelectedSpecialAttack(
        Summon attacker,
        int attackIndex,
        int selectedTargetPlateIndex,
        Action onCompleted,
        Action onFailed)
    {
        bool attackExecuted = specialAttackExecution.Execute(
            attacker,
            selectedTargetPlateIndex,
            attackIndex,
            isPlayerAttacker: true);

        if (attackExecuted)
        {
            attackStateMachine.CompleteAttack();
            ResetAttackState();
            onCompleted?.Invoke();
            return;
        }

        ResetAttackState();
        onFailed?.Invoke();
    }

    private void CancelTargetSelection(Action onCanceled)
    {
        ResetAttackState();
        onCanceled?.Invoke();
    }

    private void ResetAttackState()
    {
        attackStateMachine.Reset();
        output.ResetTargetSelection();
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
        IReadOnlyList<BattleBoardInputController> targetPlates,
        int selectedPlateIndex)
    {
        AttackData attackStrategy = attackSummon.GetAttackStrategy();
        if (attackStrategy == null)
        {
            output.Log("일반 공격 데이터가 없습니다.");
            return;
        }

        if (!attackSummon.CanUseAttackStrategy(attackStrategy))
        {
            output.Log("일반 공격이 쿨타임 중이거나 사용할 수 없습니다.");
            return;
        }

        List<Summon> targets = attackStrategy.SelectTargets(attackSummon, targetPlates, selectedPlateIndex);
        if (targets.Count == 0)
        {
            output.Log("일반 공격 대상이 없습니다.");
        }

        foreach (Summon target in targets)
        {
            ApplyNormalAttackEffect(attackSummon, attackStrategy, target);
        }

        attackSummon.PlayAttackMotion(true);
        attackSummon.ApplyAttackCooldown(attackStrategy);
        attackSummon.SetAttackAvailable(false);
    }

    private void ApplyNormalAttackEffect(Summon attacker, AttackData attackStrategy, Summon target)
    {
        if (attackStrategy.GetStatusType() != StatusType.None)
        {
            output.LogWarning($"일반 공격으로 적용할 수 없는 상태입니다: {attackStrategy.GetStatusType()}");
            return;
        }

        double damage = attacker.GetAttackPower();
        output.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}을 공격했습니다.");
        target.TakeDamage(damage);
    }

}
