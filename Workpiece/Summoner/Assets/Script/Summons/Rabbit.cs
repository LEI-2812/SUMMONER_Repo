using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rabbit : Summon
{

    private void Start()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Rabbit";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 37; //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수
        summonType = SummonType.Rabbit;

        ApplayMultiple(multiple);

        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1); //근접공격
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Heal, 0.3, 3) };

    }

    public override void ApplayMultiple(double multiple)
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
