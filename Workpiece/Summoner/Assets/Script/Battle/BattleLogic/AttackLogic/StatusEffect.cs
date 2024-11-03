using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum StatusType
{
    None, //단순 공격
    Stun, //혼란
    Poison, //중독
    Heal, //힐
    Curse, //저주
    LifeDrain, //흡혈
    Burn, //화상
    Shield, //보호막
    Upgrade, //강화
    OnceInvincibility //1회 무적
}

public class StatusEffect
{
    public StatusType statusType { get; private set; }
    public int effectTime { get; set; } // 지속 시간
    public double damagePerTurn { get; set; } // 매 턴마다 줄 데미지

    private Summon attacker; // 공격자

    private bool applyOnce = false;

    private double originAttack;
    public StatusEffect(StatusType type, int eTime, double damage=0, Summon attacker = null)
    {
        statusType = type;
        effectTime = eTime;
        damagePerTurn = damage;
        this.attacker = attacker;
        applyOnce = false;
    }


    //덮어씌어지는 것에 대해서만 넣기
    public void ApplyStatus(Summon target)
    {
        switch (statusType)
        {
            case StatusType.Stun: //공격 불가능
                target.setIsAttack(false);
                break;

            case StatusType.Poison: // 중독
                target.takeDamage(damagePerTurn); // 즉시 데미지 적용
                effectTime -= 1; //데미지를 입히자마자 턴 하나를 줄임.
                break;

            case StatusType.Burn: //화상
                target.takeDamage(damagePerTurn); // 즉시 데미지 적용
                effectTime -= 1 ; //데미지를 입히자마자 턴 하나를 줄임.
                break;

            case StatusType.LifeDrain: //흡혈
                if (target != null) //즉시 사용
                {
                    target.takeDamage(damagePerTurn);
                    attacker.Heal(damagePerTurn);
                    Debug.Log($"{target.getSummonName()}에게서 {damagePerTurn} 만큼 흡혈합니다. 현재체력: {attacker.getNowHP()}");
                }
                else
                {
                    Debug.Log("타겟이 없거나 죽어서 흡혈이 안됩니다.");
                }
                break;

            case StatusType.Shield: //쉴드
                target.AddShield(damagePerTurn);
                break;

            case StatusType.Upgrade: //강화
                target.UpgradeAttackPower(damagePerTurn);
                break;
            case StatusType.Curse: //저주
                target.Cursed(damagePerTurn);
                break;

            default:
                Debug.Log("정의되지 않은 상태이상입니다.");
                break;
        }
    }

    public double getOriginAttack()
    {
        return originAttack;
    }
    public void setOriginAttack(double originAttack)
    {
        this.originAttack = originAttack;
    }

    public Summon getAttacker()
    {
        return attacker;
    }
    public void setAttacker(Summon summon)
    {
        attacker = summon;
    }

    public bool shouldApplyOnce()
    {
        return damagePerTurn > 0 && !applyOnce;
    }

    public void setApplyOnce()
    {
        applyOnce = true; // 한 번만 적용되도록 표시
    }
}
