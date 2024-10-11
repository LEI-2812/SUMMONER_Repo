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
            Debug.Log($"{attacker.SummonName}��(��) {selectedEnemySummon.SummonName}��(��) �����ϰ� �����մϴ�.");
            selectedEnemySummon.takeDamage(attacker.SpecialPower); // ������ ����
        }
        else
        {
            Debug.Log("���õ� plate�� ���� �����ϴ�.");
        }
    }
}
