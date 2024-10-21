using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    private StatusType StatusType;
    private double damage;
    private int cooltime;
    private int currentCooldown;
    private int statusTime;
    public ClosestEnemyAttackStrategy(StatusType statusType,double damage, int cooltime, int statusTime=0)
    {
        this.StatusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
        this.statusTime = statusTime;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int SpecialAttackarrayIndex)
    {
        Summon closestEnemySummon = GetClosestEnemySummon(targetPlates);

        if (closestEnemySummon != null)
        {
            Debug.Log($"{attacker.getSummonName()}이(가) {closestEnemySummon.getSummonName()}을(를) 공격합니다.");
            closestEnemySummon.takeDamage(attacker.getAttackPower());
        }
        else
        {
            Debug.Log("공격할 적이 없습니다.");
        }
    }

    private Summon GetClosestEnemySummon(List<Plate> targetPlates)
    {
        for (int i = 0; i < targetPlates.Count; i++)
        {
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
    public StatusType getStatusType() { return StatusType; }
    public void setStatusType(StatusType type) { StatusType = type; }
    
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
