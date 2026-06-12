using UnityEngine;

public class LifeDrainStatusEffect : StatusEffect, IStatusEffect
{
    public LifeDrainStatusEffect(int effectTime, double damagePerTurn, IStatusEffectTarget attacker)
        : base(StatusType.LifeDrain, effectTime, damagePerTurn, attacker)
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

    public string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}은 이미 흡혈 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.StatusHitColorShow();
        LifeDrainApply(target);
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.GetStatusTargetName()}에게 흡혈 상태이상이 적용되었습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
        if (effectTime <= 0)
        {
            return;
        }

        LifeDrainApply(target);
        effectTime--;
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        Debug.Log($"{target.GetStatusTargetName()}의 흡혈 상태이상이 종료되었습니다.");
    }

    private void LifeDrainApply(IStatusEffectTarget target)
    {
        IStatusEffectTarget attacker = GetAttacker();

        if (target == null || attacker == null)
        {
            Debug.Log("타겟이나 공격자가 없어 흡혈이 적용되지 않습니다.");
            return;
        }

        target.DamageTake(damagePerTurn);
        attacker.HealReceive(damagePerTurn);
        Debug.Log($"{target.GetStatusTargetName()}에게서 {damagePerTurn} 만큼 흡혈합니다. 현재 체력: {attacker.GetHealth()}");
    }
}
