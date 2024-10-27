using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Summon
{
    private void Awake()
    {
        summonInitialize(1);
    }

    public override void summonInitialize(double n)
    {
        summonName = "Cat";
        maxHP = (int)100 * n;
        nowHP = maxHP;
        attackPower = (int)15 * n; //일반공격
        summonRank = SummonRank.Low; // 하급 소환수
        summonType = SummonType.Cat;
        heavyAttakPower = (int)20 * n;
        // 일반 공격: 가장 가까운 적 공격
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower ,1);
        // 특수 공격: 전체 적 공격
        specialAttackStrategies = new IAttackStrategy[] { new ClosestEnemyAttackStrategy(StatusType.None, heavyAttakPower, 1) }; //근접공격, 20데미지, 쿨타임1턴
    }

    //일반 공격과 특수공격이 같은 방식의 경우 데미지가 attackPower로 들어가는 로직이기 때문에 잠깐 SpecialPower로 하고 되돌리게
    public override void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (SpecialAttackArrayIndex < 0 || SpecialAttackArrayIndex >= specialAttackStrategies.Length)
        {
            Debug.Log("유효하지 않은 특수 공격 인덱스입니다.");
            return;
        }

        var specialAttack = specialAttackStrategies[SpecialAttackArrayIndex];

        if (specialAttack == null || specialAttack.getCurrentCooldown() > 0)
        {
            Debug.Log("특수 스킬이 쿨타임 중입니다.");
            return;
        }

        
        double originAttackPower = attackPower;
        attackPower = 20;
        // 공격 수행
        specialAttack.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);

        // 해당 공격에 쿨타임 적용
        specialAttack.ApplyCooldown();
        attackPower = originAttackPower;
        isAttack = false;
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
