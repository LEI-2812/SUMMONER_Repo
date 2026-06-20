// 역할: 상태 효과가 적용, 턴 갱신, 만료 처리를 위해 지켜야 하는 계약을 정의한다.
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
