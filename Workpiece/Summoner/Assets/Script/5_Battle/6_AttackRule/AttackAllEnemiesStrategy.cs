using System.Collections.Generic;
using UnityEngine;

// 역할: 선택된 대상 진영의 모든 소환수에게 직접 공격, 상태이상, 버프, 회복 효과를 적용한다.
public class AttackAllEnemiesStrategy : IAttackStrategy
{
    // 역할: 전체 대상에게 어떤 공격/상태 효과를 적용할지 구분한다.
    private StatusType statusType = StatusType.None;

    // 역할: StatusType에 따라 직접 피해량, 최대 체력 비율, 강화 수치, 회복 비율로 해석되는 효과 값이다.
    private double effectValue;

    // 역할: 상태 효과가 유지될 턴 수를 보관한다.
    private int statusTime;

    // 역할: 이 공격 전략의 쿨타임 상태를 관리한다.
    private AttackCooldownState cooldownState;

    // 역할: 대상 플레이트 목록에서 공격받을 모든 소환수를 찾는다.
    private IAttackTargetSelector targetSelector;

    public AttackAllEnemiesStrategy(StatusType statusType, double damage, int cooldownDuration, int statusTime=0)
    {
        this.statusType = statusType;
        this.effectValue = damage;
        this.statusTime = statusTime;
        this.cooldownState = new AttackCooldownState(cooldownDuration);
        this.targetSelector = new AllEnemiesAttackTargetSelector();
    }

    public void Attack(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
    {
        if (targetPlates == null)
        {
            Debug.LogWarning("Target plates are missing.");
            return;
        }

        List<Summon> targets = targetSelector.SelectTargets(attacker, targetPlates, selectedPlateIndex);

        foreach (Summon target in targets)
        {
            ApplyEffectToTarget(attacker, target);
        }
    }

    // 역할: 상태 타입에 맞는 전체 대상 효과 적용 메서드로 분기한다.
    private void ApplyEffectToTarget(Summon attacker, Summon target)
    {
        switch (statusType)
        {
            case StatusType.None:
                ApplyDamage(attacker, target);
                break;

            case StatusType.Poison:
                ApplyPoison(attacker, target);
                break;

            case StatusType.Burn:
                ApplyBurn(attacker, target);
                break;

            case StatusType.Upgrade:
                ApplyUpgrade(attacker, target);
                break;

            case StatusType.Heal:
                ApplyHeal(attacker, target);
                break;

            default:
                Debug.LogWarning($"AttackAllEnemiesStrategy에서 처리하지 않는 상태 타입입니다: {statusType}");
                break;
        }
    }

    // 역할: 전체 대상 중 현재 대상 하나에게 고정 피해를 준다.
    private void ApplyDamage(Summon attacker, Summon target)
    {
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을 전체 공격합니다.");
        target.TakeDamage(effectValue);
    }

    // 역할: 전체 대상 중 현재 대상 하나에게 중독 상태를 적용한다.
    private void ApplyPoison(Summon attacker, Summon target)
    {
        ApplyDamageStatus(attacker, target, StatusType.Poison);
    }

    // 역할: 전체 대상 중 현재 대상 하나에게 화상 상태를 적용한다.
    private void ApplyBurn(Summon attacker, Summon target)
    {
        ApplyDamageStatus(attacker, target, StatusType.Burn);
    }

    // 역할: 최대 체력 기준 지속 피해량을 계산해 상태 효과를 적용한다.
    private void ApplyDamageStatus(Summon attacker, Summon target, StatusType damageStatusType)
    {
        double statusDamage = target.GetMaxHP() * effectValue;
        StatusEffect statusEffect = StatusEffectInstanceCreate.Create(damageStatusType, statusTime, statusDamage);
        target.ApplyStatusEffect(statusEffect);
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 {damageStatusType} 상태를 적용했습니다.");
    }

    // 역할: 전체 대상 중 현재 대상 하나의 공격력을 강화한다.
    private void ApplyUpgrade(Summon attacker, Summon target)
    {
        StatusEffect upgradeEffect = StatusEffectInstanceCreate.Create(StatusType.Upgrade, statusTime, effectValue);
        target.ApplyStatusEffect(upgradeEffect);
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}의 공격력을 강화했습니다.");
    }

    // 역할: 전체 대상 중 현재 대상 하나를 최대 체력 기준으로 회복한다.
    private void ApplyHeal(Summon attacker, Summon target)
    {
        double healAmount = target.GetMaxHP() * effectValue;
        StatusEffect healEffect = StatusEffectInstanceCreate.Create(StatusType.Heal, 0, healAmount);
        target.ApplyStatusEffect(healEffect);
        Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을 {healAmount}만큼 치유했습니다.");
    }

    // 역할: 이 공격이 적이 아니라 아군 플레이트를 대상으로 해야 하는지 알려준다.
    public bool TargetsOwnPlates()
    {
        return statusType == StatusType.Upgrade || statusType == StatusType.Heal;
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
    public int GetCooltime()
    {
        return cooldownState.GetCooltime();
    }

    // 역할: 현재 남은 쿨타임을 알려준다.
    public int GetCurrentCooldown() => cooldownState.GetCurrentCooldown();

    // 역할: 공격 사용 직후 쿨타임을 적용한다.
    public void ApplyCooldown() => cooldownState.ApplyCooldown();

    // 역할: 턴 종료 시 남은 쿨타임을 줄인다.
    public void ReduceCooldown() => cooldownState.ReduceCooldown();
}
