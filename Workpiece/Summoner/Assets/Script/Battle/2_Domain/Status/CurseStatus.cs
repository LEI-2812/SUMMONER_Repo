using UnityEngine;

// 역할: CurseStatus의 책임을 정의한다.
public class CurseStatus : Status
{
    public CurseStatus(int effectTime, double curseRate)
        : base(StatusType.Curse, effectTime, curseRate)
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
        return $"{targetName}은 이미 저주 상태입니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        target.StatusHitColorShow();
        SetOriginAttack(target.GetAttackPower());

        if (ShouldApplyOnce())
        {
            target.AttackPowerCurse(damagePerTurn);
            SetApplyOnce();
            target.StatusChangedNotify();
        }

        target.DebuffSoundPlay();
        Debug.Log($"{target.GetStatusTargetName()}에게 저주 상태이상이 적용되었습니다.");
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
        effectTime--;
    }

    public override void StatusExpire(IStatusTarget target)
    {
        target.AttackPowerRestore(GetOriginAttack());
        Debug.Log($"{target.GetStatusTargetName()}의 저주 상태이상이 종료되었습니다.");
    }
}
