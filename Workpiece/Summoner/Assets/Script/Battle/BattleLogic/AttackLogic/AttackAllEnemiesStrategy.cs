using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    private StatusType statusType = StatusType.None;
    private double Damage;
    public AttackAllEnemiesStrategy(StatusType statusType, double damage)
    {
        this.statusType = statusType;
        Damage = damage;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates, int Arrayindex,int selectedPlateIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon enemySummon = plate.getSummon();
            if (enemySummon != null)
            {
                Debug.Log($"{attacker.getSummonName()}��(��) {enemySummon.getSummonName()}��(��) ��ü �����մϴ�.");
                enemySummon.takeDamage(attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage());
            }
        }
    }

    public double getSpecialDamage()
    {
        return Damage;
    }
    public StatusType getStatusType()
    {
        return statusType;
    }
}
