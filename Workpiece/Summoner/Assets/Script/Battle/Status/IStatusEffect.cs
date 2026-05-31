public interface IStatusEffect
{
    StatusType StatusTypeGet();
    int RemainingTurnGet();
    bool SameStatusCanApply(StatusEffect existingEffect);
    bool ExistingStatusEffectApply(StatusEffect existingEffect, IStatusEffectTarget target);
    bool StatusTurnCanUpdate(StatusUpdateTiming updateTiming);
    string AlreadyAppliedMessageGet(string targetName);
    void StatusApply(IStatusEffectTarget target);
    void StatusTurnUpdate(IStatusEffectTarget target);
    void StatusExpire(IStatusEffectTarget target);
}
