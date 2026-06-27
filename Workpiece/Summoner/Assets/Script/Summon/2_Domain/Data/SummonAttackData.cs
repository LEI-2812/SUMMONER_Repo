using System;
using UnityEngine;

[Serializable]
// 역할: SummonAttackData의 책임을 정의한다.
public class SummonAttackData
{
    [SerializeField] private SummonAttackStrategyType strategyType;
    [SerializeField] private StatusType statusType;
    [SerializeField] private double damage;
    [SerializeField] private int cooldownDuration;
    [SerializeField] private int statusTime;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite displaySprite;
    [SerializeField] private string tooltipText;

    public SummonAttackData(
        SummonAttackStrategyType strategyType,
        StatusType statusType,
        double damage,
        int cooldownDuration,
        int statusTime = 0,
        string displayName = "",
        Sprite displaySprite = null,
        string tooltipText = "")
    {
        this.strategyType = strategyType;
        this.statusType = statusType;
        this.damage = damage;
        this.cooldownDuration = cooldownDuration;
        this.statusTime = statusTime;
        this.displayName = displayName;
        this.displaySprite = displaySprite;
        this.tooltipText = tooltipText;
    }

    public SummonAttackStrategyType GetStrategyType() => strategyType;
    public StatusType GetStatusType() => statusType;
    public double GetDamage() => damage;
    public int GetCooltime() => cooldownDuration;
    public int GetStatusTime() => statusTime;
    public Sprite GetDisplaySprite() => displaySprite;

    public string GetDisplayName()
    {
        return string.IsNullOrWhiteSpace(displayName) ? CreateDefaultDisplayName() : displayName;
    }

    public string GetTooltipText()
    {
        if (!string.IsNullOrWhiteSpace(tooltipText))
        {
            return tooltipText;
        }

        return GetDisplayName()
            + "\nDamage: " + damage
            + "\nCooldown: " + cooldownDuration
            + "\nStatus: " + statusType;
    }

    public AttackData CreateAttackStrategy()
    {
        switch (strategyType)
        {
            case SummonAttackStrategyType.ClosestEnemy:
                return new AttackData(new ClosestEnemyAttackStrategy(), statusType, damage, cooldownDuration, statusTime);
            case SummonAttackStrategyType.Targeted:
                return new AttackData(new TargetedAttackStrategy(), statusType, damage, cooldownDuration, statusTime);
            case SummonAttackStrategyType.AllEnemies:
                return new AttackData(new AttackAllEnemiesStrategy(), statusType, damage, cooldownDuration, statusTime);
            default:
                throw new ArgumentOutOfRangeException(nameof(strategyType), strategyType, "Unknown summon attack strategy type.");
        }
    }

    private string CreateDefaultDisplayName()
    {
        if (statusType != StatusType.None)
        {
            return statusType.ToString();
        }

        return strategyType.ToString();
    }
}
