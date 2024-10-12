using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Slime"; //�̸� ������
        maxHP = 200; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 25; //�Ϲݰ���
        SpecialPower = 40; //������
        summonRank = SummonRank.Normal; // �Ϲ� ��ȯ��
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
