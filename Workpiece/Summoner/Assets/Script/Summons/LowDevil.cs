using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowDevil : Summon
{

    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "LowDevil"; //�̸� �ϱ޾Ǹ�
        maxHP = 700; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 170; //�Ϲݰ���
        summonRank = SummonRank.Normal; //�Ϲ� �� ����

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,1);
        // Ư�� ����: Ÿ�� ���� ����
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.Curse, 0 ,2,4) }; //Ÿ��, ������,����ð�, ��Ÿ��
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
