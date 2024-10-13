using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    private StatusType StatusType;
    private double damage;
    private int cooltime;
    private int currentCooldown;
    public ClosestEnemyAttackStrategy(StatusType statusType,double damage, int cooltime)
    {
        this.StatusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
    }
    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackarrayIndex)
    {
        Summon closestEnemySummon = GetClosestEnemySummon(selectedPlateIndex, enemyPlates);

        if (closestEnemySummon != null)
        {
            Debug.Log($"{attacker.getSummonName()}��(��) {closestEnemySummon.getSummonName()}��(��) �����մϴ�.");
            closestEnemySummon.takeDamage(attacker.getAttackPower());
        }
        else
        {
            Debug.Log("������ ���� �����ϴ�.");
        }
    }

    private Summon GetClosestEnemySummon(int playerPlateIndex, List<Plate> enemyPlates)
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].getSummon();
            if (enemySummon != null)
            {
                return enemySummon; // ù ��°�� �����ϴ� ��ȯ���� �ٷ� ��ȯ
            }
        }

        return null; // �� ��ȯ���� ������ null ��ȯ
    }

    public double getSpecialDamage()
    {
        return damage;
    }
    public StatusType getStatusType() { return StatusType; }
    public void setStatusType(StatusType type) { StatusType = type; }
    
    public int getCooltime() { return cooltime; }

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
