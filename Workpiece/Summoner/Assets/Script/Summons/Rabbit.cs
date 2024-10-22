using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rabbit : Summon
{
    private void Awake()
    {
        summonInitialize(5);
    }

    public void summonInitialize(int n)
    {
        summonName = "Rabbit";
        maxHP = 250 * n;
        nowHP = maxHP;
        attackPower = 20*n; //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수
        summonType = SummonType.Rabbit;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1); //근접공격
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Heal, 0.3, 3) };
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
