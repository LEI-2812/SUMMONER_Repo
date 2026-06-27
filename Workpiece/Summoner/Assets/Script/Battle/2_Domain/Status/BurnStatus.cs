using UnityEngine;

// 역할: BurnStatus의 책임을 정의한다.
public class BurnStatus : Status
{
    public BurnStatus(int effectTime, double damagePerTurn)
        : base(StatusType.Burn, effectTime, damagePerTurn)
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
        return existingEffect == null;
    }

    public override bool ExistingApply(StatusData existingEffect, IStatusTarget target)
    {
        return false;
    }

    public override bool StatusTurnCanUpdate(StatusTiming updateTiming)
    {
        return updateTiming == StatusTiming.Damage;
    }

    public override string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}은 이미 화상 상태입니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        target.StatusHitColorShow();
        target.DamageTake(damagePerTurn);
        effectTime--;
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.GetStatusTargetName()}에게 화상 상태이상이 적용되었습니다.");
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
        if (effectTime <= 0)
        {
            return;
        }

        target.DamageTake(damagePerTurn);
        effectTime--;
    }

    public override void StatusExpire(IStatusTarget target)
    {
        Debug.Log($"{target.GetStatusTargetName()}의 화상 상태이상이 종료되었습니다.");
    }
}
