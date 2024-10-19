using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAttackPrediction : MonoBehaviour
{
    private PlateController plateController;
    private void Awake()
    {
        plateController = GetComponent<PlateController>();
    }


    // �Ϲ� �������� ���� ����ĥ �� �ִ� ���� ����� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int getIndexofNormalAttackCanKill(Summon attackingSummon, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null && attackingSummon.getAttackPower() >= enermySummon.getNowHP())
            {
                // �Ϲ� �������� ���� ü���� 0 ���Ϸ� ���� �� ������ �ش� �ε��� ��ȯ
                return i;
            }
        }
        return -1; // ���� ������ ���� ������ -1 ��ȯ
    }

    //Ư����ų�� ���� �� �ִ� �ε��� ��ȯ
    public int getIndexofSpecialCanKill(Summon attackingSummon, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null && attackingSummon.getAttackPower() >= enermySummon.getNowHP())
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

}
