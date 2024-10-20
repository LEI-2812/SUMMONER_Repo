using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrediction
{
    // �Ϲ� ���ݰ� Ư�� ������ Ȯ�� ����
    private AttackProbability attackProbability;

    // �����ϴ� ��ȯ�� ����
    private Summon attackSummon;
    private int attackSummonPlateIndex;
    private IAttackStrategy attackStrategy;
    private int specialAttackArrayIndex;

    // ���ݹ��� �÷���Ʈ ����
    private List<Plate> targetPlate;
    private int targetPlateIndex;

    // ������
    public AttackPrediction(
        Summon attackSummon, //������
        int attackSummonPlateIndex, //������ �ڽ��� �÷���Ʈ ��ȣ
        IAttackStrategy attackStrategy, //���� ����
        int specialAttackArrayIndex, //Ư������ �ε���
        List<Plate> targetPlate, //Ÿ���� �÷���Ʈ
        int targetPlateIndex, //Ÿ���� �÷���Ʈ ��ȣ
        AttackProbability attackProbability) //Ȯ��
    {
        this.attackSummon = attackSummon;
        this.attackSummonPlateIndex = attackSummonPlateIndex;
        this.attackStrategy = attackStrategy;
        this.specialAttackArrayIndex = specialAttackArrayIndex;
        this.targetPlate = targetPlate;
        this.targetPlateIndex = targetPlateIndex;
        this.attackProbability = attackProbability;
    }

    // Getter �� Setter �޼ҵ�
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
