using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // 상태 타입 (공격인지 힐인지)

    public TargetedAttackStrategy(StatusType statusType)
    {
        this.statusType = statusType;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex)
    {
        Summon targetSummon = targetPlates[selectedPlateIndex].getSummon();

        if (targetSummon != null)
        {
            if (statusType == StatusType.Heal) // 힐인 경우
            {
                double healAmount = targetSummon.MaxHP * 0.3; // 최대 체력의 30%만큼 회복
                targetSummon.Heal(healAmount);
                Debug.Log($"{attacker.getSummonName()}이(가) {targetSummon.getSummonName()}을(를) {healAmount}만큼 치유했습니다.");
            }
            else // 공격인 경우
            {
                Debug.Log($"{attacker.getSummonName()}이(가) {targetSummon.getSummonName()}을(를) 강력하게 공격합니다.");
                targetSummon.takeDamage(attacker.SpecialPower); // 강력한 공격
            }
        }
        else
        {
            Debug.Log("선택된 plate에 대상이 없습니다.");
        }
    }


    public StatusType getTargetAttackStatusType()
    {
        return statusType;
    }
}
