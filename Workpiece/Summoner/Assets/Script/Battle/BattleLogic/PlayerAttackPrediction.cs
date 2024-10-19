using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    public AttackProbability catAttackProbability(List<Plate> playerSummons, List<Plate> enermySummons, AttackProbability attackProbability)
    {
        AttackProbability recalculatedProbability = attackProbability; // �⺻ 50% 50%

        // 1. �Ϲ� ���� �� ���͸� ����ĥ �� �ִ��� ���θ� �Ǵ�
        if (CanDefeatWithNormalAttack(playerSummons, enermySummons))
        {
            recalculatedProbability.normalAttackProbability += 0.1f; // �Ϲ� ���� Ȯ�� +10%
        }

        // 2. �� ��ȯ���� Ư�� ���� ������ ��ȸ�Ͽ� ������ üũ
        foreach (Plate plate in playerSummons)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null)
            {
                IAttackStrategy[] specialAttackStrategies = summon.getAvailableSpecialAttacks(); //��밡���� ��ų���� ��ȯ

                foreach (IAttackStrategy specialAttack in specialAttackStrategies)
                {
                    if (CanDefeatWithSpecialAttack(specialAttack, enermySummons))
                    {
                        recalculatedProbability.specialAttackProbability += 0.1f; // Ư�� ���� Ȯ�� +10%
                    }
                }
            }
        }

        // 3. ���ظ� ���� �� �� �ִ� ���ݿ� �߰����� Ȯ���� �ο�
        if (recalculatedProbability.normalAttackProbability > recalculatedProbability.specialAttackProbability)
        {
            recalculatedProbability.normalAttackProbability += 0.05f; // �Ϲ� ���� Ȯ�� +5%
        }
        else
        {
            recalculatedProbability.specialAttackProbability += 0.05f; // Ư�� ���� Ȯ�� +5%
        }

        return recalculatedProbability;
    }

    // �Ϲ� �������� ���� ����ĥ �� �ִ��� ���θ� �Ǵ��ϴ� �޼���
    private bool CanDefeatWithNormalAttack(List<Plate> playerSummons, List<Plate> enermySummons)
    {
        // �Ϲ� �������� �� ��ȯ���� ����ĥ �� �ִ��� Ȯ���ϴ� ���� �߰�
        // ���� ����: ���� ����� �� ��ȯ���� Ÿ������
        foreach (Plate enermyPlate in enermySummons)
        {
            Summon enermySummon = enermyPlate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() < 50) // ������ ���� ����
            {
                return true;
            }
        }
        return false;
    }

    // Ư�� Ư�� ���� �������� ���� ����ĥ �� �ִ��� ���θ� �Ǵ��ϴ� �޼���
    private bool CanDefeatWithSpecialAttack(IAttackStrategy specialAttack, List<Plate> enermySummons)
    {
        // Ư�� �������� �� ��ȯ���� ����ĥ �� �ִ��� Ȯ���ϴ� ���� �߰�
        foreach (Plate enermyPlate in enermySummons)
        {
            Summon enermySummon = enermyPlate.getCurrentSummon();
            //if (enermySummon != null && specialAttack.getCalculateDamage() > enermySummon.getNowHP()) // Ư�� ������ �� ü�º��� ū�� Ȯ��
            //{
            //    return true;
            //}
        }
        return false;
    }
}
