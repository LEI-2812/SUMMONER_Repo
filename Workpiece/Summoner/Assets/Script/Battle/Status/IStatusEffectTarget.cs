public interface IStatusEffectTarget
{
    string StatusTargetNameGet();
    double HealthGet();
    double AttackPowerGet();

    void DamageTake(double damage);
    void HealReceive(double healAmount);
    void AttackAvailableSet(bool canAttack);
    void ShieldAdd(double shieldAmount);
    void ShieldSet(double shieldAmount);
    void AttackPowerUpgrade(double multiplier);
    void AttackPowerCurse(double curseRate);
    void AttackPowerRestore(double originAttack);
    void OnceInvincibilitySet(bool isInvincibility);
    void StatusHitColorShow();
    void DebuffSoundPlay();
    void BuffSoundPlay();
    void StatusChangedNotify();
}
