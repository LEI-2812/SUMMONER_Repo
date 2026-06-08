using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
   void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int SpecialAttackarrayIndex);

    StatusType GetStatusType(); // 상태 타입 반환 메서드 추가
    bool BenefitEffectCheck(); // 이로운 효과 여부 반환
    double GetSpecialDamage(); //특수공격력값 반환
    int GetCooltime(); // 쿨타임 값 반환
    int GetCurrentCooldown(); // 현재 쿨타임 상태 반환
    void ApplyCooldown(); // 쿨타임 적용
    void ReduceCooldown(); // 턴 종료 시 쿨타임 감소
}
