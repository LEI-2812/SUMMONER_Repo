using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowDevil : Summon
{


    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "LowDevil"; //이름 하급악마
        maxHP = 700; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 180; //일반공격
        heavyAttakPower = 220;
        summonRank = SummonRank.Normal; //일반 적 몬스터

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,0);
        // 특수 공격: 타겟 지정 공격
        specialAttackStrategies = new IAttackStrategy[] {
            new TargetedAttackStrategy(StatusType.Curse, 0.1 ,4,1),//저주, 공격력의 10% 감소, 쿨타임 4턴, 적용 1턴
            new TargetedAttackStrategy(StatusType.OnceInvincibility,0,2) //1번 무적, 쿨타임 2턴
        }; 
    }

    private void Start()
    {
        Debug.Log("남은 체력: " + nowHP);
    }


    public override void die()
    {
        base.die();
    }

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }


}
