using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Start()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Snake";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 40; //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수
        summonType = SummonType.Snake;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //근접 공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0.2, 3, 3) };//중독, 체력에20% 쿨타임3턴 지속시간3턴

        ApplyMultiple(multiple);
    }

    public void ApplyMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //일반공격
        heavyAttakPower = (int)(heavyAttakPower * multiple);
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
