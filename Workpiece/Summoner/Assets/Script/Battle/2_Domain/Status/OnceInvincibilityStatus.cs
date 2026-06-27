// 역할: OnceInvincibilityStatus의 책임을 정의한다.
public class OnceInvincibilityStatus : Status
{
    public OnceInvincibilityStatus()
        : base(StatusType.OnceInvincibility, 0)
    {
    }

    public override StatusType GetStatusType()
    {
        return statusType;
    }

    public override int GetRemainingTurn()
    {
        return 0;
    }

    public override bool SameStatusCanApply(StatusData existingEffect)
    {
        return true;
    }

    public override bool ExistingApply(StatusData existingEffect, IStatusTarget target)
    {
        return false;
    }

    public override bool StatusTurnCanUpdate(StatusTiming updateTiming)
    {
        return false;
    }

    public override string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}은 이미 1회 무적 상태입니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        target.SetOnceInvincibility(true);
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
    }

    public override void StatusExpire(IStatusTarget target)
    {
    }
}
