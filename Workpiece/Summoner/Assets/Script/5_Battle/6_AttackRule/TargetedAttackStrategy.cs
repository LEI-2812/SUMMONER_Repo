using System.Collections.Generic;
using UnityEngine;

// 역할: 선택한 단일 플레이트 대상에게 공격 효과를 적용하는 전략이다.
public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // 상태 타입
    private double damage; // 데미지
    private AttackCooldownState cooldownState;
    private IAttackEffect attackEffect;
    private IAttackTargetSelector targetSelector;

    public TargetedAttackStrategy(StatusType statusType, double damage, int cooldownDuration, int statusTime=0)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooldownState = new AttackCooldownState(cooldownDuration);
        this.attackEffect = TargetedAttackEffectInstanceCreate.Create(statusType, statusTime, damage);
        this.targetSelector = CreateTargetSelector(statusType);
    }

    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int specialAttackArrayIndex)
    {
        List<Summon> targets = targetSelector.SelectTargets(attacker, targetPlates, selectedPlateIndex);

        if (targets.Count > 0)
        {
            attackEffect.AttackEffectApply(attacker, targets[0], specialAttackArrayIndex);
        }
        else
        {
            Debug.Log("선택한 plate에 대상이 없습니다.");
        }
    }

    private IAttackTargetSelector CreateTargetSelector(StatusType statusType)
    {
        if (statusType == StatusType.Shield || statusType == StatusType.OnceInvincibility)
        {
            return new SelfAttackTargetSelector();
        }

        return new SelectedPlateAttackTargetSelector();
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

    public int GetCooltime() { return cooldownState.GetCooltime(); }

    public int GetCurrentCooldown() => cooldownState.GetCurrentCooldown();

    // 쿨타임을 초기화한다. (스킬 사용 후 적용)
    public void ApplyCooldown() => cooldownState.ApplyCooldown();

    // 턴 종료 후 쿨타임 감소
    public void ReduceCooldown() => cooldownState.ReduceCooldown();
}
