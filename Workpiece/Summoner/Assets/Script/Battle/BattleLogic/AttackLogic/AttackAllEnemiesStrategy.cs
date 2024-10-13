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
                Debug.Log($"{attacker.getSummonName()}��(��) {enemySummon.getSummonName()}��(��) ��ü �����մϴ�.");
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

    // ��Ÿ���� �ʱ�ȭ (��ų ��� �� ����)
    public void ApplyCooldown() => currentCooldown = cooltime;

    // �� ���� �� ��Ÿ�� ����
    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}
