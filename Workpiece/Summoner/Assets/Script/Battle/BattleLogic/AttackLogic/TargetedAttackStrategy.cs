using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // ���� Ÿ�� (�������� ������)

    public TargetedAttackStrategy(StatusType statusType)
    {
        this.statusType = statusType;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex)
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
                targetSummon.takeDamage(attacker.SpecialPower); // ������ ����
            }
        }
        else
        {
            Debug.Log("���õ� plate�� ����� �����ϴ�.");
        }
    }


    public StatusType getTargetAttackStatusType()
    {
        return statusType;
    }
}
