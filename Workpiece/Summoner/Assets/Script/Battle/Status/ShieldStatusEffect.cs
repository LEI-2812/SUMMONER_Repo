using UnityEngine;

public class ShieldStatusEffect : StatusEffect, IStatusEffect
{
    public ShieldStatusEffect(int effectTime, double shieldAmount)
        : base(StatusType.Shield, effectTime, shieldAmount)
    {
    }

    public StatusType GetStatusType()
    {
        return statusType;
    }

    public int GetRemainingTurn()
    {
        return effectTime;
    }

    public bool SameStatusCanApply(StatusEffect existingEffect)
    {
        return true;
    }

    public bool ExistingStatusEffectApply(StatusEffect existingEffect, IStatusEffectTarget target)
    {
        if (existingEffect == null)
        {
            return false;
        }

        target.SetShield(existingEffect.damagePerTurn);
        target.BuffSoundPlay();
        Debug.Log($"{target.GetStatusTargetName()}의 보호막을 다시 채웠습니다.");
        return true;
    }

    public bool StatusTurnCanUpdate(StatusUpdateTiming updateTiming)
    {
        return false;
    }

    public string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}에게 이미 보호막이 있습니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.ShieldAdd(damagePerTurn);
        target.BuffSoundPlay();
        Debug.Log($"{target.GetStatusTargetName()}에게 보호막이 생겼습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        Debug.Log($"{target.GetStatusTargetName()}의 보호막 상태이상이 종료되었습니다.");
    }
}
