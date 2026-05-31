using UnityEngine;

public class BurnStatusEffect : StatusEffect, IStatusEffect
{
    public BurnStatusEffect(int effectTime, double damagePerTurn)
        : base(StatusType.Burn, effectTime, damagePerTurn)
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
        return updateTiming == StatusUpdateTiming.Damage;
    }

    public string AlreadyAppliedMessageGet(string targetName)
    {
        return $"{targetName}은 이미 화상 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.StatusHitColorShow();
        target.DamageTake(damagePerTurn);
        effectTime--;
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.StatusTargetNameGet()}에게 화상 상태이상이 적용되었습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
        if (effectTime <= 0)
        {
            return;
        }

        Debug.Log($"{target.StatusTargetNameGet()}이 화상 상태로 인해 {damagePerTurn} 피해를 입습니다. 남은 상태이상 시간: {effectTime}턴");
        target.DamageTake(damagePerTurn);
        effectTime--;
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        Debug.Log($"{target.StatusTargetNameGet()}의 화상 상태이상이 종료되었습니다.");
    }
}
