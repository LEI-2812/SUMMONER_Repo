using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: AttackType 값의 종류를 정의한다.
public enum AttackType
{
    NormalAttack, SpecialAttack
}
// 역할: SpecialAttackInfo의 책임을 정의한다.
public class SpecialAttackInfo
{
    private AttackData attackStrategy;
    private int index;
    
    public SpecialAttackInfo(AttackData strategy, int index)
    {
        this.attackStrategy = strategy;
        this.index = index;
    }


    public AttackData GetAttackInfoStrategy()
    {
        return attackStrategy;
    }
    public void SetAttackInfoStrategy(AttackData strategy)
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
