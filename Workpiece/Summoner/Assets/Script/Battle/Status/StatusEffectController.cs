using System.Collections.Generic;
using UnityEngine;

public class StatusEffectController
{
    private List<StatusEffect> activeStatusEffects;

    public StatusEffectController(List<StatusEffect> activeStatusEffects)
    {
        this.activeStatusEffects = activeStatusEffects;
    }

    // 상태이상을 적용한다. 같은 상태이상은 중복 적용하지 않는다.
    public void StatusEffectApply(StatusEffect statusEffect, IStatusEffectTarget target)
    {
        if (statusEffect == null)
        {
            Debug.Log($"{target.StatusTargetNameGet()}에게 적용할 상태이상이 없습니다.");
            return;
        }

        IStatusEffect statusEffectObject = statusEffect as IStatusEffect;

        if (statusEffectObject == null)
        {
            Debug.Log($"{target.StatusTargetNameGet()}에게 적용할 수 없는 상태이상입니다.");
            return;
        }

        StatusEffect existingEffect = StatusEffectFind(statusEffect.statusType);
        StatusEffectObjectApply(statusEffect, statusEffectObject, existingEffect, target);
    }

    // 피해를 주는 상태이상을 업데이트한다. 예: Poison, Burn, LifeDrain
    public void DamageStatusEffectsUpdate(IStatusEffectTarget target)
    {
        StatusEffectsUpdate(StatusUpdateTiming.Damage, target);
    }

    // 스턴과 저주 상태를 업데이트한다.
    public void StunAndCurseStatusUpdate(IStatusEffectTarget target)
    {
        StatusEffectsUpdate(StatusUpdateTiming.StunAndCurse, target);
    }

    // 강화 상태를 업데이트한다.
    public void UpgradeStatusUpdate(IStatusEffectTarget target)
    {
        StatusEffectsUpdate(StatusUpdateTiming.Upgrade, target);
    }

    public void StatusEffectsUpdate(StatusUpdateTiming updateTiming, IStatusEffectTarget target)
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        foreach (StatusEffect effect in activeStatusEffects)
        {
            if (StatusEffectCanUpdate(effect, updateTiming) == false)
            {
                continue;
            }

            StatusEffectTurnUpdate(effect, target, expiredEffects);
        }

        ExpiredStatusEffectsRemove(expiredEffects, target);
    }

    public void StatusEffectRemove(StatusType statusType, IStatusEffectTarget target)
    {
        StatusEffect effect = StatusEffectFind(statusType);

        if (effect == null)
        {
            return;
        }

        ExpiredStatusEffectsRemove(new List<StatusEffect> { effect }, target);
    }

    private void StatusEffectObjectApply(StatusEffect statusEffect, IStatusEffect statusEffectObject, StatusEffect existingEffect, IStatusEffectTarget target)
    {
        if (statusEffectObject.ExistingStatusEffectApply(existingEffect, target))
        {
            return;
        }

        if (statusEffectObject.SameStatusCanApply(existingEffect) == false)
        {
            Debug.Log(statusEffectObject.AlreadyAppliedMessageGet(target.StatusTargetNameGet()));
            return;
        }

        statusEffectObject.StatusApply(target);

        if (statusEffectObject.RemainingTurnGet() > 0)
        {
            activeStatusEffects.Add(statusEffect);
        }
    }

    private StatusEffect StatusEffectFind(StatusType statusType)
    {
        foreach (StatusEffect effect in activeStatusEffects)
        {
            if (effect != null && effect.statusType == statusType)
            {
                return effect;
            }
        }

        return null;
    }

    private bool StatusEffectCanUpdate(StatusEffect effect, StatusUpdateTiming updateTiming)
    {
        if (effect == null)
        {
            return false;
        }

        if (effect.effectTime <= 0)
        {
            return false;
        }

        IStatusEffect statusEffect = effect as IStatusEffect;

        if (statusEffect == null)
        {
            return false;
        }

        return statusEffect.StatusTurnCanUpdate(updateTiming);
    }

    private void StatusEffectTurnUpdate(StatusEffect effect, IStatusEffectTarget target, List<StatusEffect> expiredEffects)
    {
        IStatusEffect statusEffect = effect as IStatusEffect;

        if (statusEffect == null)
        {
            return;
        }

        statusEffect.StatusTurnUpdate(target);

        if (statusEffect.RemainingTurnGet() <= 0)
        {
            expiredEffects.Add(effect);
        }
    }

    private void ExpiredStatusEffectsRemove(List<StatusEffect> expiredEffects, IStatusEffectTarget target)
    {
        foreach (StatusEffect expired in expiredEffects)
        {
            if (expired == null)
            {
                continue;
            }

            StatusEffectExpire(expired, target);
            target.StatusChangedNotify();
        }
    }

    private void StatusEffectExpire(StatusEffect expired, IStatusEffectTarget target)
    {
        IStatusEffect statusEffect = expired as IStatusEffect;

        activeStatusEffects.Remove(expired);

        if (statusEffect != null)
        {
            statusEffect.StatusExpire(target);
            return;
        }

        Debug.Log($"{target.StatusTargetNameGet()}의 {expired.statusType} 상태이상이 종료되었습니다.");
    }
}
