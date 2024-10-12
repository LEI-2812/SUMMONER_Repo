using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex)
    {
        Summon closestEnemySummon = GetClosestEnemySummon(selectedPlateIndex, enemyPlates);

        if (closestEnemySummon != null)
        {
            Debug.Log($"{attacker.getSummonName()}��(��) {closestEnemySummon.getSummonName()}��(��) �����մϴ�.");
            closestEnemySummon.takeDamage(attacker.AttackPower);
        }
        else
        {
            Debug.Log("������ ���� �����ϴ�.");
        }
    }

    private Summon GetClosestEnemySummon(int playerPlateIndex, List<Plate> enemyPlates)
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].getSummon();
            if (enemySummon != null)
            {
                return enemySummon; // ù ��°�� �����ϴ� ��ȯ���� �ٷ� ��ȯ
            }
        }

        return null; // �� ��ȯ���� ������ null ��ȯ
    }
}
