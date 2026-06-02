using System;
using UnityEngine;

[Serializable]
public class SummonAttackData
{
    [SerializeField] private SummonAttackStrategyType strategyType;
    [SerializeField] private StatusType statusType;
    [SerializeField] private double damage;
    [SerializeField] private int cooltime;
    [SerializeField] private int statusTime;

    public SummonAttackData(
        SummonAttackStrategyType strategyType,
        StatusType statusType,
        double damage,
        int cooltime,
        int statusTime = 0)
    {
        this.strategyType = strategyType;
        this.statusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.statusTime = statusTime;
    }

    public SummonAttackStrategyType StrategyTypeGet() => strategyType;
    public StatusType StatusTypeGet() => statusType;
    public double DamageGet() => damage;
    public int CooltimeGet() => cooltime;
    public int StatusTimeGet() => statusTime;

    public IAttackStrategy AttackStrategyCreate()
    {
        switch (strategyType)
        {
            case SummonAttackStrategyType.ClosestEnemy:
                return new ClosestEnemyAttackStrategy(statusType, damage, cooltime, statusTime);
            case SummonAttackStrategyType.Targeted:
                return new TargetedAttackStrategy(statusType, damage, cooltime, statusTime);
            case SummonAttackStrategyType.AllEnemies:
                return new AttackAllEnemiesStrategy(statusType, damage, cooltime, statusTime);
            default:
                throw new ArgumentOutOfRangeException(nameof(strategyType), strategyType, "Unknown summon attack strategy type.");
        }
    }
}
