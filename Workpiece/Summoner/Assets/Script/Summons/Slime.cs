using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Summon
{
    private void Awake()
    {
        summonName = "Slime"; //이름 슬라임
        maxHP = 200; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 25; //일반공격
        SpecialPower = 40; //강공격
        summonRank = SummonRank.Normal; // 일반 소환수
    }

    private void Start()
    {
        nowHP = 100;
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
