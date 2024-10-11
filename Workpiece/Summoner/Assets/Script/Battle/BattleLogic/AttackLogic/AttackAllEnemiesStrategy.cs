using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon enemySummon = plate.getSummon();
            if (enemySummon != null)
            {
                Debug.Log($"{attacker.SummonName}��(��) {enemySummon.SummonName}��(��) ��ü �����մϴ�.");
                enemySummon.takeDamage(attacker.SpecialPower);
            }
        }
    }
}
