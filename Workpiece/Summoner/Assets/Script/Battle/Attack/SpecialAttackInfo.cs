using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    NormalAttack, SpecialAttack
}
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
