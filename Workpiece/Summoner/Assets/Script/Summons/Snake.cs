using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Summon
{
    private void Start()
    {
        SummonInitialize();
    }

    public override void SummonInitialize()
    {
        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        summonName = "Snake";
        maxHP = 300;
        nowHP = maxHP;
        attackPower = 40; //일반공격
        summonRank = SummonRank.Medium; // 중급 소환수
        summonType = SummonType.Snake;
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 1); //근접 공격
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Poison, 0.1, 3, 2) };//중독, 체력에20% 쿨타임3턴 지속시간3턴

        ApplyMultiple(multiple);
    }

    public void ApplyMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //일반공격
        heavyAttakPower = (int)(heavyAttakPower * multiple);
    }


    public override void Die()
    {
        base.Die();
    }

    public override void TakeDamage(double damage)
    {
        base.TakeDamage(damage);
    }

}
