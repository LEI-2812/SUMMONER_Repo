using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighDevil : Summon
{
    private double HeavyAttakPower; //강 공격력

    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "HighDevil"; //이름 하급악마
        maxHP = 1000; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 400; //일반공격
        HeavyAttakPower = 500;
        summonRank = SummonRank.Normal; //일반 적 몬스터

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1);
        // 특수 공격: 타겟 지정 공격
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.Curse, 140,3), //전체공격 140
            new TargetedAttackStrategy(StatusType.None,185,3) //타겟공격 185
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
