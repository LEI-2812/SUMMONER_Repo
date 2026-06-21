using System;
using System.Collections.Generic;
using UnityEngine;

// 역할: 선택 대상 하나에게 직접 공격, 회복, 버프, 디버프 효과를 적용한다.
public class TargetedAttackStrategy : IAttackStrategy
{
    // 역할: 어떤 공격/상태 효과를 적용할지 구분한다.
    private StatusType statusType;

    // 역할: StatusType에 따라 피해량, 보호막 수치, 버프/디버프 비율로 해석되는 효과 값이다.
    private double effectValue;

    // 역할: 상태 효과가 유지될 턴 수를 보관한다.
    private int statusTime;

    // 역할: 이 공격 전략의 쿨타임 상태를 관리한다.
    private AttackCooldownState cooldownState;

    // 역할: 선택 인덱스 또는 자기 자신 기준으로 실제 대상 소환수를 찾는다.
    private IAttackTargetSelector targetSelector;

    public TargetedAttackStrategy(StatusType statusType, double damage, int cooldownDuration, int statusTime=0)
    {
        this.statusType = statusType;
        this.effectValue = damage;
        this.statusTime = statusTime;
        this.cooldownState = new AttackCooldownState(cooldownDuration);
        this.targetSelector = CreateTargetSelector(statusType);
    }

    public void Attack(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = targetSelector.SelectTargets(attacker, targetPlates, selectedPlateIndex);

        if (targets.Count == 0)
        {
            Debug.Log("선택한 plate에 대상이 없습니다.");
            return;
        }

        ApplyEffectToTarget(attacker, targets[0]);
    }

    // 역할: Shield/OnceInvincibility는 공격자 자신에게, Heal/Upgrade는 선택한 아군 플레이트에 적용한다.
    private IAttackTargetSelector CreateTargetSelector(StatusType statusType)
    {
        if (statusType == StatusType.Shield || statusType == StatusType.OnceInvincibility)
        {
            return new SelfAttackTargetSelector();
        }

        return new SelectedPlateAttackTargetSelector();
    }

    // 역할: 상태 타입에 맞는 실제 효과 적용 메서드로 분기한다.
    private void ApplyEffectToTarget(Summon attacker, Summon target)
    {
        switch (statusType)
        {
            case StatusType.None:
                ApplyDamage(attacker, target);
                break;

            case StatusType.Heal:
                ApplyHeal(attacker, target);
                break;

            case StatusType.LifeDrain:
                ApplyLifeDrain(attacker, target);
                break;

            case StatusType.Shield:
                ApplyShield(attacker, target);
                break;

            case StatusType.Upgrade:
                ApplyUpgrade(attacker, target);
                break;

            case StatusType.OnceInvincibility:
                ApplyOnceInvincibility(target);
                break;

            case StatusType.Curse:
                ApplyCurse(attacker, target);
                break;

            case StatusType.Stun:
                ApplyStun(attacker, target);
                break;

            default:
                Debug.LogWarning($"TargetedAttackStrategy에서 처리하지 않는 상태 타입입니다: {statusType}");
                break;
        }
    }

    // 역할: 선택한 대상 하나에게 고정 피해를 준다.
    private void ApplyDamage(Summon attacker, Summon target)
    {
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을 강하게 공격합니다.");
        target.TakeDamage(effectValue);
    }

    // 역할: 선택한 대상 하나를 최대 체력의 30%만큼 회복한다. 현재 effectValue는 사용하지 않는다.
    private void ApplyHeal(Summon attacker, Summon target)
    {
        double healAmount = Math.Floor((int)target.GetMaxHP() * 0.3);
        target.ApplyStatusEffect(StatusEffectInstanceCreate.Create(StatusType.Heal, 0, healAmount));
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을 {healAmount}만큼 치유했습니다.");
    }

    // 역할: 선택한 대상 하나에게 최대 체력의 10% 기준 흡혈 상태를 적용한다. 현재 effectValue는 사용하지 않는다.
    private void ApplyLifeDrain(Summon attacker, Summon target)
    {
        double lifeDrainDamage = target.GetMaxHP() * 0.1;
        StatusEffect drainEffect = StatusEffectInstanceCreate.Create(StatusType.LifeDrain, statusTime, lifeDrainDamage, attacker);
        target.ApplyStatusEffect(drainEffect);
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 흡혈 상태를 적용했습니다.");
    }

    // 역할: 자기 자신에게 보호막 상태를 적용한다.
    private void ApplyShield(Summon attacker, Summon target)
    {
        StatusEffect shieldEffect = StatusEffectInstanceCreate.Create(StatusType.Shield, statusTime, effectValue);
        target.ApplyStatusEffect(shieldEffect);
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 보호막을 적용했습니다.");
    }

    // 역할: 선택한 대상 하나의 공격력을 강화한다.
    private void ApplyUpgrade(Summon attacker, Summon target)
    {
        StatusEffect upgradeEffect = StatusEffectInstanceCreate.Create(StatusType.Upgrade, statusTime, effectValue);
        target.ApplyStatusEffect(upgradeEffect);
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}의 공격력을 강화했습니다.");
    }

    // 역할: 자기 자신에게 1회 무적 상태를 적용한다.
    private void ApplyOnceInvincibility(Summon target)
    {
        target.ApplyStatusEffect(StatusEffectInstanceCreate.Create(StatusType.OnceInvincibility, 0, 0));
    }

    // 역할: 선택한 대상 하나에게 저주 상태와 즉시 피해를 함께 적용한다.
    private void ApplyCurse(Summon attacker, Summon target)
    {
        StatusEffect curseEffect = StatusEffectInstanceCreate.Create(StatusType.Curse, statusTime, effectValue);
        target.ApplyStatusEffect(curseEffect);
        target.TakeDamage(effectValue);
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 저주를 적용했습니다.");
    }

    // 역할: 선택한 대상 하나에게 스턴 상태를 적용한다.
    private void ApplyStun(Summon attacker, Summon target)
    {
        target.ApplyStatusEffect(StatusEffectInstanceCreate.Create(StatusType.Stun, statusTime, 0));
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 스턴을 적용했습니다.");
    }

    // 역할: 이 공격이 적이 아니라 아군 플레이트를 대상으로 해야 하는지 알려준다.
    public bool TargetsOwnPlates()
    {
        return statusType == StatusType.Heal
            || statusType == StatusType.Shield
            || statusType == StatusType.Upgrade
            || statusType == StatusType.OnceInvincibility;
    }

    // 역할: 이 전략이 가진 피해량/효과 수치를 알려준다.
    public double GetSpecialDamage()
    {
        return effectValue;
    }

    // 역할: 이 전략의 상태 타입을 알려준다.
    public StatusType GetStatusType()
    {
        return statusType;
    }

    // 역할: 공격 사용 후 적용할 기본 쿨타임을 알려준다.
    public int GetCooltime() { return cooldownState.GetCooltime(); }

    // 역할: 현재 남은 쿨타임을 알려준다.
    public int GetCurrentCooldown() => cooldownState.GetCurrentCooldown();

    // 역할: 공격 사용 직후 쿨타임을 적용한다.
    public void ApplyCooldown() => cooldownState.ApplyCooldown();

    // 역할: 턴 종료 시 남은 쿨타임을 줄인다.
    public void ReduceCooldown() => cooldownState.ReduceCooldown();
}
