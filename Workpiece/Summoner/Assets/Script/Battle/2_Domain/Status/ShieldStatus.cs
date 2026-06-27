using UnityEngine;

// 역할: ShieldStatus의 책임을 정의한다.
public class ShieldStatus : Status
{
    public ShieldStatus(int effectTime, double shieldAmount)
        : base(StatusType.Shield, effectTime, shieldAmount)
    {
    }

    public override StatusType GetStatusType()
    {
        return statusType;
    }

    public override int GetRemainingTurn()
    {
        return effectTime;
    }

    public override bool SameStatusCanApply(StatusData existingEffect)
    {
        return true;
    }

    public override bool ExistingApply(StatusData existingEffect, IStatusTarget target)
    {
        if (existingEffect == null)
        {
            return false;
        }

        existingEffect.damagePerTurn = damagePerTurn;
        existingEffect.effectTime = effectTime;
        target.SetShield(damagePerTurn);
        target.BuffSoundPlay();
        Debug.Log($"{target.GetStatusTargetName()}의 보호막을 다시 채웠습니다.");
        return true;
    }

    public override bool StatusTurnCanUpdate(StatusTiming updateTiming)
    {
        return false;
    }

    public override string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}에게 이미 보호막이 있습니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        target.ShieldAdd(damagePerTurn);
        target.BuffSoundPlay();
        Debug.Log($"{target.GetStatusTargetName()}에게 보호막이 생겼습니다.");
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
    }

    public override void StatusExpire(IStatusTarget target)
    {
        Debug.Log($"{target.GetStatusTargetName()}의 보호막 상태이상이 종료되었습니다.");
    }
}
