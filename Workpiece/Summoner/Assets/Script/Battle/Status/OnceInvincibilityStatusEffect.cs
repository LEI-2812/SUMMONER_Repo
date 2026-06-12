public class OnceInvincibilityStatusEffect : StatusEffect, IStatusEffect
{
    public OnceInvincibilityStatusEffect()
        : base(StatusType.OnceInvincibility, 0)
    {
    }

    public StatusType GetStatusType()
    {
        return statusType;
    }

    public int GetRemainingTurn()
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

    public string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}은 이미 1회 무적 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.SetOnceInvincibility(true);
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
    }
}
