using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
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


    // �ڱ� �ڽ��� �����ϰ� �÷��̾��� ��ȯ���� Ư�� Ư�� ���� ���°� �ִ��� Ȯ���ϴ� �޼ҵ�
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
}
