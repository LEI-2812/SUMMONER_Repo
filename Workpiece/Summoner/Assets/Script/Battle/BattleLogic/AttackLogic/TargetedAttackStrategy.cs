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
        Summon target = targetPlates[selectedPlateIndex].getCurrentSummon();

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
                    StatusEffect drainEffect = new StatusEffect(StatusType.LifeDrain, statusTime, lifeDrainDamage, attacker);
                    target.ApplyStatusEffect(drainEffect);
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 흡혈을 사용하여 {lifeDrainDamage} 데미지를 입히고 회복합니다.");
                    break;
                case StatusType.Shield: //쉴드
                    target = attacker;
                    StatusEffect shieldEffect = new StatusEffect(StatusType.Shield, statusTime, damage);
                    target.ApplyStatusEffect(shieldEffect);
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 {damage} 만큼 보호막을 부여합니다.");
                    break;
                case StatusType.Upgrade: //강화
                    double upgradeAttackPower = target.getAttackPower() * attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage();
                    StatusEffect upgradeEffect = new StatusEffect(StatusType.Upgrade, statusTime, upgradeAttackPower);
                    target.ApplyStatusEffect(upgradeEffect);
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 공격력 {upgradeAttackPower} 만큼 상승 시켰습니다.");
                    break;
                case StatusType.OnceInvincibility: //무적
                    target = attacker; //자기자신이 대상
                    target.setOnceInvincibility(true); //1번 무적 활성화
                    break;
                case StatusType.Curse:
                    double curseAttackPower = target.getAttackPower() - (target.getAttackPower() * 0.2); //대상 공격력 20% 감소
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) 강력하게 공격합니다.");
                    StatusEffect curseEffect = new StatusEffect(StatusType.Upgrade, statusTime, curseAttackPower);
                    target.ApplyStatusEffect(curseEffect);
                    target.takeDamage(attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage()); // 강력한 공격
                    break;
                case StatusType.Stun:
                    StatusEffect stunEffect = new StatusEffect(StatusType.Stun, statusTime);
                    target.ApplyStatusEffect(stunEffect);
                    Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 스턴을 적용했습니다.");
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
