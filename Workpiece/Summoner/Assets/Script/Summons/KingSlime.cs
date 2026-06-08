using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        summonName = "KingSlime"; //이름 슬라임
        maxHP = 250; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 50; //일반공격
        heavyAttakPower = 65;
        summonRank = SummonRank.Special; // 특급 적 몬스터
        summonType = SummonType.KingSlime;

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.None, 35, 1),//전체공격
            new TargetedAttackStrategy(StatusType.Shield, 80, 2)};//쉴드 
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
