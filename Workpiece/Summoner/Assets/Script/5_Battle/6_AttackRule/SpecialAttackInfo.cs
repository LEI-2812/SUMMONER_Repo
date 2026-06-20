using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 공격 종류를 구분하기 위한 값이다.
public enum AttackType
{
    NormalAttack, SpecialAttack
}
// 역할: 현재 선택된 특수 공격 전략과 배열 인덱스를 함께 보관한다.
public class SpecialAttackInfo
{
    private IAttackStrategy attackStrategy;
    private int index;
    
    public SpecialAttackInfo(IAttackStrategy strategy, int index)
    {
        this.attackStrategy = strategy;
        this.index = index;
    }


    public IAttackStrategy GetAttackInfoStrategy()
    {
        return attackStrategy;
    }
    public void SetAttackInfoStrategy(IAttackStrategy strategy)
    {
        this.attackStrategy = strategy;
    }

    public int GetAttackInfoIndex()
    {
        return index;
    }
    public void SetAttackInfoIndex(int index)
    {
        this.index = index;
    }
}
