using System.Collections.Generic;
using UnityEngine;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    private StatusType statusType = StatusType.None;
    private double damage;
    private AttackCooldownState cooldownState;
    private IAttackEffect attackEffect;
    private IAttackTargetSelector targetSelector;

    public AttackAllEnemiesStrategy(StatusType statusType, double damage, int cooldownDuration, int statusTime=0)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooldownState = new AttackCooldownState(cooldownDuration);
        this.attackEffect = AllEnemiesAttackEffectInstanceCreate.Create(statusType, statusTime);
        this.targetSelector = new AllEnemiesAttackTargetSelector();
    }

    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int specialAttackArrayIndex)
    {
        if (targetPlates == null)
        {
            Debug.LogWarning("Target plates are missing.");
            return;
        }

        List<Summon> targets = targetSelector.SelectTargets(attacker, targetPlates, selectedPlateIndex);

        foreach (Summon target in targets)
        {
            attackEffect.AttackEffectApply(attacker, target, specialAttackArrayIndex);
        }
    }

    public bool BenefitEffectCheck() => attackEffect.BenefitEffectCheck();

    public double GetSpecialDamage()
    {
        return damage;
    }

    public StatusType GetStatusType()
    {
        return statusType;
    }

    public int GetCooltime()
    {
        return cooldownState.GetCooltime();
    }

    public int GetCurrentCooldown() => cooldownState.GetCurrentCooldown();

    // 쿨타임을 초기화한다. (스킬 사용 후 적용)
    public void ApplyCooldown() => cooldownState.ApplyCooldown();

    // 턴 종료 후 쿨타임 감소
    public void ReduceCooldown() => cooldownState.ReduceCooldown();
}
