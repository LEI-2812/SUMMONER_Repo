using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpirit : Summon
{
    protected override void Awake()
    {
        base.Awake();

        if (SummonDataApply(SummonDataGet()))
        {
            return;
        }

        summonName = "GrassSpirit"; //이름 풀정령
        maxHP = 350; //최대체력 200
        nowHP = maxHP; //현재체력 // 깨어날땐 최대체력으로 설정
        attackPower = 80; //일반공격
        heavyAttakPower = 110;
        summonRank = SummonRank.Normal; // 일반 소환수
        summonType = SummonType.GrassSpirit;

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Heal, 0.1, 3) }; //최대 체력의 0.1만큼 회복, 쿨타임 3턴
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
