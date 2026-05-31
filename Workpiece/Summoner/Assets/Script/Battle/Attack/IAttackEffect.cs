public interface IAttackEffect
{
    bool BenefitEffectCheck();
    void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex);
}
