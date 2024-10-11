using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttackStrategy : IAttackStrategy
{
    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex)
    {
        Summon selectedEnemySummon = enemyPlates[selectedPlateIndex].getSummon();

        if (selectedEnemySummon != null)
        {
            Debug.Log($"{attacker.SummonName}이(가) {selectedEnemySummon.SummonName}을(를) 강력하게 공격합니다.");
            selectedEnemySummon.takeDamage(attacker.SpecialPower); // 강력한 공격
        }
        else
        {
            Debug.Log("선택된 plate에 적이 없습니다.");
        }
    }
}
