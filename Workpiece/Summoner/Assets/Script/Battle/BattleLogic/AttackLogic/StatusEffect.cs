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

    public StatusEffect(StatusType type, int time, double damage=0, Summon attacker = null)
    {
        statusType = type;
        effectTime = time;
        damagePerTurn = damage;
        this.attacker = attacker;
    }

    public void ApplyStatus(Summon target)
    {
        switch (statusType)
        {
            case StatusType.Stun: //공격을 막음
                target.CanAttack = false;
                break;

            case StatusType.Poison: // 중독
                target.ApplyDamage(damagePerTurn); // 즉시 데미지 적용
                effectTime -= 1; //데미지를 입히자마자 턴 하나를 줄임.
                break;

            case StatusType.Burn: //화상
                target.ApplyDamage(damagePerTurn); // 즉시 데미지 적용
                effectTime -= 1 ; //데미지를 입히자마자 턴 하나를 줄임.
                break;

            case StatusType.LifeDrain: //흡혈
                if (attacker != null) //즉시 사용
                {
                    target.ApplyDamage(damagePerTurn);
                    attacker.Heal(damagePerTurn);
                }
                break;

            case StatusType.Heal: //힐
                target.Heal(damagePerTurn); //즉시 사용
                break;

            default:
                Debug.Log("정의되지 않은 상태이상입니다.");
                break;
        }
    }
}
