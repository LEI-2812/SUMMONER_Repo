using UnityEngine;

public class StunStatusEffect : StatusEffect, IStatusEffect
{
    public StunStatusEffect(int effectTime)
        : base(StatusType.Stun, effectTime)
    {
    }

    public StatusType StatusTypeGet()
    {
        return statusType;
    }

    public int RemainingTurnGet()
    {
        return effectTime;
    }

    public bool SameStatusCanApply(StatusEffect existingEffect)
    {
        return existingEffect == null;
    }

    public bool ExistingStatusEffectApply(StatusEffect existingEffect, IStatusEffectTarget target)
    {
        return false;
    }

    public bool StatusTurnCanUpdate(StatusUpdateTiming updateTiming)
    {
        return updateTiming == StatusUpdateTiming.StunAndCurse;
    }

    public string AlreadyAppliedMessageGet(string targetName)
    {
        return $"{targetName}은 이미 스턴 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.StatusHitColorShow();
        target.AttackAvailableSet(false);
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.StatusTargetNameGet()}이 스턴 상태가 되었습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
        target.AttackAvailableSet(false);
        Debug.Log($"{target.StatusTargetNameGet()}은 스턴 상태로 공격할 수 없습니다.");
        effectTime--;
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        target.AttackAvailableSet(true);
        Debug.Log($"{target.StatusTargetNameGet()}의 스턴이 해제되었습니다. 공격 가능");
    }
}
