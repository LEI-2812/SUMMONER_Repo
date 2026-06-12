public interface IStatusEffectTarget
{
    // Target info
    string GetStatusTargetName();
    double GetHealth();
    double GetAttackPower();

    // Battle state
    void DamageTake(double damage);
    void HealReceive(double healAmount);
    void SetAttackAvailable(bool canAttack);
    void ShieldAdd(double shieldAmount);
    void SetShield(double shieldAmount);
    void AttackPowerUpgrade(double multiplier);
    void AttackPowerCurse(double curseRate);
    void AttackPowerRestore(double originAttack);
    void SetOnceInvincibility(bool isInvincibility);

    // Feedback
    void StatusHitColorShow();
    void DebuffSoundPlay();
    void BuffSoundPlay();

    // Observer update
    void StatusChangedNotify();
}
