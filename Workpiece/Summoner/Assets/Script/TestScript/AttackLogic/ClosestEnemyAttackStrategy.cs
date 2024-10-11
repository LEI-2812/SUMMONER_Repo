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
            Debug.Log($"{attacker.SummonName}��(��) {closestEnemySummon.SummonName}��(��) �����մϴ�.");
            closestEnemySummon.takeDamage(attacker.AttackPower);
        }
        else
        {
            Debug.Log("������ ���� �����ϴ�.");
        }
    }

    private Summon GetClosestEnemySummon(int playerPlateIndex, List<Plate> enemyPlates)
    {
        Summon closestSummon = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].getSummon();
            if (enemySummon != null)
            {
                float distance = Mathf.Abs(playerPlateIndex - i);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSummon = enemySummon;
                }
            }
        }

        return closestSummon;
    }
}
