using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpirit : Summon
{

    private void Awake()
    {
        summonName = "FireSpirit"; //�̸� ������
        maxHP = 350; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 60; //�Ϲݰ���
        heavyAttakPower = 130;
        summonRank = SummonRank.Normal; // �Ϲ� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Upgrade, 0.1, 3, 1) }; //���ݷ��� 0.1��ŭ ��ȭ, ��Ÿ�� 1��, ���ӽð� 1��
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
