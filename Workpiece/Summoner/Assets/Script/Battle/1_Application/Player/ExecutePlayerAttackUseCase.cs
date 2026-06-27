using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: ExecutePlayerAttackUseCase의 책임을 정의한다.
public class ExecutePlayerAttackUseCase
{
    private readonly ICoroutineRunner coroutineRunner;
    private readonly AttackStateMachine attackStateMachine;
    private readonly IReadOnlyList<BattleBoardInputController> playerPlates;
    private readonly IReadOnlyList<BattleBoardInputController> enemyPlates;
    private readonly BoardOutsideClickInput boardInputController;
    private readonly PlayerTargetSelectionView targetSelectionView;
    private readonly PlayerFeedbackView feedbackView;

    public ExecutePlayerAttackUseCase(
        ICoroutineRunner coroutineRunner,
        AttackStateMachine attackStateMachine,
        IReadOnlyList<BattleBoardInputController> playerPlates,
        IReadOnlyList<BattleBoardInputController> enemyPlates,
        BoardOutsideClickInput boardInputController,
        PlayerTargetSelectionView targetSelectionView,
        PlayerFeedbackView feedbackView)
    {
        this.coroutineRunner = coroutineRunner;
        this.attackStateMachine = attackStateMachine;
        this.playerPlates = playerPlates;
        this.enemyPlates = enemyPlates;
        this.boardInputController = boardInputController;
        this.targetSelectionView = targetSelectionView;
        this.feedbackView = feedbackView;
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
        feedbackView.PlayClick();
        return PlayerActionResult.Completed;
    }

    public PlayerActionResult ExecuteSpecialAttack()
    {
        return ExecuteSpecialAttack(0, null, null, null);
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
            feedbackView.Log("Special attack is cooling down.");
            ResetAttackState();
            feedbackView.PlayFail();
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
            feedbackView.PlayFail();
            return null;
        }

        if (!CanSelectedSummonAttack(attacker))
        {
            feedbackView.Log("선택한 소환수는 공격할 수 없습니다.");
            ResetAttackState();
            feedbackView.PlayFail();
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
            feedbackView.PlayFail();
            return null;
        }

        if (!attackStateMachine.HasCurrentSpecialAttackInfo())
        {
            feedbackView.Log("Selected summon has no special attack data.");
            ResetAttackState();
            feedbackView.PlayFail();
            return null;
        }

        if (!CanSelectedSummonAttack(attacker))
        {
            feedbackView.Log("선택한 소환수는 공격할 수 없습니다.");
            ResetAttackState();
            feedbackView.PlayFail();
            return null;
        }

