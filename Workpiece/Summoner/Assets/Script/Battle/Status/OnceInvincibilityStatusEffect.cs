public class OnceInvincibilityStatusEffect : StatusEffect, IStatusEffect
{
    public OnceInvincibilityStatusEffect()
        : base(StatusType.OnceInvincibility, 0)
    {
    }

    public StatusType StatusTypeGet()
    {
        return statusType;
    }

    public int RemainingTurnGet()
    {
        return 0;
    }

    public bool SameStatusCanApply(StatusEffect existingEffect)
    {
        return true;
    }

    public bool ExistingStatusEffectApply(StatusEffect existingEffect, IStatusEffectTarget target)
    {
        return false;
    }

    public bool StatusTurnCanUpdate(StatusUpdateTiming updateTiming)
    {
        return false;
    }

    public string AlreadyAppliedMessageGet(string targetName)
    {
        return $"{targetName}은 이미 1회 무적 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.OnceInvincibilitySet(true);
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
    }
}
