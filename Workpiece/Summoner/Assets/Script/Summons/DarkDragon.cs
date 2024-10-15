using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkDragon : Summon
{

    private void Awake()
    {
        summonName = "QueenSpirit"; //�̸� ������
        maxHP = 3000; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 400; //�Ϲݰ���
        heavyAttakPower = 500;
        summonRank = SummonRank.Boss; // Ư�� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.None, 370, 0), //��ü���� ������ 370
            new AttackAllEnemiesStrategy(StatusType.Burn, 0.2, 5,2), //ȭ��, ü�� 20% ������, ��Ÿ�� 5��, ���ӽð� 2��
            new TargetedAttackStrategy(StatusType.None, 450, 0), //����, ������450
            new TargetedAttackStrategy(StatusType.LifeDrain, 0.2, 4, 2) //��󿡰� ����, ��Ÿ�� 4��, ���ӽð� 2��
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
