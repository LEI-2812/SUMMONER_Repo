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
        attackPower = 170; //일반공격
        summonRank = SummonRank.Normal; // 일반 소환수

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower);
        // 특수 공격: 타겟 지정 공격
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.None, 180), new AttackAllEnemiesStrategy(StatusType.None, 140) };
    }

    private void Start()
    {
        Debug.Log("남은 체력: " + nowHP);
    }


    public override void attack()
    {
        /*
         * 일반공격과 강공격을 컨트롤시켜야할듯.
         */
        base.attack();
    }

    public override void die()
    {
        base.die();
    }

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }

    public override void takeSkill()
    {
        base.takeSkill();
        /*
         * 전체공격, 저격공격, 상태이상공격
         */
    }
}
