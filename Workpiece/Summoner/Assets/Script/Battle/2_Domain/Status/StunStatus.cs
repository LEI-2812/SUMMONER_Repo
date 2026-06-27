using UnityEngine;

// 역할: StunStatus의 책임을 정의한다.
public class StunStatus : Status
{
    public StunStatus(int effectTime)
        : base(StatusType.Stun, effectTime)
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
        return updateTiming == StatusTiming.StunAndCurse;
    }

    public override string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}은 이미 기절 상태입니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        target.StatusHitColorShow();
        target.SetAttackAvailable(false);
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.GetStatusTargetName()}이 기절 상태가 되었습니다.");
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
        target.SetAttackAvailable(false);
        Debug.Log($"{target.GetStatusTargetName()}은 기절 상태라 공격할 수 없습니다.");
        effectTime--;
    }

    public override void StatusExpire(IStatusTarget target)
    {
        target.SetAttackAvailable(true);
    }
}
