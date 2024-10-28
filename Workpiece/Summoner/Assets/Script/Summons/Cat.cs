using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Summon
{
    private void Awake()
    {
        summonInitialize();
    }

    public override void summonInitialize()
    {
        summonName = "Cat";
        maxHP = 100;
        nowHP = maxHP;
        attackPower = 15; //�Ϲݰ���
        summonRank = SummonRank.Low; // �ϱ� ��ȯ��
        summonType = SummonType.Cat;
        heavyAttakPower = 20;

        ApplayMultiple(multiple);

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower ,1);
        // Ư�� ����: ��ü �� ����
        specialAttackStrategies = new IAttackStrategy[] { new ClosestEnemyAttackStrategy(StatusType.None, heavyAttakPower, 1) }; //��������, 20������, ��Ÿ��1��

    }

    public override void ApplayMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //�Ϲݰ���
        heavyAttakPower = (int)(heavyAttakPower * multiple);
    }

    //�Ϲ� ���ݰ� Ư�������� ���� ����� ��� �������� attackPower�� ���� �����̱� ������ ��� SpecialPower�� �ϰ� �ǵ�����
    public override void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (SpecialAttackArrayIndex < 0 || SpecialAttackArrayIndex >= specialAttackStrategies.Length)
        {
            Debug.Log("��ȿ���� ���� Ư�� ���� �ε����Դϴ�.");
            return;
        }

        var specialAttack = specialAttackStrategies[SpecialAttackArrayIndex];

        if (specialAttack == null || specialAttack.getCurrentCooldown() > 0)
        {
            Debug.Log("Ư�� ��ų�� ��Ÿ�� ���Դϴ�.");
            return;
        }

        
        double originAttackPower = attackPower;
        attackPower = 20;
        // ���� ����
        specialAttack.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);

        // �ش� ���ݿ� ��Ÿ�� ����
        specialAttack.ApplyCooldown();
        attackPower = originAttackPower;
        isAttack = false;
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
