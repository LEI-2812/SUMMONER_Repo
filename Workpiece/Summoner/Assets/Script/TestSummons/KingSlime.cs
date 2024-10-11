using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Summon
{
    private void Awake()
    {
        summonName = "KingSlime"; //�̸� ������
        maxHP = 250; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 50; //�Ϲݰ���
        summonRank = SummonRank.Special; // �Ϲ� ��ȯ��
    }

    public KingSlime()
    {
        summonName = "KingSlime"; //�̸� ������
        maxHP = 250; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 50; //�Ϲݰ���
        summonRank = SummonRank.Special; // �Ϲ� ��ȯ��
    }

    private void Start()
    {
        nowHP = 80; //�׽�Ʈ��
    }

    public override void attack()
    {
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
    }
}
