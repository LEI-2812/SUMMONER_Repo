using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Eagle;
    }


    //������ ��������
    public AttackPrediction getAttackPrediction(Summon eagle, int eaglePlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates); //���� ��������� �ε��� �⺻��


        if (isTwoOrMoreEnemies(enermyPlates)) //���� 2���� �̻��ΰ�?
        {
            if (isEnermyHealthDifferenceOver30(enermyPlates)) //���� �� ���� �ٸ��ʰ� ������ �� 30% �̻� ������?
            {
                if (getIndexOfNormalAttackCanKill(eagle, enermyPlates) != -1) //�Ϲ� �������� ü���� ���� ���� ������ �� �ִ°�?
                {
                    attackIndex = getIndexOfNormalAttackCanKill(eagle, enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "������ �Ϲݰ������� ü���� ���� �� ���� ����");
                }
                else
                {
                    attackIndex = getSpecialAttackKillIndex(eagle, enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "������ �Ϲݰ������� ü���� ������ ���� �Ұ���");
                }
            }
            else if (AreEnermyHealthWithin10Percent(enermyPlates)) //������ ü���� ���� ����Ѱ�?
            {
                if (getIndexOfMostHealthEnermy(enermyPlates) != -1) //���� ü���� ���� ������ �ε���
                {
                    attackIndex = getIndexOfMostHealthEnermy(enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "������ ���� ü���� 10���̳��� �����");
                }
            }
        }
        else if (isOnlyOneEnemy(enermyPlates)) //���� 1���� �ΰ�?
        {
            if (getIndexOfNormalAttackCanKill(eagle, enermyPlates) != -1)
            {
                attackIndex = getIndexOfNormalAttackCanKill(eagle, enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "������ �Ϲݰ������� ��ɰ���");
            }
            else if (getSpecialAttackKillIndex(eagle, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "������ Ư���������� ��ɰ���");
            }
            else
            {
                if (getTypeOfMoreAttackDamage(eagle, enermyPlates) == AttackType.NormalAttack)//�Ϲݰ��ݰ� Ư������ �� ���ظ� ���� �� ���ݿ� 5%���
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "������ �Ϲݰ����� �� ū ���ظ� ����");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "������ �Ϲݰ����� �� ū ���ظ� ����");
                }
            }
        }

        //��ȯ��, ��ȯ���� �÷���Ʈ ��ȣ, ��ȯ���� Ư������ù��°, Ư�����ݹ迭 �ε�����ȣ, Ÿ���÷���Ʈ, Ÿ���÷���Ʈ ��ȣ, Ȯ��
        AttackPrediction attackPrediction = new AttackPrediction(eagle, eaglePlateIndex, eagle.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
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
        for(int i=0; i< eagle.getSpecialAttackStrategy().Length; i++)
        {
            if (eagle.isSpecialAttackCool(eagle.getSpecialAttackStrategy()[i]))
            {
                continue;
            }
            for (int ii = 0; ii < enermyPlates.Count; ii++)
            {
                Summon enermySummon = enermyPlates[ii].getCurrentSummon();
                if (enermySummon != null && eagle.getSpecialAttackStrategy()[i].getSpecialDamage() >= enermySummon.getNowHP())
                {
                    return i; // Ư�� �������� óġ ������ �ε��� ��ȯ
                }
            }
        }
        return -1; // óġ�� �� ������ -1 ��ȯ
    }

    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϰ�, ���� ������ �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getIndexOfNormalAttackCanKill(Summon eagle, List<Plate> enermyPlates)
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

    // Ȯ�� ���� �����ϰ� �����Ͽ� ��ȯ�ϴ� �޼ҵ�
    private AttackProbability AdjustAttackProbabilities(AttackProbability currentProbabilities, float AttackChange, bool isNormalAttack, string reason)
    {
        if (isNormalAttack)
        {
            // �Ϲ� ���� Ȯ���� ������Ű��, Ư�� ���� Ȯ���� �׸�ŭ ����
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
            Debug.Log($"�Ϲ� ���� Ȯ���� {AttackChange}% �����Ͽ����ϴ�. ����: {reason}. ���� Ȯ��: �Ϲ� {currentProbabilities.normalAttackProbability}%, Ư�� {currentProbabilities.specialAttackProbability}%");
        }
        else
        {
            // Ư�� ���� Ȯ���� ������Ű��, �Ϲ� ���� Ȯ���� �׸�ŭ ����
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
            Debug.Log($"Ư�� ���� Ȯ���� {AttackChange}% �����Ͽ����ϴ�. ����: {reason}. ���� Ȯ��: �Ϲ� {currentProbabilities.normalAttackProbability}%, Ư�� {currentProbabilities.specialAttackProbability}%");
        }
        return currentProbabilities;
    }
}
