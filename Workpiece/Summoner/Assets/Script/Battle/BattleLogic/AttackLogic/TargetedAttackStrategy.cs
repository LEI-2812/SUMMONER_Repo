using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // 상태 타입 (공격인지 힐인지)
    private double damage;
    private int cooltime;
    private int currentCooldown;
    public TargetedAttackStrategy(StatusType statusType, double damage, int cooltime)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int Arrayindex)
    {
        Summon targetSummon = targetPlates[selectedPlateIndex].getSummon();

        if (targetSummon != null)
        {
            if (statusType == StatusType.Heal) // 힐인 경우
            {
                double healAmount = targetSummon.getMaxHP() * 0.3; // 최대 체력의 30%만큼 회복
                targetSummon.Heal(healAmount);
                Debug.Log($"{attacker.getSummonName()}이(가) {targetSummon.getSummonName()}을(를) {healAmount}만큼 치유했습니다.");
            }
            else // 공격인 경우
            {
                Debug.Log($"{attacker.getSummonName()}이(가) {targetSummon.getSummonName()}을(를) 강력하게 공격합니다.");
                targetSummon.takeDamage(attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage()); // 강력한 공격
            }
        }
        else
        {
            Debug.Log("선택된 plate에 대상이 없습니다.");
        }
    }

    public double getSpecialDamage()
    {
        return damage;
    }
    public StatusType getStatusType()
    {
        return statusType;
    }
    public int getCooltime() { return cooltime; }


    public int getCurrentCooldown() => currentCooldown;

    // 쿨타임을 초기화 (스킬 사용 후 적용)
    public void ApplyCooldown() => currentCooldown = cooltime;

    // 턴 종료 시 쿨타임 감소
    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}
