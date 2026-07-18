using System.Collections.Generic;

// 역할: AttackData의 책임을 정의한다.
public class AttackData
{
    private readonly AttackCooldownState cooldownState;

    public AttackData(
        IAttackStrategy strategy,
        StatusType statusType,
        double effectValue,
        int cooldownDuration,
        int statusTime = 0)
    {
        Strategy = strategy;
        StatusType = statusType;
        EffectValue = effectValue;
        StatusTime = statusTime;
        cooldownState = new AttackCooldownState(cooldownDuration);
    }

    public IAttackStrategy Strategy { get; }
    public StatusType StatusType { get; }
    public double EffectValue { get; private set; }
    public int StatusTime { get; }

    public List<Summon> SelectTargets(AttackTargetInput input)
    {
        return Strategy.SelectTargets(this, input);
    }

    public bool TargetsOwnPlates()
    {
        return StatusType == StatusType.Heal
            || StatusType == StatusType.Shield
            || StatusType == StatusType.Upgrade
            || StatusType == StatusType.OnceInvincibility;
    }

    public bool IsStrategy<T>() where T : IAttackStrategy
    {
        return Strategy is T;
    }

    public StatusType GetStatusType()
    {
        return StatusType;
    }

    public double GetSpecialDamage()
    {
        return EffectValue;
    }

    public void ScaleFixedValue(double multiplier)
    {
        if (StatusType != StatusType.None && StatusType != StatusType.Shield)
        {
            return;
        }

        EffectValue *= multiplier;
    }

    public int GetStatusTime()
    {
        return StatusTime;
    }

    public int GetCooltime()
    {
        return cooldownState.GetCooltime();
    }

    public int GetCurrentCooldown()
    {
        return cooldownState.GetCurrentCooldown();
    }

    public void ApplyCooldown()
    {
        cooldownState.ApplyCooldown();
    }

    public void ReduceCooldown()
    {
        cooldownState.ReduceCooldown();
    }
}
