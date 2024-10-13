using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackInfo
{
    private IAttackStrategy attackStrategy;
    private int index;
    
    public SpecialAttackInfo(IAttackStrategy strategy, int index)
    {
        this.attackStrategy = strategy;
        this.index = index;
    }


    public IAttackStrategy getAttackInfoStrategy()
    {
        return attackStrategy;
    }
    public void setAttackInfoStrategy(IAttackStrategy strategy)
    {
        this.attackStrategy = strategy;
    }

    public int getAttackInfoIndex()
    {
        return index;
    }
    public void setAttackInfoIndex(int index)
    {
        this.index = index;
    }
}
