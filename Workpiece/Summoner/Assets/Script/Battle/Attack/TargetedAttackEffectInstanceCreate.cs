using System;
using UnityEngine;

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

    private class TargetedHealAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => true;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double healAmount = (int)target.getMaxHP() * 0.3;
            healAmount = Math.Floor(healAmount);
            StatusEffect healEffect = StatusEffectInstanceCreate.Create(StatusType.Heal, 0, healAmount);
            target.ApplyStatusEffect(healEffect);
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) {healAmount}만큼 치유했습니다.");
        }
    }

    private class TargetedNormalAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) 강력하게 공격합니다.");
            target.takeDamage(attacker.getSpecialAttackStrategy()[specialAttackArrayIndex].getSpecialDamage());
        }
    }

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
            double lifeDrainDamage = target.getMaxHP() * 0.1;
            StatusEffect drainEffect = StatusEffectInstanceCreate.Create(StatusType.LifeDrain, statusTime, lifeDrainDamage, attacker);
            target.ApplyStatusEffect(drainEffect);
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 흡혈을 사용하여 {lifeDrainDamage} 데미지를 입히고 회복합니다.");
        }
    }

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
            target = attacker;
            StatusEffect shieldEffect = StatusEffectInstanceCreate.Create(StatusType.Shield, statusTime, damage);
            target.ApplyStatusEffect(shieldEffect);
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 {damage} 만큼 보호막을 부여합니다.");
        }
    }

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
            double upgradeAttackPower = attacker.getSpecialAttackStrategy()[specialAttackArrayIndex].getSpecialDamage();
            StatusEffect upgradeEffect = StatusEffectInstanceCreate.Create(StatusType.Upgrade, statusTime, upgradeAttackPower);
            target.ApplyStatusEffect(upgradeEffect);
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 공격력 {(int)(target.getAttackPower() * upgradeAttackPower)} 만큼 상승 시켰습니다.");
        }
    }

    private class TargetedOnceInvincibilityAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => true;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            target = attacker;
            StatusEffect onceInvincibilityEffect = StatusEffectInstanceCreate.Create(StatusType.OnceInvincibility, 0, 0);
            target.ApplyStatusEffect(onceInvincibilityEffect);
        }
    }

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
            double curseAttackPower = attacker.getSpecialAttackStrategy()[specialAttackArrayIndex].getSpecialDamage();
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게  공격력 {curseAttackPower * 100} 만큼 저주를 걸었습니다.");
            StatusEffect curseEffect = StatusEffectInstanceCreate.Create(StatusType.Curse, statusTime, curseAttackPower);
            target.ApplyStatusEffect(curseEffect);
            target.takeDamage(attacker.getSpecialAttackStrategy()[specialAttackArrayIndex].getSpecialDamage());
        }
    }

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
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 혼란을 적용했습니다.");
        }
    }

    private class EmptyAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
        }
    }
}
