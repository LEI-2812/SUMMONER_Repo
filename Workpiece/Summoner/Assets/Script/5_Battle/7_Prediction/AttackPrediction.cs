using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 예측된 공격자, 공격 방식, 대상 플레이트, 공격 확률을 담는다.
public class AttackPrediction
{
    // 역할: 이 예측에서 일반 공격과 특수 공격이 선택될 가능성을 저장한다.
    private AttackProbability attackProbability;

    // 역할: 이번 예측에서 공격을 할 아군 소환수를 저장한다.
    private Summon attackSummon;

    // 역할: 공격하는 소환수가 올라가 있는 아군 플레이트 위치를 저장한다.
    private int attackSummonPlateIndex;

    // 역할: 예측된 공격이 실제로 사용할 공격 전략을 저장한다.
    private IAttackStrategy attackStrategy;

    // 역할: 여러 특수 공격 중 몇 번째 특수 공격을 쓸지 저장한다.
    private int specialAttackArrayIndex;

    // 역할: 공격 대상이 들어 있는 플레이트 목록을 저장한다.
    private List<Plate> targetPlate;

    // 역할: 대상 플레이트 목록 안에서 실제로 공격받을 위치를 저장한다.
    private int targetPlateIndex;

    // 역할: 예측 결과를 만들 때 필요한 공격자, 공격 방식, 대상, 확률 정보를 한 번에 저장한다.
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

    public IAttackStrategy GetAttackStrategy()
    {
        return attackStrategy;
    }

    public void SetAttackStrategy(IAttackStrategy attackStrategy)
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


    public List<Plate> GetTargetPlate()
    {
        return targetPlate;
    }

    public void SetTargetPlate(List<Plate> targetPlate)
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

    public AttackProbability GetAttackProbability()
    {
        return attackProbability;
    }

    public void SetAttackProbability(AttackProbability attackProbability)
    {
        this.attackProbability = attackProbability;
    }
}
