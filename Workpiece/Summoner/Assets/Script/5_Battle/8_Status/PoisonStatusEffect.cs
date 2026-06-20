using UnityEngine;

// 역할: 독 상태의 적용, 턴 피해, 만료 처리를 담당한다.
public class PoisonStatusEffect : StatusEffect, IStatusEffect
{
    public PoisonStatusEffect(int effectTime, double damagePerTurn)
        : base(StatusType.Poison, effectTime, damagePerTurn)
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
        return $"{targetName}은 이미 중독 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.StatusHitColorShow();
        target.DamageTake(damagePerTurn);
        effectTime--;
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.GetStatusTargetName()}에게 중독 상태이상이 적용되었습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
        if (effectTime <= 0)
        {
            return;
        }

        Debug.Log($"{target.GetStatusTargetName()}이 중독 상태로 인해 {damagePerTurn} 피해를 입습니다. 남은 상태이상 시간: {effectTime}턴");
        target.DamageTake(damagePerTurn);
        effectTime--;
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        Debug.Log($"{target.GetStatusTargetName()}의 중독 상태이상이 종료되었습니다.");
    }
}
