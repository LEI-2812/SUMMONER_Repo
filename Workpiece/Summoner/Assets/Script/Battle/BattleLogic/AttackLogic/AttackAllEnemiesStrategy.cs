using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    private StatusType statusType = StatusType.None;

    public AttackAllEnemiesStrategy(StatusType statusType)
    {
        this.statusType = statusType;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon enemySummon = plate.getSummon();
            if (enemySummon != null)
            {
                Debug.Log($"{attacker.getSummonName()}이(가) {enemySummon.getSummonName()}을(를) 전체 공격합니다.");
                enemySummon.takeDamage(attacker.SpecialPower);
            }
        }
    }

    public StatusType getStatusType()
    {
        return statusType;
    }
}
