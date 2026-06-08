using System.Collections.Generic;
using UnityEngine;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    private StatusType statusType = StatusType.None;
    private double Damage;
    private int cooltime;
    private int currentCooldown;
    private int statusTime; // 지속시간
    private IAttackEffect attackEffect;

    public AttackAllEnemiesStrategy(StatusType statusType, double damage, int cooltime, int statusTime=0)
    {
        this.statusType = statusType;
        Damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
        this.statusTime = statusTime;
        this.attackEffect = AllEnemiesAttackEffectInstanceCreate.Create(statusType, statusTime);
    }

    public void Attack(Summon attacker, List<Plate> targetPlates,int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (targetPlates == null)
        {
            Debug.LogWarning("Target plates are missing.");
            return;
        }

        foreach (var plate in targetPlates)
        {
            if (plate == null)
            {
                continue;
            }

            Summon target = plate.GetCurrentSummon();
            if (target != null)
            {
                attackEffect.AttackEffectApply(attacker, target, SpecialAttackArrayIndex);
            }
        }
    }

    public bool IsBenefitEffect(AttackAllEnemiesStrategy strategy)
    {
        return BenefitEffectCheck();
    }

    public bool BenefitEffectCheck() => attackEffect.BenefitEffectCheck();

    public double GetSpecialDamage()
    {
        return Damage;
    }

    public StatusType GetStatusType()
    {
        return statusType;
    }

    public int GetCooltime()
    {
        return cooltime;
    }

    public int GetCurrentCooldown() => currentCooldown;

    // 쿨타임을 초기화한다. (스킬 사용 후 적용)
    public void ApplyCooldown() => currentCooldown = cooltime;

    // 턴 종료 후 쿨타임 감소
    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}
