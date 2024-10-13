using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
   void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex);

    StatusType getStatusType(); // 상태 타입 반환 메서드 추가
}
