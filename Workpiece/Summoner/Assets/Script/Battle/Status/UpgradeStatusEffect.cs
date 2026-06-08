using UnityEngine;

public class UpgradeStatusEffect : StatusEffect, IStatusEffect
{
    public UpgradeStatusEffect(int effectTime, double upgradeRate)
        : base(StatusType.Upgrade, effectTime, upgradeRate)
    {
    }

    public StatusType StatusTypeGet()
    {
        return statusType;
    }

    public int RemainingTurnGet()
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
        return updateTiming == StatusUpdateTiming.Upgrade;
    }

    public string AlreadyAppliedMessageGet(string targetName)
    {
        return $"{targetName}은 이미 강화 상태입니다.";
    }

    public void StatusApply(IStatusEffectTarget target)
    {
        SetOriginAttack(target.AttackPowerGet());

        if (ShouldApplyOnce())
        {
            target.AttackPowerUpgrade(damagePerTurn);
            SetApplyOnce();
            target.StatusChangedNotify();
            target.BuffSoundPlay();
        }

        Debug.Log($"{target.StatusTargetNameGet()}의 공격력이 강화되었습니다.");
    }

    public void StatusTurnUpdate(IStatusEffectTarget target)
    {
        effectTime--;
    }

    public void StatusExpire(IStatusEffectTarget target)
    {
        Debug.Log("공격력 복구");
        target.AttackPowerRestore(GetOriginAttack());
        Debug.Log($"{target.StatusTargetNameGet()}의 강화 상태이상이 종료되었습니다.");
    }
}
