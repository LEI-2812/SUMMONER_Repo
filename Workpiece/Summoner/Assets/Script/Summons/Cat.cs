using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cat : Summon
{

    public override void summonInitialize()
    {
        summonName = "Cat";
        maxHP = 250;
        nowHP = maxHP;
        attackPower = 30; //�Ϲݰ���
        summonRank = SummonRank.Low; // �ϱ� ��ȯ��
        summonType = SummonType.Cat;
        heavyAttakPower = 40;

        ApplayMultiple(multiple);

        // �Ϲ� ����: ���� ����� �� ����
        attackStrategy = new ClosestEnemyAttackStrategy(StatusType.None, attackPower ,1);
        // Ư�� ����: ��ü �� ����
        specialAttackStrategies = new IAttackStrategy[] { new ClosestEnemyAttackStrategy(StatusType.None, heavyAttakPower, 1) }; //��������, 20������, ��Ÿ��1��

    }

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
        attackPower = heavyAttakPower;

        specialAttack.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);
        animator.SetTrigger("attack");
        StartCoroutine(ColorChange(1)); // ������
        specialAttack.ApplyCooldown();
        attackPower = originAttackPower;
        isAttack = false;
    }

    public override void ApplayMultiple(double multiple)
    {
        maxHP = (int)(maxHP * multiple);
        nowHP = maxHP;
        attackPower = (int)(attackPower * multiple); //�Ϲݰ���
        heavyAttakPower = (int)(heavyAttakPower * multiple);
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
