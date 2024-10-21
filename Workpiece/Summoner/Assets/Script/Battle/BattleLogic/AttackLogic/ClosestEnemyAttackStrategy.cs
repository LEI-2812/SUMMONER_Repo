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
            Debug.Log($"{attacker.getSummonName()}��(��) {closestEnemySummon.getSummonName()}��(��) �����մϴ�.");
            closestEnemySummon.takeDamage(attacker.getAttackPower());
        }
        else
        {
            Debug.Log("������ ���� �����ϴ�.");
        }
    }

    private Summon GetClosestEnemySummon(List<Plate> targetPlates)
    {
        for (int i = 0; i < targetPlates.Count; i++)
        {
            Summon enemySummon = targetPlates[i].getCurrentSummon();
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
