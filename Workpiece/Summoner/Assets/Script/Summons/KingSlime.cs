using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "KingSlime"; //�̸� ������
        maxHP = 250; //�ִ�ü�� 200
        nowHP = maxHP; //����ü�� // ����� �ִ�ü������ ����
        attackPower = 50; //�Ϲݰ���
        heavyAttakPower = 65;
        summonRank = SummonRank.Special; // Ư�� �� ����

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower, 0);
        specialAttackStrategies = new IAttackStrategy[] {
            new AttackAllEnemiesStrategy(StatusType.None, 35, 1),//��ü����
            new TargetedAttackStrategy(StatusType.Shield, 80, 2)};//���� 
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
