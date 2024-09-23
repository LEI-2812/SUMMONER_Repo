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
        nowHP = 80; //테스트용
    }

    public override void attack()
    {
        base.attack();
    }

    public override void die()
    {
        base.die();
    }

    public override void takeDamage(int damage)
    {
        base.takeDamage(damage);
    }

    public override void takeSkill()
    {
        base.takeSkill();
    }
}
