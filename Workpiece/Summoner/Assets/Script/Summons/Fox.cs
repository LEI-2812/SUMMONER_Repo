using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Summon
{

    public override void summonInitialize()
    {
        summonName = "Fox"; //이름 Fox
        maxHP = 250; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 35; //일반공격
        summonRank = SummonRank.Low; // 하급 소환수
        summonType = SummonType.Fox;
        ApplayMultiple(multiple);

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Upgrade, 0.3, 3, 1) };//공격력 강화, 20% 상승, 쿨타임 3턴 //지속시간 1턴

    }

    public override void ApplayMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //일반공격
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
