using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Cat;
    }

    //����� ��������
    public AttackPrediction getAttackPrediction(Summon cat, int catPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        // �Ϲ� �������� óġ�� �����ϸ� �Ϲ� ���� Ȯ�� 10% ����
        if (getIndexOfNormalAttackCanKill(cat, enermyPlates) != -1)
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "����� �Ϲݰ������� óġ ����");
            attackIndex = getIndexOfNormalAttackCanKill(cat, enermyPlates); //�Ϲ� �������� óġ������ �ε��� �ޱ�
        }
        else
        {
            // Ư�� �������� óġ�� �����ϸ� Ư�� ���� Ȯ�� ����
            if (getIndexOfSpecialCanKill(cat, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "����� Ư���������� óġ����");
                attackIndex = getIndexOfSpecialCanKill(cat, enermyPlates); //Ư�� �������� óġ������ �ε��� �ޱ�
            }
            else
            { //Ư�����ݿ� +5%
                if(getMostDamageAttack(cat) == AttackType.NormalAttack)
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "����� �Ϲݰ����� �� ���� �������� ����");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "����� Ư�������� �� ���� �������� ����");
                }
                attackIndex = getClosestEnermyIndex(enermyPlates); //�� �÷���Ʈ �߿��� ���� ������ �ִ� �ε��� �ޱ�
            }
        }

        attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }

    public AttackType getMostDamageAttack(Summon attackingSummon)
    {

        // ��� ������ Ư�� ���� ��� ��������
        IAttackStrategy[] availableSpecialAttacks = attackingSummon.getAvailableSpecialAttacks();

        // �� Ư�� ���� Ȯ��
        foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
        {
            //Ư�������� null�̸� �Ϲݰ��� ��ȯ
            if(availableSpecialAttacks == null)
            {
                return AttackType.NormalAttack;
            }

            //�Ϲݰ��ݷ��� �� �ظ� �Ϲݰ��� ��ȯ ����̴� ����Ÿ�� �����̱⶧���� ���ݷ¸� ��
            if (attackingSummon.getAttackPower() > specialAttack.getSpecialDamage())
                return AttackType.NormalAttack;
            else
                return AttackType.SpecialAttack;
            }

        return AttackType.NormalAttack;
    }


    // �Ϲ� �������� ���� ����ĥ �� �ִ� ���� ����� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getIndexOfNormalAttackCanKill(Summon cat, List<Plate> enermyPlates)
    {
        // ���� ����� ���� �ε����� ������
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // ���� ����� ���� ��ȯ���� �ְ�, �Ϲ� �������� ����ĥ �� �ִ��� Ȯ��
            if (closestEnermySummon != null && cat.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return closestIndex; // �������� ����ĥ �� ������ �ε��� ��ȯ
            }
        }

        return -1; // ���� ������ ���� ������ -1 ��ȯ
    }

    //Ư����ų�� ���� �� �ִ� �ε��� ��ȯ
    public int getIndexOfSpecialCanKill(Summon cat, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null && cat.getHeavyAttackPower() >= enermySummon.getNowHP())
            {
                // �Ϲ� �������� ���� ü���� 0 ���Ϸ� ���� �� ������ �ش� �ε��� ��ȯ
                return i;
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
            Debug.Log($"�Ϲ� ���� Ȯ���� {AttackChange*100}% �����Ͽ����ϴ�. ����: {reason}. ���� Ȯ��: �Ϲ� {currentProbabilities.normalAttackProbability}%, Ư�� {currentProbabilities.specialAttackProbability}%");
        }
        else
        {
            // Ư�� ���� Ȯ���� ������Ű��, �Ϲ� ���� Ȯ���� �׸�ŭ ����
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
            Debug.Log($"Ư�� ���� Ȯ���� {AttackChange * 100}% �����Ͽ����ϴ�. ����: {reason}. ���� Ȯ��: �Ϲ� {currentProbabilities.normalAttackProbability}%, Ư�� {currentProbabilities.specialAttackProbability}%");
        }
        return currentProbabilities;
    }

}
