using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // ���� Ÿ�� (�������� ������)
    private double damage;
    public TargetedAttackStrategy(StatusType statusType, double damage)
    {
        this.statusType = statusType;
        this.damage = damage;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int Arrayindex)
    {
        Summon targetSummon = targetPlates[selectedPlateIndex].getSummon();

        if (targetSummon != null)
        {
            if (statusType == StatusType.Heal) // ���� ���
            {
                double healAmount = targetSummon.MaxHP * 0.3; // �ִ� ü���� 30%��ŭ ȸ��
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

}
