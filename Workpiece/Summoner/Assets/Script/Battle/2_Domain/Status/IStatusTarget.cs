// 역할: IStatusTarget 구현체가 지켜야 할 계약을 정의한다.
public interface IStatusTarget
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
