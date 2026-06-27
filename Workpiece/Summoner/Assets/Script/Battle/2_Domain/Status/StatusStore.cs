using System.Collections.Generic;
using UnityEngine;

// 역할: StatusStore의 책임을 정의한다.
public class StatusStore
{
    private List<StatusData> activeStatuses;

    public StatusStore()
        : this(new List<StatusData>())
    {
    }

    public StatusStore(List<StatusData> activeStatuses)
    {
        this.activeStatuses = activeStatuses;
    }

    public IReadOnlyList<StatusData> GetActiveStatuses()
    {
        return activeStatuses;
    }

    public List<StatusType> GetTypes()
    {
        List<StatusType> statusTypes = new List<StatusType>();

        foreach (StatusData effect in activeStatuses)
        {
            if (effect != null)
            {
                statusTypes.Add(effect.statusType);
            }
        }

        return statusTypes;
    }

    public bool Contains(StatusType statusType)
    {
        return StatusDataFind(statusType) != null;
    }

    public void Apply(StatusData statusEffect, IStatusTarget target)
    {
        if (statusEffect == null)
        {
            Debug.Log($"{target.GetStatusTargetName()}에게 적용할 상태이상이 없습니다.");
            return;
        }

        Status statusEffectObject = statusEffect as Status;

        if (statusEffectObject == null)
        {
            Debug.Log($"{target.GetStatusTargetName()}에게 적용할 수 없는 상태이상입니다.");
            return;
        }

        StatusData existingEffect = StatusDataFind(statusEffect.statusType);
        StatusDataObjectApply(statusEffect, statusEffectObject, existingEffect, target);
    }

    public void Update(StatusTiming updateTiming, IStatusTarget target)
    {
        List<StatusData> expiredEffects = new List<StatusData>();

        foreach (StatusData effect in activeStatuses)
        {
            if (StatusDataCanUpdate(effect, updateTiming) == false)
            {
                continue;
            }

            StatusDataTurnUpdate(effect, target, expiredEffects);
        }

        ExpiredStatusDatasRemove(expiredEffects, target);
    }

    public void Remove(StatusType statusType, IStatusTarget target)
    {
        StatusData effect = StatusDataFind(statusType);

        if (effect == null)
        {
            return;
        }

        ExpiredStatusDatasRemove(new List<StatusData> { effect }, target);
    }

    private void StatusDataObjectApply(StatusData statusEffect, Status statusEffectObject, StatusData existingEffect, IStatusTarget target)
    {
        if (statusEffectObject.ExistingApply(existingEffect, target))
        {
            return;
        }

        if (statusEffectObject.SameStatusCanApply(existingEffect) == false)
        {
            Debug.Log(statusEffectObject.GetAlreadyAppliedMessage(target.GetStatusTargetName()));
            return;
        }

        statusEffectObject.StatusApply(target);

        if (statusEffectObject.GetRemainingTurn() > 0)
        {
            activeStatuses.Add(statusEffect);
        }
    }

    private StatusData StatusDataFind(StatusType statusType)
    {
        foreach (StatusData effect in activeStatuses)
        {
            if (effect != null && effect.statusType == statusType)
            {
                return effect;
            }
        }

        return null;
    }

    private bool StatusDataCanUpdate(StatusData effect, StatusTiming updateTiming)
    {
        if (effect == null)
        {
            return false;
        }

        if (effect.effectTime <= 0)
        {
            return false;
        }

        Status statusEffect = effect as Status;

        if (statusEffect == null)
        {
            return false;
        }

        return statusEffect.StatusTurnCanUpdate(updateTiming);
    }

    private void StatusDataTurnUpdate(StatusData effect, IStatusTarget target, List<StatusData> expiredEffects)
    {
        Status statusEffect = effect as Status;

        if (statusEffect == null)
        {
            return;
        }

        statusEffect.StatusTurnUpdate(target);

        if (statusEffect.GetRemainingTurn() <= 0)
        {
            expiredEffects.Add(effect);
        }
    }

    private void ExpiredStatusDatasRemove(List<StatusData> expiredEffects, IStatusTarget target)
    {
        foreach (StatusData expired in expiredEffects)
        {
            if (expired == null)
            {
                continue;
            }

            StatusDataExpire(expired, target);
            target.StatusChangedNotify();
        }
    }

    private void StatusDataExpire(StatusData expired, IStatusTarget target)
    {
        Status statusEffect = expired as Status;

        activeStatuses.Remove(expired);

        if (statusEffect != null)
        {
            statusEffect.StatusExpire(target);
            return;
        }

            Debug.Log($"{target.GetStatusTargetName()}의 {expired.statusType} 상태이상이 종료되었습니다.");
    }
}
