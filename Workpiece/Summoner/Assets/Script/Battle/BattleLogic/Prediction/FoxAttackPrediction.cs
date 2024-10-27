using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Fox;
    }

    //���� ��������
    public AttackPrediction getAttackPrediction(Summon fox, int foxPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        List<Plate> targetPlate = enermyPlates;

        //��ȯ��, ��ȯ���� �÷���Ʈ ��ȣ, ��ȯ���� Ư������ù��°, Ư�����ݹ迭 �ε�����ȣ, Ÿ���÷���Ʈ, Ÿ���÷���Ʈ ��ȣ, Ȯ��
        AttackPrediction attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);


        if (getIndexOfSummonWithCurseStatus(playerPlates) != -1) //��ȯ���� ���ֻ��¿� �ɷ��ִ� ���� �ִ°�?
        {
            attackProbability = new AttackProbability(0f, 100f); //Ư������ 100%
            attackIndex = getIndexOfSummonWithCurseStatus(playerPlates);
            attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (isTwoOrMoreEnemies(enermyPlates)) //���� 2���� �̻� �����ϴ°�?
        {
            if (AllEnemiesHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
            {
                if (AllSummonsLowOrMediumRank(playerPlates)) //�Ʊ��� ����� ��� �ϱް� �߱��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "���� �Ʊ� ����� ��� �ϱް� �߱�");
                    targetPlate = playerPlates;
                }
            }
            else if (isAnyEnemyHealthDown30Percent(enermyPlates)) //���� ü���� �ϳ��� 30% �Ʒ��ΰ�
            {
                if (getIndexOfNormalAttackCanKill(fox, enermyPlates) != -1) //�Ϲݰ��ݽ� óġ�� �� �ִ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "���� �Ϲݰ��ݽ� óġ����");
                }
            }
        }
        else if (isOnlyOneEnemy(enermyPlates)) //���� 1���� �ΰ�?
        {
            if (isAnyEnemyHealthOver70Percent(enermyPlates)) //���� ü���� 70% �̻��ΰ�?
            {
                if (AllSummonsLowOrMediumRank(playerPlates)) //�Ʊ��� ����� ��� �ϱް� �߱��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "���� �Ʊ� ����� ��� �ϱް� �߱�");
                    targetPlate = playerPlates;
                }
            }
            else if (getIndexOfNormalAttackCanKill(fox, enermyPlates) != -1) //�Ϲݰ��ݽ� ���͸� ����ĥ �� �ִ°�?
            {
                if (getIndexOfNormalAttackCanKill(fox, enermyPlates) != -1) //�Ϲݰ��ݽ� óġ�� �� �ִ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "���� �Ϲݰ��ݽ� ������� óġ ����");
                }
            }
        }
        else
        {
            attackIndex = getIndexOfHighestAttackPower(playerPlates);
            attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }

        return attackPrediction;
    }

    // ��ȯ�� �� ���� ���� �̻� �ɷ��ִ� ���� �����ϴ°�?
    public int getIndexOfSummonWithCurseStatus(List<Plate> playerPlates)
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].getCurrentSummon();
            if (playerSummon != null && playerSummon.IsCursed())
            {
                return i; // ���� ���¿� �ɸ� ��ȯ���� �ε��� ��ȯ
            }
        }
        return -1; // ���� ���¿� �ɸ� ��ȯ���� ������ -1 ��ȯ
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

    // ���� ü���� ��� 50% �̻��ΰ�?
    public bool AllEnemiesHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.5)
            {
                return false;
            }
        }
        return true;
    }

    // �Ʊ��� ����� ��� �ϱް� �߱��ΰ�?
    public bool AllSummonsLowOrMediumRank(List<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
            if (playerSummon != null && (playerSummon.getSummonRank() != SummonRank.Low && playerSummon.getSummonRank() != SummonRank.Medium))
            {
                return false;
            }
        }
        return true;
    }

    // ���� ü���� �ϳ��� 30% �Ʒ��ΰ�?
    public bool isAnyEnemyHealthDown30Percent(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.3)
            {
                return true;
            }
        }
        return false;
    }

    // ���� ü���� 70% �̻��ΰ�?
    public bool isAnyEnemyHealthOver70Percent(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() > 0.7)
            {
                return true;
            }
        }
        return false;
    }

    // ���� ���ݷ��� ���� ��ȯ���� �÷���Ʈ �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getIndexOfHighestAttackPower(List<Plate> playerPlates)
    {
        int highestAttackIndex = -1;
        double highestAttackPower = double.MinValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].getCurrentSummon();
            if (summon != null)
            {
                double attackPower = summon.getAttackPower();
                if (attackPower > highestAttackPower)
                {
                    highestAttackPower = attackPower;
                    highestAttackIndex = i; // ���� ���� ���ݷ��� ���� ��ȯ���� �ε����� ���
                }
            }
        }

        return highestAttackIndex; // ���� ���� ���ݷ��� ��ȯ�� �ε��� ��ȯ
    }



    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϴ� �޼ҵ�
    public int getIndexOfNormalAttackCanKill(Summon fox, List<Plate> enermyPlates)
    {
        // ���� ����� ���� �ε����� ������
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // ���� ����� ���� ��ȯ���� �ְ�, �Ϲ� �������� ����ĥ �� �ִ��� Ȯ��
            if (closestEnermySummon != null && fox.getAttackPower() >= closestEnermySummon.getNowHP())
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
