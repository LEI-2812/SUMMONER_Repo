using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAttackStrategy : IAttackStrategy
{
    private StatusType statusType; //상태이상 종류
    private int statusTime; //적용시간(턴)

    public StatusAttackStrategy(StatusType statusType, int time)
    {
        this.statusType = statusType;
        this.statusTime = time;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon target = plate.getSummon();
            if (target != null)
            {
                switch (statusType)
                {
                    case StatusType.Poison:
                        double poisonDamage = target.MaxHP * 0.1; // 최대 체력의 10% 데미지
                        StatusEffect poisonEffect = new StatusEffect(StatusType.Poison, statusTime, poisonDamage);
                        target.ApplyStatusEffect(poisonEffect);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 중독 상태를 부여하여 매 턴 {poisonDamage} 데미지를 입힙니다.");
                        break;

                    case StatusType.Stun:
                        StatusEffect stunEffect = new StatusEffect(StatusType.Stun , statusTime);
                        target.ApplyStatusEffect(stunEffect);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 스턴 상태를 부여했습니다.");
                        break;

                    case StatusType.Curse:
                        StatusEffect curseEffect = new StatusEffect(StatusType.Curse , statusTime);
                        target.ApplyStatusEffect(curseEffect);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 저주 상태를 부여하여 공격력을 감소시킵니다.");
                        break;

                    case StatusType.Burn:
                        double burnDamage = target.MaxHP * 0.2; // 최대 체력의 20% 데미지
                        StatusEffect burnEffect = new StatusEffect(StatusType.Burn, statusTime, burnDamage);
                        target.ApplyStatusEffect(burnEffect);
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 화상을 입혀 매 턴 {burnDamage} 데미지를 입힙니다.");
                        break;

                    case StatusType.LifeDrain:
                        double lifeDrainDamage = target.MaxHP * 0.1;
                        StatusEffect lifeDrainEffect = new StatusEffect(StatusType.LifeDrain, statusTime, lifeDrainDamage);
                        target.ApplyStatusEffect(lifeDrainEffect);
                        attacker.Heal(lifeDrainDamage); // 흡혈한 만큼 체력 회복
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}에게 흡혈을 사용하여 {lifeDrainDamage} 데미지를 입히고 회복합니다.");
                        break;

                    case StatusType.Heal:
                        double heal = target.MaxHP * 0.3;
                        target.Heal(heal); // 체력 회복
                        Debug.Log($"{attacker.getSummonName()}이(가) {target.getSummonName()}의 체력을 회복합니다.");
                        break;

                    default:
                        Debug.Log($"{statusType} 상태이상이 정의되지 않았습니다.");
                        break;
                }
            }
        }
    }

    public StatusType getStatusType()
    {
        return statusType;
    }

    public void setStatusType(StatusType type)
    {
        this.statusType = type;
    }
}
