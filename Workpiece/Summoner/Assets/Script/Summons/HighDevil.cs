using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighDevil : Summon
{
    private double HeavyAttakPower; //�� ���ݷ�

    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "HighDevil"; //�̸� �ϱ޾Ǹ�
        maxHP = 1000; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 400; //�Ϲݰ���
        HeavyAttakPower = 500;
        summonRank = SummonRank.Normal; //�Ϲ� �� ����

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1);
        // Ư�� ����: Ÿ�� ���� ����
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.Curse, 140,3), //��ü���� 140
            new TargetedAttackStrategy(StatusType.None,185,3) //Ÿ�ٰ��� 185
        };
    }

    private void Start()
    {
        Debug.Log("���� ü��: " + nowHP);
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
