using UnityEngine;

// 역할: UpgradeStatus의 책임을 정의한다.
public class UpgradeStatus : Status
{
    public UpgradeStatus(int effectTime, double upgradeRate)
        : base(StatusType.Upgrade, effectTime, upgradeRate)
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
        return updateTiming == StatusTiming.Upgrade;
    }

    public override string GetAlreadyAppliedMessage(string targetName)
    {
        return $"{targetName}은 이미 강화 상태입니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        SetOriginAttack(target.GetAttackPower());

        if (ShouldApplyOnce())
        {
            target.AttackPowerUpgrade(damagePerTurn);
            SetApplyOnce();
            target.StatusChangedNotify();
            target.BuffSoundPlay();
        }

        Debug.Log($"{target.GetStatusTargetName()}의 공격력이 강화되었습니다.");
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
        effectTime--;
    }

    public override void StatusExpire(IStatusTarget target)
    {
        Debug.Log("공격력 복구");
        target.AttackPowerRestore(GetOriginAttack());
        Debug.Log($"{target.GetStatusTargetName()}의 강화 상태이상이 종료되었습니다.");
    }
}
