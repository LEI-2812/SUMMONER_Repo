using System;
using UnityEngine;

// 역할: 상태 타입에 맞는 단일 대상 공격 효과 객체를 생성한다.
public static class TargetedAttackEffectInstanceCreate
{
    public static IAttackEffect Create(StatusType statusType, int statusTime, double damage)
    {
        switch (statusType)
        {
            case StatusType.Heal:
                return new TargetedHealAttackEffect();

            case StatusType.None:
                return new TargetedNormalAttackEffect();

            case StatusType.LifeDrain:
                return new TargetedLifeDrainAttackEffect(statusTime);

            case StatusType.Shield:
                return new TargetedShieldAttackEffect(statusTime, damage);

            case StatusType.Upgrade:
                return new TargetedUpgradeAttackEffect(statusTime);

            case StatusType.OnceInvincibility:
                return new TargetedOnceInvincibilityAttackEffect();

            case StatusType.Curse:
                return new TargetedCurseAttackEffect(statusTime);

            case StatusType.Stun:
                return new TargetedStunAttackEffect(statusTime);

            default:
                return new EmptyAttackEffect();
        }
    }

    // 역할: 단일 대상에게 회복 상태 효과를 적용한다.
    private class TargetedHealAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => true;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double healAmount = (int)target.GetMaxHP() * 0.3;
            healAmount = Math.Floor(healAmount);
            StatusEffect healEffect = StatusEffectInstanceCreate.Create(StatusType.Heal, 0, healAmount);
            target.ApplyStatusEffect(healEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을(를) {healAmount}만큼 치유했습니다.");
        }
    }

    // 역할: 단일 대상에게 특수 공격 피해를 적용한다.
    private class TargetedNormalAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을(를) 강력하게 공격합니다.");
            target.TakeDamage(attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage());
        }
    }

    // 역할: 단일 대상에게 흡혈 상태 효과를 적용한다.
    private class TargetedLifeDrainAttackEffect : IAttackEffect
    {
        private int statusTime;

        public TargetedLifeDrainAttackEffect(int statusTime)
        {
            this.statusTime = statusTime;
        }

        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double lifeDrainDamage = target.GetMaxHP() * 0.1;
            StatusEffect drainEffect = StatusEffectInstanceCreate.Create(StatusType.LifeDrain, statusTime, lifeDrainDamage, attacker);
            target.ApplyStatusEffect(drainEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 흡혈을 사용하여 {lifeDrainDamage} 데미지를 입히고 회복합니다.");
        }
    }

    // 역할: 단일 대상에게 보호막 상태 효과를 적용한다.
    private class TargetedShieldAttackEffect : IAttackEffect
    {
        private int statusTime;
        private double damage;

        public TargetedShieldAttackEffect(int statusTime, double damage)
        {
            this.statusTime = statusTime;
            this.damage = damage;
        }

        public bool BenefitEffectCheck() => true;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            StatusEffect shieldEffect = StatusEffectInstanceCreate.Create(StatusType.Shield, statusTime, damage);
            target.ApplyStatusEffect(shieldEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 {damage} 만큼 보호막을 부여합니다.");
        }
    }

    // 역할: 단일 대상에게 공격력 강화 상태 효과를 적용한다.
    private class TargetedUpgradeAttackEffect : IAttackEffect
    {
        private int statusTime;

        public TargetedUpgradeAttackEffect(int statusTime)
        {
            this.statusTime = statusTime;
        }

        public bool BenefitEffectCheck() => true;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double upgradeAttackPower = attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage();
            StatusEffect upgradeEffect = StatusEffectInstanceCreate.Create(StatusType.Upgrade, statusTime, upgradeAttackPower);
            target.ApplyStatusEffect(upgradeEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 공격력 {(int)(target.GetAttackPower() * upgradeAttackPower)} 만큼 상승 시켰습니다.");
        }
    }

    // 역할: 단일 대상에게 1회 무적 상태 효과를 적용한다.
    private class TargetedOnceInvincibilityAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => true;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            StatusEffect onceInvincibilityEffect = StatusEffectInstanceCreate.Create(StatusType.OnceInvincibility, 0, 0);
            target.ApplyStatusEffect(onceInvincibilityEffect);
        }
    }

    // 역할: 단일 대상에게 저주 상태 효과와 피해를 적용한다.
    private class TargetedCurseAttackEffect : IAttackEffect
    {
        private int statusTime;

        public TargetedCurseAttackEffect(int statusTime)
        {
            this.statusTime = statusTime;
        }

        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double curseAttackPower = attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage();
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게  공격력 {curseAttackPower * 100} 만큼 저주를 걸었습니다.");
            StatusEffect curseEffect = StatusEffectInstanceCreate.Create(StatusType.Curse, statusTime, curseAttackPower);
            target.ApplyStatusEffect(curseEffect);
            target.TakeDamage(attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage());
        }
    }

    // 역할: 단일 대상에게 스턴 상태 효과를 적용한다.
    private class TargetedStunAttackEffect : IAttackEffect
    {
        private int statusTime;

        public TargetedStunAttackEffect(int statusTime)
        {
            this.statusTime = statusTime;
        }

        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            StatusEffect stunEffect = StatusEffectInstanceCreate.Create(StatusType.Stun, statusTime, 0);
            target.ApplyStatusEffect(stunEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 혼란을 적용했습니다.");
        }
    }

    // 역할: 해당 상태 타입에서 실제 효과가 없을 때 사용하는 빈 공격 효과다.
    private class EmptyAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
        }
    }
}
