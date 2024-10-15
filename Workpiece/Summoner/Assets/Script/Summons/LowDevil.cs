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
        attackPower = 180; //�Ϲݰ���
        heavyAttakPower = 220;
        summonRank = SummonRank.Normal; //�Ϲ� �� ����

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower,0);
        // Ư�� ����: Ÿ�� ���� ����
        specialAttackStrategies = new IAttackStrategy[] {
            new TargetedAttackStrategy(StatusType.Curse, 0.1 ,4,1),//����, ���ݷ��� 10% ����, ��Ÿ�� 4��, ���� 1��
            new TargetedAttackStrategy(StatusType.OnceInvincibility,0,2) //1�� ����, ��Ÿ�� 2��
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
