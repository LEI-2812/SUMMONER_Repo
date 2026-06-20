using UnityEngine;

// 역할: 스턴 상태의 공격 불가 적용과 만료 복구를 담당한다.
public class StunStatusEffect : StatusEffect, IStatusEffect
{
    public StunStatusEffect(int effectTime)
        : base(StatusType.Stun, effectTime)
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
        return updateTiming == StatusUpdateTiming.StunAndCurse;
    }

    public string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}은 이미 스턴 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        target.StatusHitColorShow();
        target.SetAttackAvailable(false);
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.GetStatusTargetName()}이 스턴 상태가 되었습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
        target.SetAttackAvailable(false);
        Debug.Log($"{target.GetStatusTargetName()}은 스턴 상태로 공격할 수 없습니다.");
        effectTime--;
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        target.SetAttackAvailable(true);
        Debug.Log($"{target.GetStatusTargetName()}의 스턴이 해제되었습니다. 공격 가능");
    }
}
