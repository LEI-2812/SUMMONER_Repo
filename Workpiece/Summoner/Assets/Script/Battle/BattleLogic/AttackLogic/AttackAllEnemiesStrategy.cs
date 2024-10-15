using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    private StatusType statusType = StatusType.None;
    private double Damage;
    private int cooltime;
    private int currentCooldown;
    private int statusTime; //지속시간
    public AttackAllEnemiesStrategy(StatusType statusType, double damage, int cooltime, int statusTime=0)
    {
        this.statusType = statusType;
        Damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
        this.statusTime = statusTime;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates,int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon target = plate.getSummon();
            if (target != null)
            {
                switch (statusType)
                {
                    case StatusType.None:
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) 전체 공격합니다.");
                        target.takeDamage(attacker.getSpecialAttackStrategy()[SpecialAttackArrayIndex].getSpecialDamage());
                        break;
                    case StatusType.Poison:
                        double poisonDamage = target.getMaxHP() * attacker.getSpecialAttackStrategy()[SpecialAttackArrayIndex].getSpecialDamage(); // 최대 체력의 10% 데미지
                        StatusEffect poisonEffect = new StatusEffect(StatusType.Poison, statusTime, poisonDamage);
                        target.ApplyStatusEffect(poisonEffect);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 중독 상태를 부여하여 매 턴 {poisonDamage} 데미지를 입힙니다.");
                        break;
                    case StatusType.Burn:
                        double burnDamage = target.getMaxHP() * attacker.getSpecialAttackStrategy()[SpecialAttackArrayIndex].getSpecialDamage(); // 최대 체력의 20% 데미지
                        StatusEffect burnEffect = new StatusEffect(StatusType.Burn, statusTime, burnDamage);
                        target.ApplyStatusEffect(burnEffect);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 화상을 입혀 매 턴 {burnDamage} 데미지를 입힙니다.");
                        break;
                    case StatusType.Upgrade:
                        double upgradeAttackPower = target.getAttackPower() * attacker.getSpecialAttackStrategy()[SpecialAttackArrayIndex].getSpecialDamage();
                        StatusEffect upgradeEffect = new StatusEffect(StatusType.Upgrade, statusTime, upgradeAttackPower);
                        target.ApplyStatusEffect(upgradeEffect);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 공격력 {upgradeAttackPower} 만큼 상승 시켰습니다.");
                        break;
                    case StatusType.Heal:
                        double healAmount = target.getMaxHP() * attacker.getSpecialAttackStrategy()[SpecialAttackArrayIndex].getSpecialDamage(); // 최대 체력의 20%만큼 회복
                        target.Heal(healAmount);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}을(를) {healAmount}만큼 치유했습니다.");
                        break;
                }

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
    public int getCooltime()
    {
        return cooltime;
    }

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
