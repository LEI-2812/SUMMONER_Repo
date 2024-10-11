using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Summon
{
    private void Awake()
    {
        summonName = "KingSlime"; //이름 슬라임
        maxHP = 250; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 50; //일반공격
        summonRank = SummonRank.Special; // 일반 소환수
    }

    public KingSlime()
    {
        summonName = "KingSlime"; //이름 슬라임
        maxHP = 250; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 50; //일반공격
        summonRank = SummonRank.Special; // 일반 소환수
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

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }

    public override void takeSkill()
    {
        base.takeSkill();
    }
}
