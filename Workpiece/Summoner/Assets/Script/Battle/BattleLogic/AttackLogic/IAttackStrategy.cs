using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
   void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackarrayIndex);

    StatusType getStatusType(); // 상태 타입 반환 메서드 추가
    double getSpecialDamage(); //특수공격력값 반환
    int getCooltime(); // 쿨타임 값 반환
    int getCurrentCooldown(); // 현재 쿨타임 상태 반환
    void ApplyCooldown(); // 쿨타임 적용
    void ReduceCooldown(); // 턴 종료 시 쿨타임 감소
}
