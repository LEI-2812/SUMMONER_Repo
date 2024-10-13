using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    private StatusType statusType = StatusType.None;
    private double Damage;
    private int cooltime;
    private int currentCooldown;
    public AttackAllEnemiesStrategy(StatusType statusType, double damage, int cooltime)
    {
        this.statusType = statusType;
        Damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates,int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon enemySummon = plate.getSummon();
            if (enemySummon != null)
            {
                Debug.Log($"{attacker.getSummonName()}이(가) {enemySummon.getSummonName()}을(를) 전체 공격합니다.");
                enemySummon.takeDamage(attacker.getSpecialAttackStrategy()[SpecialAttackArrayIndex].getSpecialDamage());
            }
        }
    }

    public double getSpecialDamage()
    {
        return Damage;
    }
    public StatusType getStatusType()
    {
        return statusType;
    }
    public int getCooltime()
    {
        return cooltime;
    }

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
