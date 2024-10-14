using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetedAttackStrategy : IAttackStrategy
{
    private StatusType statusType; // ���� Ÿ�� (�������� ������)
    private double damage; //������
    private int cooltime; //��Ÿ��
    private int currentCooldown; //���� ��Ÿ�� ����ð�
    private int statusTime; //���ӽð�
    public TargetedAttackStrategy(StatusType statusType, double damage, int cooltime, int statusTime=0)
    {
        this.statusType = statusType;
        this.damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
        this.statusTime = statusTime;
    }
    public void Attack(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex, int Arrayindex)
    {
        Summon target = targetPlates[selectedPlateIndex].getSummon();

        if (target != null)
        {
            switch (statusType)
            {
                case StatusType.Heal:
                    double healAmount = target.getMaxHP() * 0.3; // �ִ� ü���� 30%��ŭ ȸ��
                    target.Heal(healAmount);
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}��(��) {healAmount}��ŭ ġ���߽��ϴ�.");
                    break;
                case StatusType.None:
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}��(��) �����ϰ� �����մϴ�.");
                    target.takeDamage(attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage()); // ������ ����
                    break;
                case StatusType.LifeDrain: //����
                    double lifeDrainDamage = target.getMaxHP() * 0.1;
                    attacker.Heal(lifeDrainDamage); // ������ ��ŭ ü�� ȸ��
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ������ ����Ͽ� {lifeDrainDamage} �������� ������ ȸ���մϴ�.");
                    break;
               
            }
        }
        else
        {
            Debug.Log("���õ� plate�� ����� �����ϴ�.");
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
    public int getCooltime() { return cooltime; }


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
