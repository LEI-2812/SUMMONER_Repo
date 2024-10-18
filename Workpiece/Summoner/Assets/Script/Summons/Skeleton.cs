using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Summon
{

    private void Awake()
    {
        summonName = "Skeleton"; //�̸� ���̷���
        maxHP = 650; //�ִ�ü�� 650
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 150; //�Ϲݰ���
        heavyAttakPower = 170;
        summonRank = SummonRank.Normal; // Ư�� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0); //��������
        specialAttackStrategies = new IAttackStrategy[] {
            new TargetedAttackStrategy(StatusType.None, 160,0) //Ÿ�ٰ��� 160������
        };
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
