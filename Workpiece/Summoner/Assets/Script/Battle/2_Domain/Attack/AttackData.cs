using System.Collections.Generic;

// 역할: AttackData의 책임을 정의한다.
public class AttackData
{
    private readonly AttackCooldownStore cooldownStore;

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
        cooldownStore = new AttackCooldownStore(cooldownDuration);
    }

    public IAttackStrategy Strategy { get; }
    public StatusType StatusType { get; }
    public double EffectValue { get; private set; }
    public int StatusTime { get; }

    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        return Strategy.SelectTargets(this, attacker, targetPlates, selectedPlateIndex);
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
        return cooldownStore.GetCooltime();
    }

    public int GetCurrentCooldown()
    {
        return cooldownStore.GetCurrentCooldown();
    }

    public void ApplyCooldown()
    {
        cooldownStore.ApplyCooldown();
    }

    public void ReduceCooldown()
    {
        cooldownStore.ReduceCooldown();
    }
}
