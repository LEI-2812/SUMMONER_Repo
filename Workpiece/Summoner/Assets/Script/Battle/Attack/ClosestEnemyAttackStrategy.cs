using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    private StatusType statusType;
    // 실제 근접 피해는 effect에서 attacker.GetAttackPower()로 계산한다.
    // 이 값은 예측/표시/데이터 기준값으로 보존한다.
    private double damage;
    private AttackCooldownState cooldownState;
    private IAttackEffect attackEffect;
    private IAttackTargetSelector targetSelector;

    public ClosestEnemyAttackStrategy(StatusType statusType, double damage, int cooldownDuration, int statusTime=0)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooldownState = new AttackCooldownState(cooldownDuration);
        this.attackEffect = ClosestEnemyAttackEffectInstanceCreate.Create(statusType);
        this.targetSelector = new ClosestEnemyAttackTargetSelector();
    }

    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int specialAttackArrayIndex)
    {
        if (targetPlates == null)
        {
            Debug.LogWarning("Target plates are missing.");
            return;
        }

        List<Summon> targets = targetSelector.SelectTargets(attacker, targetPlates, selectedPlateIndex);

        if (targets.Count > 0)
        {
            attackEffect.AttackEffectApply(attacker, targets[0], specialAttackArrayIndex);
        }
        else
        {
            Debug.Log("공격할 적이 없습니다.");
        }
    }

    public double GetSpecialDamage()
    {
        return damage;
    }

    public bool BenefitEffectCheck() => attackEffect.BenefitEffectCheck();

    public StatusType GetStatusType() { return statusType; }
    
    public int GetCooltime() { return cooldownState.GetCooltime(); }

    public int GetCurrentCooldown() => cooldownState.GetCurrentCooldown();

    // 쿨타임을 초기화 (스킬 사용 후 적용)
    public void ApplyCooldown() => cooldownState.ApplyCooldown();

    // 턴 종료 시 쿨타임 감소
    public void ReduceCooldown() => cooldownState.ReduceCooldown();
}
