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
            Debug.Log($"{attacker.SummonName}��(��) {selectedEnemySummon.SummonName}���� ���� �̻��� �ο��մϴ�.");
            // �����̻� �ο� ���� (��: ����)
            //selectedEnemySummon.ApplyStatusEffect("Stun");
        }
        else
        {
            Debug.Log("���õ� plate�� ���� �����ϴ�.");
        }
    }
}
