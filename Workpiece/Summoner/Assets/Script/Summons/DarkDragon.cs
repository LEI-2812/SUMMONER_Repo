using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkDragon : Summon
{

    private void Awake()
    {
        summonName = "QueenSpirit"; //이름 슬라임
        maxHP = 3000; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 400; //일반공격
        heavyAttakPower = 500;
        summonRank = SummonRank.Boss; // 특급 소환수

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.None, 370, 0), //전체공격 데미지 370
            new AttackAllEnemiesStrategy(StatusType.Burn, 0.2, 5,2), //화상, 체력 20% 데미지, 쿨타임 5턴, 지속시간 2턴
            new TargetedAttackStrategy(StatusType.None, 450, 0), //저격, 데미지450
            new TargetedAttackStrategy(StatusType.LifeDrain, 0.2, 4, 2) //대상에게 흡혈, 쿨타임 4턴, 지속시간 2턴
        };
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
