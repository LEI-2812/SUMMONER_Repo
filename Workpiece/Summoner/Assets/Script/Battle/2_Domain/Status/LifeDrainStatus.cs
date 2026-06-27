using UnityEngine;

// 역할: LifeDrainStatus의 책임을 정의한다.
public class LifeDrainStatus : Status
{
    public LifeDrainStatus(int effectTime, double damagePerTurn, IStatusTarget attacker)
        : base(StatusType.LifeDrain, effectTime, damagePerTurn, attacker)
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
        return $"{targetName}은 이미 흡혈 상태입니다.";
    }

    public override void StatusApply(IStatusTarget target)
    {
        target.StatusHitColorShow();
        LifeDrainApply(target);
        effectTime--;
        target.StatusChangedNotify();
        target.DebuffSoundPlay();

        Debug.Log($"{target.GetStatusTargetName()}에게 흡혈 상태이상이 적용되었습니다.");
    }

    public override void StatusTurnUpdate(IStatusTarget target)
    {
        if (effectTime <= 0)
        {
            return;
        }

        LifeDrainApply(target);
        effectTime--;
    }

    public override void StatusExpire(IStatusTarget target)
    {
        Debug.Log($"{target.GetStatusTargetName()}의 흡혈 상태이상이 종료되었습니다.");
    }

    private void LifeDrainApply(IStatusTarget target)
    {
        IStatusTarget attacker = GetAttacker();

        if (target == null || attacker == null)
        {
            Debug.Log("대상 또는 공격자가 없어 흡혈을 적용할 수 없습니다.");
            return;
        }

        target.DamageTake(damagePerTurn);
        attacker.HealReceive(damagePerTurn);
        Debug.Log($"{target.GetStatusTargetName()}에게 {damagePerTurn}만큼 흡혈했습니다. 현재 체력: {attacker.GetHealth()}");
    }
}
