using UnityEngine;

// 역할: HealStatus의 책임을 정의한다.
public class HealStatus : Status
{
    public HealStatus(double healAmount)
        : base(StatusType.Heal, 0, healAmount)
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
        return $"{targetName}은 이미 회복 효과를 받았습니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        target.HealReceive(damagePerTurn);
        Debug.Log($"{target.GetStatusTargetName()}이 {damagePerTurn}만큼 회복했습니다.");
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
    }

    public override void StatusExpire(IStatusTarget target)
    {
    }
}
