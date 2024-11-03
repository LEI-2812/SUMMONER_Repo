using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum StatusType
{
    None, //�ܼ� ����
    Stun, //ȥ��
    Poison, //�ߵ�
    Heal, //��
    Curse, //����
    LifeDrain, //����
    Burn, //ȭ��
    Shield, //��ȣ��
    Upgrade, //��ȭ
    OnceInvincibility //1ȸ ����
}

public class StatusEffect
{
    public StatusType statusType { get; private set; }
    public int effectTime { get; set; } // ���� �ð�
    public double damagePerTurn { get; set; } // �� �ϸ��� �� ������

    private Summon attacker; // ������

    private bool applyOnce = false;

    private double originAttack;
    public StatusEffect(StatusType type, int eTime, double damage=0, Summon attacker = null)
    {
        statusType = type;
        effectTime = eTime;
        damagePerTurn = damage;
        this.attacker = attacker;
        applyOnce = false;
    }


    //��������� �Ϳ� ���ؼ��� �ֱ�
    public void ApplyStatus(Summon target)
    {
        switch (statusType)
        {
            case StatusType.Stun: //���� �Ұ���
                target.setIsAttack(false);
                break;

            case StatusType.Poison: // �ߵ�
                target.takeDamage(damagePerTurn); // ��� ������ ����
                effectTime -= 1; //�������� �����ڸ��� �� �ϳ��� ����.
                break;

            case StatusType.Burn: //ȭ��
                target.takeDamage(damagePerTurn); // ��� ������ ����
                effectTime -= 1 ; //�������� �����ڸ��� �� �ϳ��� ����.
                break;

            case StatusType.LifeDrain: //����
                if (target != null) //��� ���
                {
                    target.takeDamage(damagePerTurn);
                    attacker.Heal(damagePerTurn);
                    Debug.Log($"{target.getSummonName()}���Լ� {damagePerTurn} ��ŭ �����մϴ�. ����ü��: {attacker.getNowHP()}");
                }
                else
                {
                    Debug.Log("Ÿ���� ���ų� �׾ ������ �ȵ˴ϴ�.");
                }
                break;

            case StatusType.Shield: //����
                target.AddShield(damagePerTurn);
                break;

            case StatusType.Upgrade: //��ȭ
                target.UpgradeAttackPower(damagePerTurn);
                break;
            case StatusType.Curse: //����
                target.Cursed(damagePerTurn);
                break;

            default:
                Debug.Log("���ǵ��� ���� �����̻��Դϴ�.");
                break;
        }
    }

    public double getOriginAttack()
    {
        return originAttack;
    }
    public void setOriginAttack(double originAttack)
    {
        this.originAttack = originAttack;
    }

    public Summon getAttacker()
    {
        return attacker;
    }
    public void setAttacker(Summon summon)
    {
        attacker = summon;
    }

    public bool shouldApplyOnce()
    {
        return damagePerTurn > 0 && !applyOnce;
    }

    public void setApplyOnce()
    {
        applyOnce = true; // �� ���� ����ǵ��� ǥ��
    }
}
