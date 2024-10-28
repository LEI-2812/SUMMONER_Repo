using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Snake";
        maxHP = 200;
        nowHP = maxHP;
        attackPower = 30; //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수
        summonType = SummonType.Snake;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //근접 공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0.1, 3, 3) };//중독, 체력에10% 쿨타임3턴 지속시간3턴

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
