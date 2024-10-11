using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAttackStrategy : IAttackStrategy
{
    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex)
    {
        Summon selectedEnemySummon = enemyPlates[selectedPlateIndex].getSummon();

        if (selectedEnemySummon != null)
        {
            Debug.Log($"{attacker.SummonName}이(가) {selectedEnemySummon.SummonName}에게 상태 이상을 부여합니다.");
            // 상태이상 부여 로직 (예: 스턴)
            //selectedEnemySummon.ApplyStatusEffect("Stun");
        }
        else
        {
            Debug.Log("선택된 plate에 적이 없습니다.");
        }
    }
}
