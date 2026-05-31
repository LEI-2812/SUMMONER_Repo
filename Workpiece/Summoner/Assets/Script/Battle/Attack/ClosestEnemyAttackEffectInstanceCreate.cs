using UnityEngine;

public static class ClosestEnemyAttackEffectInstanceCreate
{
    public static IAttackEffect Create(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.None:
                return new ClosestEnemyNormalAttackEffect();

            default:
                return new EmptyAttackEffect();
        }
    }

    private class ClosestEnemyNormalAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) 공격합니다.");
            target.takeDamage(attacker.getAttackPower());
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
