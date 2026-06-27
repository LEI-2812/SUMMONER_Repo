using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: AttackPrediction의 책임을 정의한다.
public class AttackPredictionData
{
    private AttackProbabilityData AttackProbabilityData;

    private Summon attackSummon;

    private int attackSummonPlateIndex;

    private AttackData attackStrategy;

    private int specialAttackArrayIndex;

    private IReadOnlyList<IPlateState> targetPlate;

    private int targetPlateIndex;

    public AttackPredictionData(
        Summon attackSummon, // 공격하는 소환수
        int attackSummonPlateIndex,
        AttackData attackStrategy,
        int specialAttackArrayIndex,
        IReadOnlyList<IPlateState> targetPlate, // 공격 대상 플레이트
        int targetPlateIndex,
        AttackProbabilityData AttackProbabilityData)
    {
        this.attackSummon = attackSummon;
        this.attackSummonPlateIndex = attackSummonPlateIndex;
        this.attackStrategy = attackStrategy;
        this.specialAttackArrayIndex = specialAttackArrayIndex;
        this.targetPlate = targetPlate;
        this.targetPlateIndex = targetPlateIndex;
        this.AttackProbabilityData = AttackProbabilityData;
    }

    public Summon GetAttackSummon()
    {
        return attackSummon;
    }

    public void SetAttackSummon(Summon attackSummon)
    {
        this.attackSummon = attackSummon;
    }

    public int GetAttackSummonPlateIndex()
    {
        return attackSummonPlateIndex;
    }

    public void SetAttackSummonPlateIndex(int attackSummonPlateIndex)
    {
        this.attackSummonPlateIndex = attackSummonPlateIndex;
    }

    public AttackData GetAttackStrategy()
    {
        return attackStrategy;
    }

    public void SetAttackStrategy(AttackData attackStrategy)
    {
        this.attackStrategy = attackStrategy;
    }

    public int GetSpecialAttackArrayIndex()
    {
        return specialAttackArrayIndex;
    }

    public void SetSpecialAttackArrayIndex(int specialAttackArrayIndex)
    {
        this.specialAttackArrayIndex = specialAttackArrayIndex;
    }


    public IReadOnlyList<IPlateState> GetTargetPlate()
    {
        return targetPlate;
    }

    public void SetTargetPlate(IReadOnlyList<IPlateState> targetPlate)
    {
        this.targetPlate = targetPlate;
    }

    public int GetTargetPlateIndex()
    {
        return targetPlateIndex;
    }

    public void SetTargetPlateIndex(int targetPlateIndex)
    {
        this.targetPlateIndex = targetPlateIndex;
    }

    public AttackProbabilityData GetAttackProbability()
    {
        return AttackProbabilityData;
    }

    public void SetAttackProbability(AttackProbabilityData AttackProbabilityData)
    {
        this.AttackProbabilityData = AttackProbabilityData;
    }
}
