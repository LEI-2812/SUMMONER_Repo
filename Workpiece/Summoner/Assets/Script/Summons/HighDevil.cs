using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighDevil : Summon
{

    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "HighDevil"; //�̸� �ϱ޾Ǹ�
        maxHP = 1000; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 200; //�Ϲݰ���
        heavyAttakPower = 250;
        summonRank = SummonRank.Special; //�Ϲ� �� ����

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,0);
        // Ư�� ����: Ÿ�� ���� ����
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.None, 140,0), //��ü���� 140
            new TargetedAttackStrategy(StatusType.None,230,0) //Ÿ�ٰ��� 230
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
