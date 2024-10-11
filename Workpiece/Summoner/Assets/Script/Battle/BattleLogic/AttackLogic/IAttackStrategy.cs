using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
   void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex);
}
