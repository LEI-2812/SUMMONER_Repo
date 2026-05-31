using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    private StatusType statusType;
    private double damage;
    private int cooltime;
    private int currentCooldown;
    private IAttackEffect attackEffect;

    public ClosestEnemyAttackStrategy(StatusType statusType,double damage, int cooltime, int statusTime=0)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
        this.attackEffect = ClosestEnemyAttackEffectInstanceCreate.Create(statusType);
    }

    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int SpecialAttackarrayIndex)
    {
        if (targetPlates == null)
        {
            Debug.LogWarning("Target plates are missing.");
            return;
        }

        Summon closestEnemySummon = GetClosestEnemySummon(targetPlates);

        if (closestEnemySummon != null)
        {
            attackEffect.AttackEffectApply(attacker, closestEnemySummon, SpecialAttackarrayIndex);
        }
        else
        {
            Debug.Log("공격할 적이 없습니다.");
        }
    }

    private Summon GetClosestEnemySummon(List<Plate> targetPlates)
    {
        if (targetPlates == null)
        {
            return null;
        }

        for (int i = 0; i < targetPlates.Count; i++)
        {
            if (targetPlates[i] == null)
            {
                continue;
            }

            Summon enemySummon = targetPlates[i].getCurrentSummon();
            if (enemySummon != null)
            {
                return enemySummon; // 첫 번째로 존재하는 소환수를 바로 반환
            }
        }

        return null; // 적 소환수가 없으면 null 반환
    }

    public double getSpecialDamage()
    {
        return damage;
    }

    public bool BenefitEffectCheck() => attackEffect.BenefitEffectCheck();

    public StatusType getStatusType() { return statusType; }
    public void setStatusType(StatusType type) { statusType = type; }
    
    public int getCooltime() { return cooltime; }

    public int getCurrentCooldown() => currentCooldown;

    // 쿨타임을 초기화 (스킬 사용 후 적용)
    public void ApplyCooldown() => currentCooldown = cooltime;

    // 턴 종료 시 쿨타임 감소
    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}
