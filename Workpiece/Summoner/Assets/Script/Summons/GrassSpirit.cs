using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpirit : Summon
{

    private void Awake()
    {
        summonName = "GrassSpirit"; //�̸� Ǯ����
        maxHP = 350; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 80; //�Ϲݰ���
        heavyAttakPower = 110;
        summonRank = SummonRank.Normal; // �Ϲ� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] { new AttackAllEnemiesStrategy(StatusType.Heal, 0.1, 3) }; //�ִ� ü���� 0.1��ŭ ȸ��, ��Ÿ�� 3��
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
