using System.Collections.Generic;
using UnityEngine;

public class Cat : Summon
{

    public override void summonInitialize()
    {
        summonName = "Cat";
        maxHP = 250;
        nowHP = maxHP;
        attackPower = 30; //일반공격
        summonRank = SummonRank.Low; // 하급 소환수
        summonType = SummonType.Cat;
        heavyAttakPower = 40;

        ApplayMultiple(multiple);

        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower ,1);
        // 특수 공격: 전체 적 공격
        specialAttackStrategies = new IAttackStrategy[] { new ClosestEnemyAttackStrategy(StatusType.None, heavyAttakPower, 1) }; //근접공격, 20데미지, 쿨타임1턴

    }

    public override void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (!SpecialAttackIndexCheck(SpecialAttackArrayIndex))
        {
            Debug.Log("유효하지 않은 특수 공격 인덱스입니다.");
            return;
        }

        var specialAttack = specialAttackStrategies[SpecialAttackArrayIndex];

        if (!AttackCanUse(specialAttack))
        {
            Debug.Log("특수 스킬이 쿨타임 중입니다.");
            return;
        }


        double originAttackPower = attackPower;
        attackPower = heavyAttakPower;

        specialAttack.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);
        AttackMotionPlay(false);
        AttackCooldownApply(specialAttack);
        attackPower = originAttackPower;
        isAttack = false;
    }

    public override void ApplayMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //일반공격
        heavyAttakPower = (int)(heavyAttakPower * multiple);
    }

}
