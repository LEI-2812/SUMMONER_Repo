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
        summonRank = SummonRank.Normal; // �Ϲ� ��ȯ��

        // �Ϲ� ����: ���� ����� �� ����
        AttackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower);
        // Ư�� ����: Ÿ�� ���� ����
        specialAttackStrategies = new IAttackStrategy[] { new TargetedAttackStrategy(StatusType.None, 180), new AttackAllEnemiesStrategy(StatusType.None, 140) };
    }

    private void Start()
    {
        Debug.Log("���� ü��: " + nowHP);
    }


    public override void attack()
    {
        /*
         * �Ϲݰ��ݰ� �������� ��Ʈ�ѽ��Ѿ��ҵ�.
         */
        base.attack();
    }

    public override void die()
    {
        base.die();
    }

    public override void takeDamage(double damage)
    {
        base.takeDamage(damage);
    }

    public override void takeSkill()
    {
        base.takeSkill();
        /*
         * ��ü����, ���ݰ���, �����̻����
         */
    }
}
