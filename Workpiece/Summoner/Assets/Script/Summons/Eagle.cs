using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Eagle";
        maxHP = 400;
        nowHP = maxHP;
        attackPower = 45; //일반공격
        SpecialPower = 30; //특수공격
        summonRank = SummonRank.High; // 상급 소환수

        // 일반 공격: 가장 가까운 적 공격
        AttackStrategy = new ClosestEnemyAttackStrategy();
        // 특수 공격: 타겟 지정 공격
        SpecialAttackStrategy = new TargetedAttackStrategy();
    }

    void Start()
    {
        Debug.Log("공격력" + attackPower);
    }


    public override void attack()
    {

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
    }
}
