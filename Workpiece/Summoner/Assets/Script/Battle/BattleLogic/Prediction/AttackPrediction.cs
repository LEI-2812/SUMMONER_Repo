using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrediction
{
    // 일반 공격과 특수 공격의 확률 정보
    private AttackProbability attackProbability;

    // 공격하는 소환수 정보
    private Summon attackSummon;
    private int attackSummonPlateIndex;
    private IAttackStrategy attackStrategy;
    private int specialAttackArrayIndex;

    // 공격받을 플레이트 정보
    private List<Plate> targetPlate;
    private int targetPlateIndex;

    // 생성자
    public AttackPrediction(
        Summon attackSummon, //공격자
        int attackSummonPlateIndex, //공격자 자신의 플레이트 번호
        IAttackStrategy attackStrategy, //공격 유형
        int specialAttackArrayIndex, //특수공격 인덱스
        List<Plate> targetPlate, //타겟의 플레이트
        int targetPlateIndex, //타겟의 플레이트 번호
        AttackProbability attackProbability) //확률
    {
        this.attackSummon = attackSummon;
        this.attackSummonPlateIndex = attackSummonPlateIndex;
        this.attackStrategy = attackStrategy;
        this.specialAttackArrayIndex = specialAttackArrayIndex;
        this.targetPlate = targetPlate;
        this.targetPlateIndex = targetPlateIndex;
        this.attackProbability = attackProbability;
    }

    // Getter 및 Setter 메소드
    public Summon getAttackSummon()
    {
        return attackSummon;
    }

    public void setAttackSummon(Summon attackSummon)
    {
        this.attackSummon = attackSummon;
    }

    public int getAttackSummonPlateIndex()
    {
        return attackSummonPlateIndex;
    }

    public void setAttackSummonPlateIndex(int attackSummonPlateIndex)
    {
        this.attackSummonPlateIndex = attackSummonPlateIndex;
    }

    public IAttackStrategy getAttackStrategy()
    {
        return attackStrategy;
    }

    public void setAttackStrategy(IAttackStrategy attackStrategy)
    {
        this.attackStrategy = attackStrategy;
    }

    public int getSpecialAttackArrayIndex()
    {
        return specialAttackArrayIndex;
    }

    public void setSpecialAttackArrayIndex(int specialAttackArrayIndex)
    {
        this.specialAttackArrayIndex = specialAttackArrayIndex;
    }


    public List<Plate> getTargetPlate()
    {
        return targetPlate;
    }

    public void setTargetPlate(List<Plate> targetPlate)
    {
        this.targetPlate = targetPlate;
    }

    public int getTargetPlateIndex()
    {
        return targetPlateIndex;
    }

    public void setTargetPlateIndex(int targetPlateIndex)
    {
        this.targetPlateIndex = targetPlateIndex;
    }

    public AttackProbability getAttackProbability()
    {
        return attackProbability;
    }

    public void setAttackProbability(AttackProbability attackProbability)
    {
        this.attackProbability = attackProbability;
    }
}
