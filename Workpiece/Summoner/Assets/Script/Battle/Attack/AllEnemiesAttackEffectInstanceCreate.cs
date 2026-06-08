using UnityEngine;

public static class AllEnemiesAttackEffectInstanceCreate
{
    public static IAttackEffect Create(StatusType statusType, int statusTime)
    {
        switch (statusType)
        {
            case StatusType.None:
                return new AllEnemiesNormalAttackEffect();

            case StatusType.Poison:
                return new AllEnemiesPoisonAttackEffect(statusTime);

            case StatusType.Burn:
                return new AllEnemiesBurnAttackEffect(statusTime);

            case StatusType.Upgrade:
                return new AllEnemiesUpgradeAttackEffect(statusTime);

            case StatusType.Heal:
                return new AllEnemiesHealAttackEffect();

            default:
                return new EmptyAttackEffect();
        }
    }

    private class AllEnemiesNormalAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을(를) 전체 공격합니다.");
            target.TakeDamage(attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage());
        }
    }

    private class AllEnemiesPoisonAttackEffect : IAttackEffect
    {
        private int statusTime;

        public AllEnemiesPoisonAttackEffect(int statusTime)
        {
            this.statusTime = statusTime;
        }

        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double poisonDamage = target.GetMaxHP() * attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage();
            StatusEffect poisonEffect = StatusEffectInstanceCreate.Create(StatusType.Poison, statusTime, poisonDamage);
            target.ApplyStatusEffect(poisonEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 중독 상태를 부여하여 매 턴 {poisonDamage} 데미지를 입힙니다.");
        }
    }

    private class AllEnemiesBurnAttackEffect : IAttackEffect
    {
        private int statusTime;

        public AllEnemiesBurnAttackEffect(int statusTime)
        {
            this.statusTime = statusTime;
        }

        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double burnDamage = target.GetMaxHP() * attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage();
            StatusEffect burnEffect = StatusEffectInstanceCreate.Create(StatusType.Burn, statusTime, burnDamage);
            target.ApplyStatusEffect(burnEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}에게 화상을 입혀 매 턴 {burnDamage} 데미지를 입힙니다.");
        }
    }

    private class AllEnemiesUpgradeAttackEffect : IAttackEffect
    {
        private int statusTime;

        public AllEnemiesUpgradeAttackEffect(int statusTime)
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

    private class AllEnemiesHealAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => true;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            double healAmount = target.GetMaxHP() * attacker.GetSpecialAttackStrategy()[specialAttackArrayIndex].GetSpecialDamage();
            StatusEffect healEffect = StatusEffectInstanceCreate.Create(StatusType.Heal, 0, healAmount);
            target.ApplyStatusEffect(healEffect);
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을(를) {healAmount}만큼 치유했습니다.");
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
