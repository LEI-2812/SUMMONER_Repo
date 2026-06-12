using System;
using UnityEngine;

[Serializable]
public class SummonAttackData
{
    [SerializeField] private SummonAttackStrategyType strategyType;
    [SerializeField] private StatusType statusType;
    [SerializeField] private double damage;
    [SerializeField] private int cooldownDuration;
    [SerializeField] private int statusTime;

    public SummonAttackData(
        SummonAttackStrategyType strategyType,
        StatusType statusType,
        double damage,
        int cooldownDuration,
        int statusTime = 0)
    {
        this.strategyType = strategyType;
        this.statusType = statusType;
        this.damage = damage;
        this.cooldownDuration = cooldownDuration;
        this.statusTime = statusTime;
    }

    public SummonAttackStrategyType GetStrategyType() => strategyType;
    public StatusType GetStatusType() => statusType;
    public double GetDamage() => damage;
    public int GetCooltime() => cooldownDuration;
    public int GetStatusTime() => statusTime;

    public IAttackStrategy CreateAttackStrategy()
    {
        switch (strategyType)
        {
            case SummonAttackStrategyType.ClosestEnemy:
                return new ClosestEnemyAttackStrategy(statusType, damage, cooldownDuration, statusTime);
            case SummonAttackStrategyType.Targeted:
                return new TargetedAttackStrategy(statusType, damage, cooldownDuration, statusTime);
            case SummonAttackStrategyType.AllEnemies:
                return new AttackAllEnemiesStrategy(statusType, damage, cooldownDuration, statusTime);
            default:
                throw new ArgumentOutOfRangeException(nameof(strategyType), strategyType, "Unknown summon attack strategy type.");
        }
    }
}
