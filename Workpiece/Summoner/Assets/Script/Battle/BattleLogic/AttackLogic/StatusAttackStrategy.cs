using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAttackStrategy : IAttackStrategy
{
    private StatusType statusType; //�����̻� ����
    private int statusTime; //����ð�(��)
    private double damage;
    private int cooltime; //��ų ��Ÿ��
    private int currentCooldown;
    public StatusAttackStrategy(StatusType statusType, int time, int cooltime)
    {
        this.statusType = statusType;
        this.statusTime = time;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackarrayIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon target = plate.getSummon();
            if (target != null)
            {
                switch (statusType)
                {
                    case StatusType.Poison: //�ߵ�
                        double poisonDamage = target.getMaxHP() * 0.1; // �ִ� ü���� 10% ������
                        StatusEffect poisonEffect = new StatusEffect(StatusType.Poison, statusTime, poisonDamage);
                        target.ApplyStatusEffect(poisonEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� �ߵ� ���¸� �ο��Ͽ� �� �� {poisonDamage} �������� �����ϴ�.");
                        break;

                    case StatusType.Stun: //ȥ��
                        StatusEffect stunEffect = new StatusEffect(StatusType.Stun , statusTime);
                        target.ApplyStatusEffect(stunEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ���� ���¸� �ο��߽��ϴ�.");
                        break;

                    case StatusType.Curse: //����
                        StatusEffect curseEffect = new StatusEffect(StatusType.Curse , statusTime);
                        target.ApplyStatusEffect(curseEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ���� ���¸� �ο��Ͽ� ���ݷ��� ���ҽ�ŵ�ϴ�.");
                        break;

                    case StatusType.Burn: //ȭ��
                        double burnDamage = target.getMaxHP() * 0.2; // �ִ� ü���� 20% ������
                        StatusEffect burnEffect = new StatusEffect(StatusType.Burn, statusTime, burnDamage);
                        target.ApplyStatusEffect(burnEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ȭ���� ���� �� �� {burnDamage} �������� �����ϴ�.");
                        break;

                    case StatusType.LifeDrain: //����
                        double lifeDrainDamage = target.getMaxHP() * 0.1;
                        StatusEffect lifeDrainEffect = new StatusEffect(StatusType.LifeDrain, statusTime, lifeDrainDamage);
                        target.ApplyStatusEffect(lifeDrainEffect);
                        attacker.Heal(lifeDrainDamage); // ������ ��ŭ ü�� ȸ��
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ������ ����Ͽ� {lifeDrainDamage} �������� ������ ȸ���մϴ�.");
                        break;
                    default:
                        Debug.Log($"{statusType} �����̻��� ���ǵ��� �ʾҽ��ϴ�.");
                        break;
                }
            }
        }
    }

    public double getSpecialDamage()
    {
        return damage;
    }
    public StatusType getStatusType()
    {
        return statusType;
    }

    public void setStatusType(StatusType type)
    {
        this.statusType = type;
    }

    public int getCooltime() => cooltime;

    public int getCurrentCooldown() => currentCooldown;

    // ��Ÿ���� �ʱ�ȭ (��ų ��� �� ����)
    public void ApplyCooldown() => currentCooldown = cooltime;

    // �� ���� �� ��Ÿ�� ����
    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}
