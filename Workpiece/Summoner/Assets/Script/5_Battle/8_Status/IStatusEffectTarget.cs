// 역할: 상태 효과가 소환수에게 피해, 회복, 버프를 적용하기 위한 대상 계약을 정의한다.
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
