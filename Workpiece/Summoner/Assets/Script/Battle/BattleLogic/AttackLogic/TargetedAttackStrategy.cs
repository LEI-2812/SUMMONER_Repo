using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // ���� Ÿ�� (�������� ������)
    private double damage;
    private int cooltime;
    private int currentCooldown;
    public TargetedAttackStrategy(StatusType statusType, double damage, int cooltime)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int Arrayindex)
    {
        Summon targetSummon = targetPlates[selectedPlateIndex].getSummon();

        if (targetSummon != null)
        {
            if (statusType == StatusType.Heal) // ���� ���
            {
                double healAmount = targetSummon.getMaxHP() * 0.3; // �ִ� ü���� 30%��ŭ ȸ��
                targetSummon.Heal(healAmount);
                Debug.Log($"{attacker.getSummonName()}��(��) {targetSummon.getSummonName()}��(��) {healAmount}��ŭ ġ���߽��ϴ�.");
            }
            else // ������ ���
            {
                Debug.Log($"{attacker.getSummonName()}��(��) {targetSummon.getSummonName()}��(��) �����ϰ� �����մϴ�.");
                targetSummon.takeDamage(attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage()); // ������ ����
            }
        }
        else
        {
            Debug.Log("���õ� plate�� ����� �����ϴ�.");
        }
    }

    public double getSpecialDamage()
    {
        return damage;
    }
    public StatusType getStatusType()
    {
        return statusType;
    }
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
