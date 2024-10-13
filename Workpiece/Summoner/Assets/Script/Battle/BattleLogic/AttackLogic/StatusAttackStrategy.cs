using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAttackStrategy : IAttackStrategy
{
    private StatusType statusType; //�����̻� ����
    private int statusTime; //����ð�(��)

    public StatusAttackStrategy(StatusType statusType, int time)
    {
        this.statusType = statusType;
        this.statusTime = time;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon target = plate.getSummon();
            if (target != null)
            {
                switch (statusType)
                {
                    case StatusType.Poison:
                        double poisonDamage = target.MaxHP * 0.1; // �ִ� ü���� 10% ������
                        StatusEffect poisonEffect = new StatusEffect(StatusType.Poison, statusTime, poisonDamage);
                        target.ApplyStatusEffect(poisonEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� �ߵ� ���¸� �ο��Ͽ� �� �� {poisonDamage} �������� �����ϴ�.");
                        break;

                    case StatusType.Stun:
                        StatusEffect stunEffect = new StatusEffect(StatusType.Stun , statusTime);
                        target.ApplyStatusEffect(stunEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ���� ���¸� �ο��߽��ϴ�.");
                        break;

                    case StatusType.Curse:
                        StatusEffect curseEffect = new StatusEffect(StatusType.Curse , statusTime);
                        target.ApplyStatusEffect(curseEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ���� ���¸� �ο��Ͽ� ���ݷ��� ���ҽ�ŵ�ϴ�.");
                        break;

                    case StatusType.Burn:
                        double burnDamage = target.MaxHP * 0.2; // �ִ� ü���� 20% ������
                        StatusEffect burnEffect = new StatusEffect(StatusType.Burn, statusTime, burnDamage);
                        target.ApplyStatusEffect(burnEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ȭ���� ���� �� �� {burnDamage} �������� �����ϴ�.");
                        break;

                    case StatusType.LifeDrain:
                        double lifeDrainDamage = target.MaxHP * 0.1;
                        StatusEffect lifeDrainEffect = new StatusEffect(StatusType.LifeDrain, statusTime, lifeDrainDamage);
                        target.ApplyStatusEffect(lifeDrainEffect);
                        attacker.Heal(lifeDrainDamage); // ������ ��ŭ ü�� ȸ��
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ������ ����Ͽ� {lifeDrainDamage} �������� ������ ȸ���մϴ�.");
                        break;

                    case StatusType.Heal:
                        double heal = target.MaxHP * 0.3;
                        target.Heal(heal); // ü�� ȸ��
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}�� ü���� ȸ���մϴ�.");
                        break;

                    default:
                        Debug.Log($"{statusType} �����̻��� ���ǵ��� �ʾҽ��ϴ�.");
                        break;
                }
            }
        }
    }

    public StatusType getStatusType()
    {
        return statusType;
    }

    public void setStatusType(StatusType type)
    {
        this.statusType = type;
    }
}
