using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
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
    public bool hasMonsterWithMoreThan4Attacks(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getSpecialAttackCount() >= 4)
            {
                return true;
            }
        }
        return false;
    }


    // ���� ����� ���� �������� �� ����ĥ �� �ִ��� Ȯ���ϴ� �޼ҵ�
    public bool CanNormalAttackKill(Summon snake, List<Plate> enermyPlates)
    {
        int closestIndex = getClosestEnermyIndex(enermyPlates);
        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            if (closestEnermySummon != null && snake.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return true; // ���� ����� ���� �Ϲ� �������� óġ�� �� ������ true ��ȯ
            }
        }
        return false; // óġ�� �� ������ false ��ȯ
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
