using System.Collections.Generic;
using UnityEngine;

// 역할: 가장 가까운 대상 하나에게 현재 공격력 기반 피해를 적용한다.
public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    // 역할: 근접 기본 공격 여부를 보관한다. 이 전략은 StatusType.None만 실제 피해를 준다.
    private StatusType statusType;

    // 실제 근접 피해는 attacker.GetAttackPower()로 계산한다.
    // 이 값은 Attack() 실행 피해가 아니라 GetSpecialDamage() 예측/표시 기준값이다.
    private double displayDamage;

    // 역할: 이 공격 전략의 쿨타임 상태를 관리한다.
    private AttackCooldownState cooldownState;

    // 역할: 대상 플레이트 목록에서 가장 가까운 대상 소환수를 찾는다.
    private IAttackTargetSelector targetSelector;

    public ClosestEnemyAttackStrategy(StatusType statusType, double damage, int cooldownDuration, int statusTime=0)
    {
        this.statusType = statusType;
        this.displayDamage = damage;
        this.cooldownState = new AttackCooldownState(cooldownDuration);
        this.targetSelector = new ClosestEnemyAttackTargetSelector();
    }

    public void Attack(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
    {
        if (targetPlates == null)
        {
            Debug.LogWarning("Target plates are missing.");
            return;
        }

        List<Summon> targets = targetSelector.SelectTargets(attacker, targetPlates, selectedPlateIndex);

        if (targets.Count == 0)
        {
            Debug.Log("공격할 대상이 없습니다.");
            return;
        }

        ApplyEffectToTarget(attacker, targets[0]);
    }

    // 역할: 가장 가까운 대상 하나에게 현재 공격력 기반 피해를 적용한다.
    private void ApplyEffectToTarget(Summon attacker, Summon target)
    {
        if (statusType != StatusType.None)
        {
            Debug.LogWarning($"ClosestEnemyAttackStrategy는 StatusType.None만 처리합니다: {statusType}");
            return;
        }

        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을 공격합니다.");
        // 버프/저주/강공격 전환이 반영된 현재 공격력을 사용한다.
        target.TakeDamage(attacker.GetAttackPower());
    }

    // 역할: 예측/표시에 사용할 기준 피해 값을 알려준다.
    public double GetSpecialDamage()
    {
        return displayDamage;
    }

    // 역할: 근접 공격은 적 플레이트를 대상으로 한다고 알려준다.
    public bool TargetsOwnPlates() => false;

    // 역할: 이 전략의 상태 타입을 알려준다.
    public StatusType GetStatusType() { return statusType; }

    // 역할: 공격 사용 후 적용할 기본 쿨타임을 알려준다.
    public int GetCooltime() { return cooldownState.GetCooltime(); }

    // 역할: 현재 남은 쿨타임을 알려준다.
    public int GetCurrentCooldown() => cooldownState.GetCurrentCooldown();

    // 역할: 공격 사용 직후 쿨타임을 적용한다.
    public void ApplyCooldown() => cooldownState.ApplyCooldown();

    // 역할: 턴 종료 시 남은 쿨타임을 줄인다.
    public void ReduceCooldown() => cooldownState.ReduceCooldown();
}
