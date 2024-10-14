using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // 상태 타입 (공격인지 힐인지)
    private double damage; //데미지
    private int cooltime; //쿨타임
    private int currentCooldown; //현재 쿨타임 진행시간
    private int statusTime; //지속시간
    public TargetedAttackStrategy(StatusType statusType, double damage, int cooltime, int statusTime=0)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
        this.statusTime = statusTime;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int Arrayindex)
    {
        Summon target = targetPlates[selectedPlateIndex].getSummon();

        if (target != null)
        {
            switch (statusType)
            {
                case StatusType.Heal:
                    double healAmount = target.getMaxHP() * 0.3; // 최대 체력의 30%만큼 회복
                    target.Heal(healAmount);
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) {healAmount}만큼 치유했습니다.");
                    break;
                case StatusType.None:
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) 강력하게 공격합니다.");
                    target.takeDamage(attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage()); // 강력한 공격
                    break;
                case StatusType.LifeDrain: //흡혈
                    double lifeDrainDamage = target.getMaxHP() * 0.1;
                    attacker.Heal(lifeDrainDamage); // 흡혈한 만큼 체력 회복
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 흡혈을 사용하여 {lifeDrainDamage} 데미지를 입히고 회복합니다.");
                    break;
               
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
