using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleAttackPrediction : MonoBehaviour
{

    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
    }


    // ���� 2���� �̻��ΰ�?
    public bool isTwoOrMoreEnemies(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) count++;
        }
        return count >= 2;
    }

    // ���� 1�����ΰ�?
    public bool isOnlyOneEnemy(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) count++;
        }
        return count == 1;
    }


    // ������ ������ �ٸ� ���� ü���� ������ �� 30% �̻� ������?
    public bool isEnermyHealthDifferenceOver30(List<Plate> enermyPlates)
    {
        if (enermyPlates.Count < 2) return false;

        double maxHealthRatio = double.MinValue;
        double minHealthRatio = double.MaxValue;
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio > maxHealthRatio) maxHealthRatio = healthRatio;
                if (healthRatio < minHealthRatio) minHealthRatio = healthRatio;
            }
        }

        return (maxHealthRatio - minHealthRatio) > 0.3f;
    }

    // ���͵��� ü���� ���� 10% �̳� �������� �˻��ϴ� �޼ҵ�
    public bool AreEnermyHealthWithin10Percent(List<Plate> enermyPlates)
    {
        double minHealthRatio = double.MaxValue;
        double maxHealthRatio = double.MinValue;

        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null)
            {
                // ���� ü�� ������ ���
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();

                // �ּ� �� �ִ� ü�� ���� ������Ʈ
                if (healthRatio < minHealthRatio) minHealthRatio = healthRatio;
                if (healthRatio > maxHealthRatio) maxHealthRatio = healthRatio;
            }
        }

        // �ִ�� �ּ� ü�� ������ ���̰� 10% �̳����� �˻�
        return (maxHealthRatio - minHealthRatio) <= 0.1;
    }


    // ���� ü���� ���� ������ �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getIndexOfMostHealthEnermy(List<Plate> enermyPlates)
    {
        int maxHealthIndex = -1;
        double maxHealth = double.MinValue;

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double currentHealth = enermySummon.getNowHP();
                if (currentHealth > maxHealth)
                {
                    maxHealth = currentHealth;
                    maxHealthIndex = i; // ���� �ִ� ü���� ��ȯ���� �ε��� ����
                }
            }
        }

        return maxHealthIndex; // ���� ü���� ���� ��ȯ���� �ε��� ��ȯ, ������ -1
    }

    public AttackType getTypeOfMoreAttackDamage(Summon eagle, List<Plate> enermyPlates)
    {
        double maxDamage = eagle.getAttackPower(); // �⺻��: �Ϲ� ������ ������

        // ��� ������ Ư�� ���� ��� ��������
        IAttackStrategy[] availableSpecialAttacks = eagle.getAvailableSpecialAttacks();

        // �� Ư�� ���� Ȯ��
        foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
        {
            double totalSpecialAttackDamage = 0;

            // �� �÷���Ʈ�� �ִ� ��� ��ȯ������ Ư�� ���� �� ���� ���� ����
            foreach (Plate plate in enermyPlates)
            {
                Summon enermySummon = plate.getCurrentSummon();
                if (enermySummon != null)
                {
                    totalSpecialAttackDamage += specialAttack.getSpecialDamage();
                }
            }

            // Ư�� �������� �� ���ذ� �Ϲ� ���ݺ��� ũ�ٸ� ������Ʈ
            if (totalSpecialAttackDamage > maxDamage)
            {
                maxDamage = totalSpecialAttackDamage;
                return AttackType.SpecialAttack;
            }
        }

        return AttackType.NormalAttack;
    }


    // Ư�� �������� ������ �� �ִ��� Ȯ���ϰ�, ���� ������ �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getSpecialAttackKillIndex(Summon eagle, List<Plate> enermyPlates)
    {
        foreach (IAttackStrategy specialAttack in eagle.getAvailableSpecialAttacks())
        {
            for (int i = 0; i < enermyPlates.Count; i++)
            {
                Summon enermySummon = enermyPlates[i].getCurrentSummon();
                if (enermySummon != null && specialAttack.getSpecialDamage() >= enermySummon.getNowHP())
                {
                    return i; // Ư�� �������� óġ ������ �ε��� ��ȯ
                }
            }
        }
        return -1; // óġ�� �� ������ -1 ��ȯ
    }

    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϰ�, ���� ������ �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getNormalAttackKillIndex(Summon eagle, List<Plate> enermyPlates)
    {
        int closestIndex = getClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            if (closestEnermySummon != null && eagle.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return closestIndex; // ���� ����� ���� �Ϲ� �������� óġ�� �� �ִ� �ε��� ��ȯ
            }
        }
        return -1; // óġ�� �� ������ -1 ��ȯ
    }

    public int getClosestEnermyIndex(List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                return i; // ���� �����(ù ��°�� �߰ߵ�) �� ��ȯ���� �ε��� ��ȯ
            }
        }
        return -1; // �� ��ȯ���� ������ -1 ��ȯ
    }
}
