using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Awake()
    {
        summonInitialize(5);
    }

    public override void summonInitialize(double n)
    {
        n = 5;
        summonName = "Snake";
        maxHP = (int)(200 * n);
        nowHP = maxHP;
        attackPower = (int)(30 * n); //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수
        summonType = SummonType.Snake;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //근접 공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0.1, 3, 3) };//중독, 체력에10% 쿨타임3턴 지속시간3턴
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
