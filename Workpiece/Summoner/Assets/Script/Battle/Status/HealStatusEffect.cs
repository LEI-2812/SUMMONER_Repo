using UnityEngine;

public class HealStatusEffect : StatusEffect, IStatusEffect
{
    public HealStatusEffect(double healAmount)
        : base(StatusType.Heal, 0, healAmount)
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
        return $"{targetName}은 이미 회복 효과를 받았습니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.HealReceive(damagePerTurn);
        Debug.Log($"{target.GetStatusTargetName()}이 {damagePerTurn}만큼 회복했습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
    }
}
