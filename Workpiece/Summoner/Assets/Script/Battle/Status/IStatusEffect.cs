public interface IStatusEffect
{
    StatusType GetStatusType();
    int GetRemainingTurn();
    bool SameStatusCanApply(StatusEffect existingEffect);
    bool ExistingStatusEffectApply(StatusEffect existingEffect, IStatusEffectTarget target);
    bool StatusTurnCanUpdate(StatusUpdateTiming updateTiming);
    string GetAlreadyAppliedMessage(string targetName);
    void StatusApply(IStatusEffectTarget target);
    void StatusTurnUpdate(IStatusEffectTarget target);
    void StatusExpire(IStatusEffectTarget target);
}
