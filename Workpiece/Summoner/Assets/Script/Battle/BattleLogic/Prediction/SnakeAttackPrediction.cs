using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Snake;
    }

    //�� ��������
    public AttackPrediction getAttackPrediction(Summon snake, int snakePlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (IsEnermyAlreadyPoisoned(enermyPlates))
        { //���͵��� �̹� �ߵ� �����ΰ�?
            attackProbability = new AttackProbability(100f, 0f);
            attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getAttackStrategy(), 0, enermyPlates, attackIndex, attackProbability); //�Ϲݰ�������
        }
        else if (!canUseSpecialAttack(snake)) //Ư�������� ����� �� �ִ°�?
        {
            attackProbability = new AttackProbability(100f, 0f);
            attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getAttackStrategy(), 0, enermyPlates, attackIndex, attackProbability); //�Ϲݰ�������
        }
        else
        {
            if (isEnermyCountOverTwo(enermyPlates)) //���� 2���� �̻��ΰ�?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "�� ���� 2���� �̻�");
                if (AllEnermyHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "�� ���� ü���� ��� 50%�̻�");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "�� ���� ü���� ��� 50%�� �ƴ�");
                }
                if (hasMonsterWithMoreThan3Attacks(enermyPlates)) //���� �� ������ ������ 3�� �̻��� ���� �����ϴ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "�� ���� ������ ������ 3�� �̻��� ���� �ִ°�");
                }
            }
            else //1���� �϶�
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "�� ���� 1���� ��");
                if (AllEnermyHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "�� ���� ü���� 50% �̻�");
                }
                if (getIndexOfNormalAttackCanKill(snake, enermyPlates) != -1) //�Ϲ� ���� �� ���͸� ����ĥ �� �ִ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "�� �Ϲ� ���ݽ� óġ����");
                }
            }
            attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        }

        return attackPrediction;
    }



    // ���� �̹� �ߵ� �������� Ȯ���ϴ� �޼ҵ�
    public bool IsEnermyAlreadyPoisoned(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getAllStatusTypes().Contains(StatusType.Poison))
            {
                return true;
            }
        }
        return false;
    }


    // Ư�� ������ ����� �� �ִ��� Ȯ���ϴ� �޼ҵ�
    public bool canUseSpecialAttack(Summon snake)
    {
        var availableSpecialAttacks = snake.getAvailableSpecialAttacks();
        return availableSpecialAttacks.Length > 0;
    }



    // ��� ���� ü���� 50% �̻����� Ȯ���ϴ� �޼ҵ�
    public bool AllEnermyHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.getNowHP() / enermySummon.getMaxHP();
                if (healthRatio < 0.5)
                {
                    return false; // �ϳ��� 50% �����̸� false ��ȯ
                }
            }
        }
        return true;
    }



    // �� �߿� ���� ������ 4�� �̻��� ��ȯ���� �ִ��� Ȯ���ϴ� �޼ҵ�
    public bool hasMonsterWithMoreThan3Attacks(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getSpecialAttackCount() >= 3)
            {
                return true;
            }
        }
        return false;
    }


    // ���� 2���� �̻� �ִ��� Ȯ���ϴ� �޼ҵ�
    public bool isEnermyCountOverTwo(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null)
            {
                count++;
                if (count >= 2) return true;
            }
        }
        return false;
    }

    // ���� 1���� �̻� �ִ��� Ȯ���ϴ� �޼ҵ�
    public bool isEnermyCountOnlyOne(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null)
            {
                return true;
            }
        }
        return false;
    }


    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϴ� �޼ҵ�
    public int getIndexOfNormalAttackCanKill(Summon snake, List<Plate> enermyPlates)
    {
        // ���� ����� ���� �ε����� ������
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // ���� ����� ���� ��ȯ���� �ְ�, �Ϲ� �������� ����ĥ �� �ִ��� Ȯ��
            if (closestEnermySummon != null && snake.getAttackPower() >= closestEnermySummon.getNowHP())
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
