using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Wolf;
    }

    public AttackPrediction getAttackPrediction(Summon wolf, int wolfPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(wolf, wolfPlateIndex, wolf.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (IsEnermyCountTwoOrMore(enermyPlates)) //���� 2���� �̻��ΰ�?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "���� ���� 2���� �̻�");
            if (AllEnermyHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "���� �� ���� ü���� ��� 50% �̻�");
            }
            else if (AllEnermyHealthDown50(enermyPlates)) //���� ü���� ��� 50% �����ΰ�?
            {
                if (HasSpecificAvailableSpecialAttack(wolf, enermyPlates))
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "���� �� ���� ü���� ��� 50% ����");
                }
            }
            else if (IsEnermyHealthDifferenceOver30(enermyPlates)) //���� ������ �ٸ� �ʿ����� ü���� 30% ������?
            {
                if (IsLowestHealthEnermyClosest(wolf, enermyPlates)) //�������� �ε����� ���������ϴ� �ε����� �����Ѱ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 20f, true, "���� ���������� �����ϴ� �ε����� ����");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "���� �Ϲݰ������� �����ϴ� �ε����� ����ġ");
                }
            }
        }
        else if (IsEnermyCountOne(enermyPlates)) //���� 1���� �ΰ�?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "���� ���� 1������");

            if (getIndexOfNormalAttackCanKill(wolf, enermyPlates) != -1) //�Ϲ� �������� ���͸� ����ĥ �� �ִ°�?
            {
                attackIndex = getIndexOfNormalAttackCanKill(wolf, enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "���� �Ϲ� �������� óġ����");
            }
            else
            {
                //�Ϲ� ���ݰ� Ư�������� ���ظ� �� �� �� �ִ� ������ ��ȯ���� �� �Ϲ� �����ϰ��
                if (getMostDamageAttack(wolf, enermyPlates) == AttackType.NormalAttack)
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "���� �Ϲ� ������ �� ���� ���ظ� ����");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "���� Ư�������� �� ���� ���ظ� ����");
                }
            }
        }

        attackPrediction = new AttackPrediction(wolf, wolfPlateIndex, wolf.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }


    // ���� ü���� ��� 50% �̻����� Ȯ���ϴ� �޼ҵ�
    public bool AllEnermyHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.5f)
            {
                return false;
            }
        }
        return true;
    }

    // ���� ü���� ��� 50% �������� Ȯ���ϴ� �޼ҵ�
    public bool AllEnermyHealthDown50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() > 0.5f)
            {
                return false;
            }
        }
        return true;
    }

    // ���� ü�� ���̰� 30% �̻����� Ȯ���ϴ� �޼ҵ�
    public bool IsEnermyHealthDifferenceOver30(List<Plate> enermyPlates)
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

    // ���� ü�� ���̰� 30% �̻����� Ȯ���ϰ�, ü���� ���� ���� ������ �ε����� ������ ������ �ε����� ���ϴ� �޼ҵ�
    public bool IsLowestHealthEnermyClosest(Summon attackingSummon, List<Plate> enermyPlates)
    {
        // �� ��ȯ���� 2�� �̸��� ��� ���� �� �����Ƿ� false ��ȯ
        if (enermyPlates.Count < 2) return false;

        double maxHealthRatio = double.MinValue;
        double minHealthRatio = double.MaxValue;
        int lowestHealthIndex = -1;

        // �� ������ �ִ� ü�� ������ �ּ� ü�� ������ ����ϰ�, �ּ� ü���� ���� �ε��� ����
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio > maxHealthRatio) maxHealthRatio = healthRatio;
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    lowestHealthIndex = i;
                }
            }
        }

        // ü�� ������ ���̰� 30% �̻����� Ȯ��
        if ((maxHealthRatio - minHealthRatio) > 0.3f)
        {
            // ü���� ���� ���� �� ��ȯ���� �ε����� ���� ����� �� ��ȯ���� �ε����� ��
            int closestIndex = getClosestEnermyIndex(attackingSummon , enermyPlates);
            return (lowestHealthIndex == closestIndex);
        }

        return false; // ü�� ���̰� 30% �̻��� �ƴϰų� ������ �������� ������ false ��ȯ
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



    // ���� 2���� �̻����� Ȯ���ϴ� �޼ҵ�
    public bool IsEnermyCountTwoOrMore(List<Plate> enermyPlates)
    {
        return enermyPlates.Count >= 2;
    }

    // ���� 1�������� Ȯ���ϴ� �޼ҵ�
    public bool IsEnermyCountOne(List<Plate> enermyPlates)
    {
        return enermyPlates.Count == 1;
    }


    // �ڱ� �ڽ��� �����ϰ� �÷��̾��� ��ȯ���� Ư�������� �������� ��ų�� �ִ��� Ȯ���ϴ� �޼ҵ�
    public bool HasSpecificAvailableSpecialAttack(Summon self, List<Plate> playerPlates)
    {
        // �˻��� Ư�� ���� ���� Ÿ�Ե� (None, Burn, Poison, LifeDrain)
        StatusType[] specificStatuses = new StatusType[] { StatusType.None, StatusType.Burn, StatusType.Poison, StatusType.LifeDrain };

        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
            // �ڱ� �ڽ��� �����ϰ� �˻�
            if (playerSummon != null && playerSummon != self)
            {
                // ��ȯ���� ��� ������ Ư�� ���ݵ��� ������
                IAttackStrategy[] availableSpecialAttacks = playerSummon.getAvailableSpecialAttacks();

                // ��� ������ Ư�� ���� �� Ư�� ���°� �ִ��� Ȯ��
                foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
                {
                    if (specialAttack != null && specificStatuses.Contains(specialAttack.getStatusType()))
                    {
                        // Ư�� ���°� �ִ� Ư�� ������ �ִٸ� true ��ȯ
                        return true;
                    }
                }
            }
        }
        return false; // Ư�� ���°� ������ false ��ȯ
    }


    public AttackType getMostDamageAttack(Summon attackingSummon, List<Plate> enermyPlates)
    {
        double maxDamage = attackingSummon.getAttackPower(); // �⺻��: �Ϲ� ������ ������


        // ��� ������ Ư�� ���� ��� ��������
        IAttackStrategy[] availableSpecialAttacks = attackingSummon.getAvailableSpecialAttacks();

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

    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϴ� �޼ҵ�
    public int getIndexOfNormalAttackCanKill(Summon wolf, List<Plate> enermyPlates)
    {
        // ���� ����� ���� �ε����� ������
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // ���� ����� ���� ��ȯ���� �ְ�, �Ϲ� �������� ����ĥ �� �ִ��� Ȯ��
            if (closestEnermySummon != null && wolf.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return closestIndex; // �������� ����ĥ �� ������ �ε��� ��ȯ
            }
        }

        return -1; // ���� ������ ���� ������ -1 ��ȯ
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
