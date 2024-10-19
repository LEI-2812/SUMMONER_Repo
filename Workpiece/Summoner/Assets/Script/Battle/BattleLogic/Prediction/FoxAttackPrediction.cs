using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
    }

    // ��ȯ�� �� ���� ���� �̻� �ɷ��ִ� ���� �����ϴ°�?
    public bool isAnySummonWithCurseStatus(List<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
            if (playerSummon != null && playerSummon.IsCursed())
            {
                return true;
            }
        }
        return false;
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



    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϴ� �޼ҵ�
    public bool CanNormalAttackKill(Summon fox, List<Plate> enermyPlates)
    {
        int closestIndex = getClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            if (closestEnermySummon != null && fox.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return true; // ���� ����� ���� �Ϲ� �������� óġ�� �� ������ true ��ȯ
            }
        }
        return false; // óġ�� �� ������ false ��ȯ
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
