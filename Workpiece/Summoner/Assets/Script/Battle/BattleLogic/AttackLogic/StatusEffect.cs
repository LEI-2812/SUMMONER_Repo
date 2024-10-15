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

    public StatusEffect(StatusType type, int time, double damage=0, Summon attacker = null)
    {
        statusType = type;
        effectTime = time;
        damagePerTurn = damage;
        this.attacker = attacker;
    }

    public void ApplyStatus(Summon target)
    {
        switch (statusType)
        {
            case StatusType.Stun: //������ ����
                target.CanAttack = false;
                break;

            case StatusType.Poison: // �ߵ�
                target.ApplyDamage(damagePerTurn); // ��� ������ ����
                effectTime -= 1; //�������� �����ڸ��� �� �ϳ��� ����.
                break;

            case StatusType.Burn: //ȭ��
                target.ApplyDamage(damagePerTurn); // ��� ������ ����
                effectTime -= 1 ; //�������� �����ڸ��� �� �ϳ��� ����.
                break;

            case StatusType.LifeDrain: //����
                if (attacker != null) //��� ���
                {
                    target.ApplyDamage(damagePerTurn);
                    attacker.Heal(damagePerTurn);
                }
                break;

            case StatusType.Heal: //��
                target.Heal(damagePerTurn); //��� ���
                break;

            default:
                Debug.Log("���ǵ��� ���� �����̻��Դϴ�.");
                break;
        }
    }
}
