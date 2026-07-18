using System.Collections.Generic;

// 역할: SummonEntity의 책임을 정의한다.
public class SummonEntity
{
    public string Name { get; private set; }
    public SummonRank Rank { get; private set; }
    public double MaxHp { get; private set; }
    public double CurrentHp { get; private set; }
    public double AttackPower { get; private set; }
    public double BaseAttackPower { get; private set; }
    public double HeavyAttackPower { get; private set; }
    public double Shield { get; private set; }
    public double InitialShield { get; private set; }
    public bool CanAttack { get; private set; } = true;
    public bool HasOnceInvincibility { get; private set; }
    public AttackData NormalAttack { get; private set; }
    public AttackData[] SpecialAttacks { get; private set; }
    private ActiveStatusList activeStatusList = new ActiveStatusList();

    public void SetBattleStats(
        string name,
        SummonRank rank,
        double maxHp,
        double attackPower,
        double heavyAttackPower)
    {
        Name = name;
        Rank = rank;
        MaxHp = maxHp;
        CurrentHp = maxHp;
        AttackPower = attackPower;
        BaseAttackPower = attackPower;
        HeavyAttackPower = heavyAttackPower;
    }

    public void SetAttackStrategies(AttackData normalAttack, params AttackData[] specialAttacks)
    {
        NormalAttack = normalAttack;
        SpecialAttacks = specialAttacks;
    }

    public void ScaleStats(double multiplier)
    {
        MaxHp = (int)(MaxHp * multiplier);
        CurrentHp = MaxHp;
        AttackPower = (int)(AttackPower * multiplier);
        BaseAttackPower = AttackPower;
        HeavyAttackPower = (int)(HeavyAttackPower * multiplier);
        ScaleFixedAttackValues(multiplier);
    }

    private void ScaleFixedAttackValues(double multiplier)
    {
        NormalAttack?.ScaleFixedValue(multiplier);

        if (SpecialAttacks == null)
        {
            return;
        }

        foreach (AttackData specialAttack in SpecialAttacks)
        {
            specialAttack?.ScaleFixedValue(multiplier);
        }
    }

    public void SetCurrentHp(double hp)
    {
        CurrentHp = hp;
    }

    public void SetAttackPower(double power)
    {
        AttackPower = power;
    }

    public void Heal(double amount)
    {
        CurrentHp += amount;
        if (CurrentHp > MaxHp)
        {
            CurrentHp = MaxHp;
        }
    }

    public void TakeHealthDamage(double damage)
    {
        CurrentHp -= damage;
    }

    public void TakeDamage(double damage)
    {
        TakeHealthDamage(damage);
    }

    public void AddShield(double amount)
    {
        if (Shield == 0)
        {
            InitialShield = amount;
        }

        Shield += amount;
    }

    public void SetShield(double amount)
    {
        Shield = amount;
    }

    public void ApplyAttackPowerUpgrade(double value)
    {
        AttackPower *= 1 + value;
        AttackPower = (int)System.Math.Floor(AttackPower);
    }

    public void ApplyAttackPowerCurse(double value)
    {
        AttackPower *= 1 - value;
    }

    public void RestoreAttackPower(double value)
    {
        AttackPower = value;
    }

    public void SetAttackAvailable(bool canAttack)
    {
        CanAttack = canAttack;
    }

    public bool IsSpecialAttackCoolingDown(AttackData strategy)
    {
        return strategy != null && strategy.GetCurrentCooldown() > 0;
    }

    public bool CanUseAttack(AttackData strategy)
    {
        return strategy != null && !IsSpecialAttackCoolingDown(strategy);
    }

    public void UpdateSpecialAttackCooldowns(System.Action<string> log)
    {
        if (SpecialAttacks == null)
        {
            return;
        }

        foreach (AttackData specialAttack in SpecialAttacks)
        {
            if (specialAttack == null)
            {
                continue;
            }

            if (IsSpecialAttackCoolingDown(specialAttack))
            {
                specialAttack.ReduceCooldown();
                continue;
            }

            log?.Invoke($"{Name}의 {specialAttack.GetType().Name} 특수 공격 쿨타임이 종료되었습니다.");
        }
    }

    public AttackData[] GetAvailableSpecialAttacks()
    {
        List<AttackData> availableSpecialAttacks = new List<AttackData>();

        if (SpecialAttacks == null)
        {
            return availableSpecialAttacks.ToArray();
        }

        foreach (AttackData specialAttack in SpecialAttacks)
        {
            if (CanUseAttack(specialAttack))
            {
                availableSpecialAttacks.Add(specialAttack);
            }
        }

        return availableSpecialAttacks.ToArray();
    }

    public void ApplyStatus(StatusData status, IStatusTarget target)
    {
        activeStatusList.Apply(status, target);
    }

    public void UpdateStatus(StatusTiming timing, IStatusTarget target)
    {
        activeStatusList.Update(timing, target);
    }

    public void RemoveStatus(StatusType statusType, IStatusTarget target)
    {
        activeStatusList.Remove(statusType, target);
    }

    public List<StatusType> GetStatusTypes()
    {
        return activeStatusList.GetTypes();
    }

    public IReadOnlyList<StatusData> GetActiveStatuses()
    {
        return activeStatusList.GetActiveStatuses();
    }

    public bool ContainsStatus(StatusType statusType)
    {
        return activeStatusList.Contains(statusType);
    }

    public void SetOnceInvincibility(bool hasOnceInvincibility)
    {
        HasOnceInvincibility = hasOnceInvincibility;
    }

    public SummonEntity CloneForPrediction()
    {
        return (SummonEntity)MemberwiseClone();
    }
}
