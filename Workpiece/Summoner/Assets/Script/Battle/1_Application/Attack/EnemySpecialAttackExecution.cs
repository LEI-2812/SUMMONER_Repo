using System;
using System.Collections.Generic;
using UnityEngine;

// 역할: EnemySpecialAttackExecution의 책임을 정의한다.
public class EnemySpecialAttackExecution
{
    private readonly IReadOnlyList<BattleBoardInputController> playerPlates;
    private readonly IReadOnlyList<BattleBoardInputController> enemyPlates;

    public EnemySpecialAttackExecution(
        IReadOnlyList<BattleBoardInputController> playerPlates,
        IReadOnlyList<BattleBoardInputController> enemyPlates)
    {
        this.playerPlates = playerPlates;
        this.enemyPlates = enemyPlates;
    }

    public bool Execute(
        Summon attackSummon,
        int selectedPlateIndex,
        int specialAttackIndex,
        bool isPlayer)
    {
        if (attackSummon == null)
        {
            Debug.Log("공격 소환수가 없습니다.");
            return false;
        }

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
        return IsValidPlateIndex(selectedPlateIndex, targetPlates.Count);
    }

    private bool IsValidPlateIndex(int selectedPlateIndex, int plateCount)
    {
        return selectedPlateIndex >= 0 && selectedPlateIndex < plateCount;
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
