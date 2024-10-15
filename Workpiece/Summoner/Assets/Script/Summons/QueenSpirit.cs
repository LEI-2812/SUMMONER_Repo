using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenSpirit : Summon
{

    private void Awake()
    {
        summonName = "QueenSpirit"; //�̸� ������
        maxHP = 400; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 100; //�Ϲݰ���
        heavyAttakPower = 140;
        summonRank = SummonRank.Special; // Ư�� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.None, 70, 0), //��ü���� ������ 70
            new AttackAllEnemiesStrategy(StatusType.Heal, 0.2, 3), //�Ʊ� ��ü 20% ȸ�� ��Ÿ�� 3��
            new TargetedAttackStrategy(StatusType.Stun,0,3,1) //��󿡰� ȥ��, ��Ÿ�� 3��, ���ӽð� 1��
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
