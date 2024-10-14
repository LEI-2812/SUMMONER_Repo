using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Awake()
    {
        summonName = "Snake";
        maxHP = 200;
        nowHP = maxHP;
        attackPower = 30; //일반공격
        specialPower = 25;
        summonRank = SummonRank.Medium; // 중급 소환수

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1); //근접 공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0,3,3) };//상태이상 (중독)공격
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
