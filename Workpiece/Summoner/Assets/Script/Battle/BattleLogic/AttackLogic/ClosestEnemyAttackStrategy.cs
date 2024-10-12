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
            Debug.Log($"{attacker.getSummonName()}이(가) {closestEnemySummon.getSummonName()}을(를) 공격합니다.");
            closestEnemySummon.takeDamage(attacker.AttackPower);
        }
        else
        {
            Debug.Log("공격할 적이 없습니다.");
        }
    }

    private Summon GetClosestEnemySummon(int playerPlateIndex, List<Plate> enemyPlates)
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon enemySummon = enemyPlates[i].getSummon();
            if (enemySummon != null)
            {
                return enemySummon; // 첫 번째로 존재하는 소환수를 바로 반환
            }
        }

        return null; // 적 소환수가 없으면 null 반환
    }
}
