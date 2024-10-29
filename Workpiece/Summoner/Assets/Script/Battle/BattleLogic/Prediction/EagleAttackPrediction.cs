using System;
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

        //��ȯ��, ��ȯ���� �÷���Ʈ ��ȣ, ��ȯ���� Ư������ù��°, Ư�����ݹ迭 �ε�����ȣ, Ÿ���÷���Ʈ, Ÿ���÷���Ʈ ��ȣ, Ȯ��
        AttackPrediction attackPrediction = new AttackPrediction(eagle, eaglePlateIndex, eagle.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (isTwoOrMoreEnemies(enermyPlates)) //���� 2���� �̻��ΰ�?
        {
            if (IsEnermyHealthDifferenceOver30(enermyPlates) != -1) //���� �� ���� �ٸ��ʰ� ������ �� 30% �̻� ������?
            {
                int lowestIndex = IsEnermyHealthDifferenceOver30(enermyPlates);
                if (CanNormalAttack(eagle, enermyPlates, lowestIndex) != -1) //�Ϲ� �������� ü���� ���� ���� ������ �� �ִ°�?
                {
                    attackIndex = CanNormalAttack(eagle, enermyPlates, lowestIndex);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "������ �Ϲݰ������� ü���� ���� �� ���� ����");
                }
                else
                {
                    attackIndex = lowestIndex;
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "������ �Ϲݰ������� ü���� ������ ���� �Ұ���");
                }
            }
            else if (AreEnermyHealthWithin10Percent(eagle,enermyPlates) != -1) //������ ü���� ���� ����Ѱ�? (10%�̳�)
            {
                attackIndex = AreEnermyHealthWithin10Percent(eagle,enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "������ ���� ü���� 10���̳��� �����");
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
                attackIndex = getSpecialAttackKillIndex(eagle, enermyPlates);
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

        attackPrediction = new AttackPrediction(eagle, eaglePlateIndex, eagle.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        Debug.Log("������ �ܳ�: " + attackIndex);
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


    // ���� ���� ü�� �� �ٸ� ��ȯ���� ü�º��� 30% ���� ��ȯ�� �� ���� ü���� ���� ��ȯ���� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int IsEnermyHealthDifferenceOver30(List<Plate> enermyPlates)
    {
        if (enermyPlates.Count < 2) return -1;

        int lowestHealthIndex = -1;
        double lowestHealth = double.MaxValue;

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon currentSummon = enermyPlates[i].getCurrentSummon();
            if (currentSummon == null) continue;

            for (int j = 0; j < enermyPlates.Count; j++)
            {
                if (i == j) continue; // �ڱ� �ڽ��� ������ ����

                Summon compareSummon = enermyPlates[j].getCurrentSummon();
                if (compareSummon == null) continue;

                // ���� ��ȯ���� ü�� ���� ��� (�ڱ� ü�� / �� ���� ü��)
                double healthRatio = currentSummon.getNowHP() / compareSummon.getNowHP();

                // ������ �����ϴ� ��� �߿��� ���� ���� ü���� ���� ��ȯ���� �ε����� ����
                if (healthRatio <= 0.7 && currentSummon.getNowHP() < lowestHealth)
                {
                    lowestHealth = currentSummon.getNowHP();
                    lowestHealthIndex = i;
                }
            }
        }

        return lowestHealthIndex; // ������ �����ϴ� ���� ���� ü���� ��ȯ�� �ε��� ��ȯ, ������ -1 ��ȯ
    }


    //ü���� ���� ���� ������ �ε����� ������ ������ �ε����� ���ϴ� �޼ҵ�
    public bool IsLowestHealthEnermyClosest(Summon attackingSummon, List<Plate> enermyPlates, int lowestIndex)
    {
        // �� ��ȯ���� 2�� �̸��� ��� ���� �� �����Ƿ� false ��ȯ
        if (enermyPlates.Count < 2) return false;

        //���� ����� �ε��� ��ȯ
        int closetIndex = getClosestEnermyIndex(attackingSummon, enermyPlates);

        if (closetIndex == lowestIndex)
            return true;

        return false;
    }

    // ���� ����� �� ��ȯ���� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getClosestEnermyIndex(Summon attackingSummon, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null && enermySummon != attackingSummon)
            {
                return i; // ù ��°�� ������ ��ȿ�� �� ��ȯ���� �ε����� ��ȯ
            }
        }

        return -1; // �� ��ȯ���� ���� ��� -1 ��ȯ
    }


    // ���͵��� ü���� ���� 10% �̳� �������� �˻��ϴ� �޼ҵ�
    public int AreEnermyHealthWithin10Percent(Summon eagle,List<Plate> enermyPlates)
    {
        if (enermyPlates.Count < 2) return -1;

        // ��� ������ Ư�� ������ �ִ��� ���� �˻�
        bool hasAvailableSpecialAttack = false;
        for (int i = 0; i < eagle.getSpecialAttackStrategy().Length; i++)
        {
            if (!eagle.isSpecialAttackCool(eagle.getSpecialAttackStrategy()[i]))
            {
                hasAvailableSpecialAttack = true;
                break;
            }
        }

        // ��� ������ Ư�� ������ ������ -1 ��ȯ
        if (!hasAvailableSpecialAttack)
        {
            return -1;
        }

        int maxHealthIndex = -1;
        double highestHealth = double.MinValue;

        // �ִ� ���� ü���� ���� ��ȯ���� ã��
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon currentSummon = enermyPlates[i].getCurrentSummon();
            if (currentSummon == null) continue;

            double currentHealth = currentSummon.getNowHP();

            if (currentHealth > highestHealth)
            {
                highestHealth = currentHealth;
                maxHealthIndex = i;
            }
        }

        // ���� ü���� ���� ���� ��ȯ���� ���ٸ� -1 ��ȯ
        if (maxHealthIndex == -1) return -1;

        // ��ȯ�� �ε��� �ʱ�ȭ (-1�� ����, ���� ���� �� maxHealthIndex�� ����)
        int resultIndex = maxHealthIndex;
        double maxHealth = highestHealth;

        // �ִ� ���� ü���� ���� ��ȯ���� �����ϰ� ������ ��ȯ������ 10% �̳� �������� �˻�
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (i == maxHealthIndex) continue; // �ִ� ���� ü�� ��ȯ���� �񱳿��� ����

            Summon compareSummon = enermyPlates[i].getCurrentSummon();
            if (compareSummon == null) continue;

            double healthDifference = Math.Abs(maxHealth - compareSummon.getNowHP());

            // 10% �̻��� ���̰� ���� ������ �������� �����Ƿ� resultIndex�� -1�� �����ϰ� ����
            if (healthDifference > maxHealth * 0.1)
            {
                resultIndex = -1;
                break;
            }
        }

        // ��� ��ȯ���� 10% �̳� ���̸� ������ ��쿡�� �ִ� ü�� ��ȯ���� �ε����� ��ȯ
        return resultIndex;
    }

    private int CanNormalAttack(Summon attackingSummon ,List<Plate> enermyPlates , int lowestIndex)
    {
        // �� ��ȯ���� 2�� �̸��� ��� ���� �� �����Ƿ� false ��ȯ
        if (enermyPlates.Count < 2) return -1;

        //���� ����� �ε��� ��ȯ
        int closetIndex = getClosestEnermyIndex(attackingSummon, enermyPlates);

        if (closetIndex == lowestIndex)
            return closetIndex;

        return -1;
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
        // ��� ������ Ư�� ������ �ִ��� ���� �˻�
        bool hasAvailableSpecialAttack = false;
        for (int i = 0; i < eagle.getSpecialAttackStrategy().Length; i++)
        {
            if (!eagle.isSpecialAttackCool(eagle.getSpecialAttackStrategy()[i]))
            {
                hasAvailableSpecialAttack = true;
                break;
            }
        }

        // ��� ������ Ư�� ������ ������ -1 ��ȯ
        if (!hasAvailableSpecialAttack)
        {
            return -1;
        }


        for (int i=0; i< eagle.getSpecialAttackStrategy().Length; i++)
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
