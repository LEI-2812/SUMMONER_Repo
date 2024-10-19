using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrediction
{
    private AttackProbability attackProbability;
    private List<Plate> targetPlate;
    private int attackPlateIndex;
    private int specialAttackArrayIndex;



    public AttackPrediction(List<Plate> targetPlate ,AttackProbability probability, int attackIndex=0, int specialIndex=0)
    {
        this.targetPlate = targetPlate;
        this.attackProbability = probability;
        this.attackPlateIndex = attackIndex;
        this.specialAttackArrayIndex = specialIndex;
    }




    public AttackProbability getAttackProbability()
    {
        return attackProbability;
    }

    public void setAttackProbability(AttackProbability attackProbability)
    {
        this.attackProbability = attackProbability;
    }


    public int getAttackPlateIndex()
    {
        return attackPlateIndex;
    }

    public void setAttackPlateIndex(int attackPlateIndex)
    {
        this.attackPlateIndex = attackPlateIndex;
    }


    public int getSpecialAttackArrayIndex()
    {
        return specialAttackArrayIndex;
    }

    public void setSpecialAttackArrayIndex(int specialAttackArrayIndex)
    {
        this.specialAttackArrayIndex = specialAttackArrayIndex;
    }



}
