using UnityEngine;

public class CurseStatusEffect : StatusEffect, IStatusEffect
{
    public CurseStatusEffect(int effectTime, double curseRate)
        : base(StatusType.Curse, effectTime, curseRate)
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
        return $"{targetName}은 이미 저주 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.StatusHitColorShow();

        if (ShouldApplyOnce())
        {
            target.AttackPowerCurse(damagePerTurn);
            SetApplyOnce();
            target.StatusChangedNotify();
        }

        target.DebuffSoundPlay();
        Debug.Log($"{target.StatusTargetNameGet()}에게 저주 상태이상이 적용되었습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
        effectTime--;
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        Debug.Log($"{target.StatusTargetNameGet()}의 저주 상태이상이 종료되었습니다.");
    }
}