        return attacker;
    }

    private bool ExecuteImmediateSpecialAttack(Summon attacker, int attackIndex)
    {
        if (!ExecuteSpecialAttack(
                attacker,
                attackStateMachine.GetAttackingPlateIndex(),
                attackIndex,
                true))
        {
            ResetAttackState();
            return false;
        }

        attackStateMachine.CompleteAttack();
        ResetAttackState();
        feedbackView.PlayClick();
        return true;
    }

    private void StartTargetedSpecialAttack(
        Summon attacker,
        int attackIndex,
        Action onWaitTargetSelection,
        Action onCompleted,
        Action onCanceled)
    {
        feedbackView.PlayClick();
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
        feedbackView.Log("플레이어 플레이트를 선택하세요.");
        coroutineRunner.StartCoroutine(WaitForTargetPlateSelection(
            playerPlates,
            false,
            "플레이어 플레이트 선택을 기다립니다.",
            "대상 선택이 취소되었습니다.",
            "특수 공격 대상으로 플레이어 플레이트 {0}번을 선택했습니다.",
            "유효하지 않은 플레이어 플레이트 대상입니다.",
            onTargetSelected,
            onTargetSelectionCanceled));
    }

    private void StartEnemyPlateSelection(Action<int> onTargetSelected, Action onTargetSelectionCanceled)
    {
        feedbackView.Log("적 플레이트를 선택하세요.");
        coroutineRunner.StartCoroutine(WaitForTargetPlateSelection(
            enemyPlates,
            true,
            "적 플레이트 선택을 기다립니다.",
            "대상 선택이 취소되었습니다.",
            "특수 공격 대상으로 적 플레이트 {0}번을 선택했습니다.",
            "유효하지 않은 적 플레이트 대상입니다.",
            onTargetSelected,
            onTargetSelectionCanceled));
    }

    private IEnumerator WaitForTargetPlateSelection(
        IReadOnlyList<BattleBoardInputController> targetPlates,
        bool downTransparencyForPlayerPlate,
        string waitLog,
        string outsideClickLog,
        string executeLogFormat,
        string invalidLog,
        Action<int> onTargetSelected,
        Action onTargetSelectionCanceled)
    {
        attackStateMachine.StartTargetSelection();
        targetSelectionView.ShowTargetSelection(downTransparencyForPlayerPlate);
        attackStateMachine.ClearTargetSelection();
        feedbackView.Log(waitLog);

        while (attackStateMachine.GetSelectedSpecialAttackTargetPlateIndex() < 0)
        {
            if (attackStateMachine.IsSpecialAttackTargetSelectionActive()
                && boardInputController.IsOutsideClick(targetPlates))
            {
                feedbackView.Log(outsideClickLog);
                targetSelectionView.HideTargetSelection();
                attackStateMachine.CancelTargetSelection();
                onTargetSelectionCanceled?.Invoke();
                yield break;
            }

            yield return null;
        }

        int selectedTargetPlateIndex = attackStateMachine.GetSelectedSpecialAttackTargetPlateIndex();
        if (selectedTargetPlateIndex >= 0)
        {
            feedbackView.Log(string.Format(executeLogFormat, selectedTargetPlateIndex));
            targetSelectionView.HideTargetSelection();
            attackStateMachine.ClearTargetSelection();
            onTargetSelected?.Invoke(selectedTargetPlateIndex);
            yield break;
        }

        feedbackView.LogError(invalidLog);
    }

    private void ExecuteSelectedSpecialAttack(
        Summon attacker,
        int attackIndex,
        int selectedTargetPlateIndex,
        Action onCompleted,
        Action onFailed)
    {
        bool attackExecuted = ExecuteSpecialAttack(
            attacker,
            selectedTargetPlateIndex,
            attackIndex,
            true);

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
        targetSelectionView.ResetTargetSelection();
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
            Debug.Log("일반 공격 데이터가 없습니다.");
            return;
        }

        if (!attackSummon.CanUseAttackStrategy(attackStrategy))
        {
            Debug.Log("일반 공격이 쿨타임 중이거나 사용할 수 없습니다.");
            return;
        }

        List<Summon> targets = attackStrategy.SelectTargets(attackSummon, targetPlates, selectedPlateIndex);
        if (targets.Count == 0)
        {
            Debug.Log("일반 공격 대상이 없습니다.");
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
            Debug.LogWarning($"일반 공격으로 적용할 수 없는 상태입니다: {attackStrategy.GetStatusType()}");
            return;
        }

        double damage = attacker.GetAttackPower();
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}을 공격했습니다.");
        target.TakeDamage(damage);
    }

    private bool ExecuteSpecialAttack(
        Summon attackSummon,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        if (!IsValidSpecialAttackIndex(attackSummon, specialAttackIndex))
        {
            Debug.LogError("유효하지 않은 특수 공격 인덱스: " + specialAttackIndex);
            return false;
        }

        AttackData attackStrategy = attackSummon.GetSpecialAttackStrategy()[specialAttackIndex];
        if (!attackSummon.CanUseAttackStrategy(attackStrategy))
        {
            Debug.Log("특수 공격이 쿨타임 중이거나 사용할 수 없습니다.");
            return false;
        }

        if (!CanExecuteTargetedAttack(attackStrategy, selectedPlateIndex, isPlayer))
        {
            Debug.Log(GetTargetedAttackInvalidLog(isPlayer));
            return false;
        }

        IReadOnlyList<BattleBoardInputController> targetPlates = GetSpecialAttackTargetPlates(attackStrategy, isPlayer);

        attackSummon.AttackSoundPlay();
        ApplySpecialAttack(
            attackSummon,
            attackStrategy,
            targetPlates,
            selectedPlateIndex);

        attackSummon.PlayAttackMotion(false);
        attackSummon.ApplyAttackCooldown(attackStrategy);
        attackSummon.SetAttackAvailable(false);
        Debug.Log(GetSpecialAttackSuccessLog(attackStrategy, selectedPlateIndex, isPlayer));
        return true;
    }

    private bool IsValidSpecialAttackIndex(Summon attackSummon, int specialAttackIndex)
    {
        if (attackSummon == null || attackSummon.GetSpecialAttackStrategy() == null)
        {
            return false;
        }

        return specialAttackIndex >= 0
            && specialAttackIndex < attackSummon.GetSpecialAttackStrategy().Length
            && attackSummon.GetSpecialAttackStrategy()[specialAttackIndex] != null;
    }

    private bool CanExecuteTargetedAttack(AttackData attackStrategy, int selectedPlateIndex, bool isPlayer)
    {
        if (!attackStrategy.IsStrategy<TargetedAttackStrategy>() || attackStrategy.TargetsOwnPlates())
        {
            return true;
        }

        IReadOnlyList<BattleBoardInputController> targetPlates = GetSpecialAttackTargetPlates(attackStrategy, isPlayer);
        return selectedPlateIndex >= 0 && selectedPlateIndex < targetPlates.Count;
    }

    private IReadOnlyList<BattleBoardInputController> GetSpecialAttackTargetPlates(AttackData attackStrategy, bool isPlayer)
    {
        bool targetsOwnPlates = attackStrategy.TargetsOwnPlates();

        if (isPlayer == targetsOwnPlates)
        {
            return playerPlates;
        }

        return enemyPlates;
    }

    private void ApplySpecialAttack(
        Summon attacker,
        AttackData attackStrategy,
        IReadOnlyList<BattleBoardInputController> targetPlates,
        int selectedPlateIndex)
    {
        List<Summon> targets = attackStrategy.SelectTargets(attacker, targetPlates, selectedPlateIndex);
        if (targets.Count == 0)
        {
            Debug.Log("특수 공격 대상이 없습니다.");
            return;
        }

        foreach (Summon target in targets)
        {
            ApplySpecialAttackEffect(attacker, attackStrategy, target);
        }
    }

    private void ApplySpecialAttackEffect(Summon attacker, AttackData attackStrategy, Summon target)
    {
        switch (attackStrategy.GetStatusType())
        {
            case StatusType.None:
                ApplyDamage(attacker, attackStrategy, target);
                break;

            case StatusType.Heal:
                ApplyHeal(attacker, attackStrategy, target);
                break;

            case StatusType.LifeDrain:
                ApplyLifeDrain(attacker, attackStrategy, target);
                break;

            case StatusType.Shield:
                ApplyStatus(attacker, target, StatusDataFactory.Create(StatusType.Shield, attackStrategy.GetStatusTime(), attackStrategy.GetSpecialDamage()));
                break;

            case StatusType.Upgrade:
                ApplyStatus(attacker, target, StatusDataFactory.Create(StatusType.Upgrade, attackStrategy.GetStatusTime(), attackStrategy.GetSpecialDamage()));
                break;

            case StatusType.OnceInvincibility:
                ApplyStatus(attacker, target, StatusDataFactory.Create(StatusType.OnceInvincibility, 0, 0));
                break;

            case StatusType.Curse:
                ApplyCurse(attacker, attackStrategy, target);
                break;

            case StatusType.Stun:
                ApplyStatus(attacker, target, StatusDataFactory.Create(StatusType.Stun, attackStrategy.GetStatusTime(), 0));
                break;

            case StatusType.Poison:
            case StatusType.Burn:
                ApplyDamageStatus(attacker, attackStrategy, target);
                break;

            default:
                Debug.LogWarning($"지원하지 않는 특수 공격 상태입니다: {attackStrategy.GetStatusType()}");
                break;
        }
    }

    private void ApplyDamage(Summon attacker, AttackData attackStrategy, Summon target)
    {
        double damage = attackStrategy.GetSpecialDamage();
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}을 공격했습니다.");
        target.TakeDamage(damage);
    }

    private void ApplyHeal(Summon attacker, AttackData attackStrategy, Summon target)
    {
        double healAmount = attackStrategy.IsStrategy<TargetedAttackStrategy>()
            ? Math.Floor((int)target.GetMaxHP() * 0.3)
            : target.GetMaxHP() * attackStrategy.GetSpecialDamage();
        target.ApplyStatus(StatusDataFactory.Create(StatusType.Heal, 0, healAmount));
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}을 {healAmount}만큼 회복시켰습니다.");
    }

    private void ApplyLifeDrain(Summon attacker, AttackData attackStrategy, Summon target)
    {
        double lifeDrainDamage = target.GetMaxHP() * 0.1;
        target.ApplyStatus(StatusDataFactory.Create(StatusType.LifeDrain, attackStrategy.GetStatusTime(), lifeDrainDamage, attacker));
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}에게 흡혈을 적용했습니다.");
    }

    private void ApplyDamageStatus(Summon attacker, AttackData attackStrategy, Summon target)
    {
        double statusDamage = target.GetMaxHP() * attackStrategy.GetSpecialDamage();
        target.ApplyStatus(StatusDataFactory.Create(attackStrategy.GetStatusType(), attackStrategy.GetStatusTime(), statusDamage));
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}에게 {attackStrategy.GetStatusType()} 상태를 적용했습니다.");
    }

    private void ApplyCurse(Summon attacker, AttackData attackStrategy, Summon target)
    {
        double curseValue = attackStrategy.GetSpecialDamage();
        target.ApplyStatus(StatusDataFactory.Create(StatusType.Curse, attackStrategy.GetStatusTime(), curseValue));
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}에게 저주를 적용했습니다.");
    }

    private void ApplyStatus(Summon attacker, Summon target, StatusData status)
    {
        target.ApplyStatus(status);
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}에게 {status.statusType} 상태를 적용했습니다.");
    }

    private string GetSpecialAttackSuccessLog(
        AttackData attackStrategy,
        int selectedPlateIndex,
        bool isPlayer)
    {
        if (attackStrategy.IsStrategy<TargetedAttackStrategy>())
        {
            return isPlayer
                ? $"플레이어가 {selectedPlateIndex}번 플레이트에 대상 지정 특수 공격을 사용했습니다."
                : $"적이 {selectedPlateIndex}번 플레이트에 대상 지정 특수 공격을 사용했습니다.";
        }

        if (attackStrategy.IsStrategy<AttackAllEnemiesStrategy>())
        {
            return isPlayer
                ? "Player used all-target special attack."
                : "Enemy used all-target special attack.";
        }

        if (attackStrategy.IsStrategy<ClosestEnemyAttackStrategy>())
        {
            return isPlayer
                ? "Player used closest-target special attack."
                : "Enemy used closest-target special attack.";
        }

        return "Special attack executed.";
    }

    private string GetTargetedAttackInvalidLog(bool isPlayer)
    {
        return isPlayer
            ? "플레이어 대상 지정 특수 공격의 적 플레이트 인덱스가 유효하지 않습니다."
            : "적 대상 지정 특수 공격의 플레이어 플레이트 인덱스가 유효하지 않습니다.";
    }
}
