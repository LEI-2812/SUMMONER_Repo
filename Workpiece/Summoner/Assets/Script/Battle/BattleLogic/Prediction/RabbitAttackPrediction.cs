using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitAttackPrediction : MonoBehaviour, IAttackPrediction
{
 

    public SummonType getPreSummonType()
    {
        return SummonType.Rabbit;
    }

    public AttackPrediction getAttackPrediction(Summon rabbit, int rabbitPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (GetIndexOfLowerHealthIfDifferenceOver30(playerPlates) != -1) //��ȯ�� �� ������ �ٸ� �ʰ� ü���� ������ �� 30% �̻� ������?
        {
            attackIndex = GetIndexOfLowerHealthIfDifferenceOver30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "�䳢 ��ȯ���� ������ �ٸ� �ʰ� ���Ҷ� 30% ����");
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (getIndexOfLowerHealthIfAllDown30(playerPlates) != -1) //��ȯ�� ����� ü���� 30% �����ΰ�?
        {
            attackIndex = getIndexOfLowerHealthIfAllDown30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "�䳢 ��ȯ���� ü���� ��� 30% ����");
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (AllPlayerSummonOver70Percent(playerPlates))
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "�䳢 ��� �÷��̾� ��ȯ�� ü���� 70% �̻�");
            attackIndex = getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (getIndexOfNormalAttackCanKill(rabbit, enermyPlates) != -1)
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "�䳢 �Ϲݰ������� óġ����");
            attackIndex = getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else// ��� ������ �ȸ����� ���� ���� ü�� �Ʊ� ��
        {
            attackIndex = getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }


        return attackPrediction;
    }




    // ��ȯ���� ü�� ���̰� 30% �̻� ������ Ȯ���ϴ� �޼ҵ�
    // �Ʊ� ��ȯ�� �� ü�� ���̰� 30% �̻��� ���, �� ���� ü���� ���� ��ȯ���� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int GetIndexOfLowerHealthIfDifferenceOver30(List<Plate> playerPlates)
    {
        if (playerPlates.Count < 2) return -1; // �Ʊ��� 2�� �̸��̸� ���� �� ����

        double maxHealthRatio = double.MinValue;
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon enermySummon = playerPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i; // ü���� �� ���� ��ȯ���� �ε����� ���
                }
                if (healthRatio > maxHealthRatio)
                {
                    maxHealthRatio = healthRatio;
                }
            }
        }

        // ü�� ���̰� 30% �̻��� ��쿡�� �ε��� ��ȯ
        if ((maxHealthRatio - minHealthRatio) > 0.3f)
        {
            return indexOfMinHealth;
        }

        return -1; // ü�� ���̰� 30% �̻��� �ƴϸ� -1 ��ȯ
    }


    // ��� �÷��̾� ��ȯ�� �� ü���� 30% ������ ��ȯ�� �� ���� ���� ü���� ���� ��ȯ���� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getIndexOfLowerHealthIfAllDown30(List<Plate> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].getCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.getNowHP() / playerSummon.getMaxHP();

                // ��� ��ȯ���� ü�� 30% �������� Ȯ��
                if (healthRatio > 0.3f)
                {
                    return -1; // �ϳ��� ü���� 30%�� ������ -1 ��ȯ
                }

                // ���� ���� ü���� ���� ��ȯ�� �ε����� ���
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i;
                }
            }
        }

        return indexOfMinHealth; // ��� ��ȯ���� 30% ������ ��� ���� ���� ü���� �ε����� ��ȯ
    }

    // �÷��̾� ��ȯ���� ü���� ��� 70% �̻����� Ȯ��
    public bool AllPlayerSummonOver70Percent(List<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
            if (playerSummon != null)
            {
                double healthRatio = playerSummon.getNowHP() / playerSummon.getMaxHP();
                if (healthRatio < 0.7f)
                {
                    return false; // �ϳ��� 70% �����̸� false ��ȯ
                }
            }
        }
        return true;
    }

    // ���� ���� ü���� ���� ��ȯ���� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getIndexOfLowestHealthSummon(List<Plate> playerPlates)
    {
        double minHealthRatio = double.MaxValue;
        int indexOfMinHealth = -1;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].getCurrentSummon();
            if (summon != null)
            {
                double healthRatio = summon.getNowHP() / summon.getMaxHP();

                // ���� ���� ü���� ���� ��ȯ���� �ε��� ���
                if (healthRatio < minHealthRatio)
                {
                    minHealthRatio = healthRatio;
                    indexOfMinHealth = i;
                }
            }
        }

        return indexOfMinHealth; // ���� ���� ü���� ��ȯ�� �ε��� ��ȯ
    }

    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϴ� �޼ҵ�
    public int getIndexOfNormalAttackCanKill(Summon rabbit, List<Plate> enermyPlates)
    {
        // ���� ����� ���� �ε����� ������
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // ���� ����� ���� ��ȯ���� �ְ�, �Ϲ� �������� ����ĥ �� �ִ��� Ȯ��
            if (closestEnermySummon != null && rabbit.getAttackPower() >= closestEnermySummon.getNowHP())
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
