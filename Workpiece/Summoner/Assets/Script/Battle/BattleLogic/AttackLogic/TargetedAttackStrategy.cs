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
        Summon target = targetPlates[selectedPlateIndex].getCurrentSummon();

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
                    StatusEffect drainEffect = new StatusEffect(StatusType.LifeDrain, statusTime, lifeDrainDamage, attacker);
                    target.ApplyStatusEffect(drainEffect);
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ������ ����Ͽ� {lifeDrainDamage} �������� ������ ȸ���մϴ�.");
                    break;
                case StatusType.Shield: //����
                    target = attacker;
                    StatusEffect shieldEffect = new StatusEffect(StatusType.Shield, statusTime, damage);
                    target.ApplyStatusEffect(shieldEffect);
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� {damage} ��ŭ ��ȣ���� �ο��մϴ�.");
                    break;
                case StatusType.Upgrade: //��ȭ
                    double upgradeAttackPower = target.getAttackPower() * attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage();
                    StatusEffect upgradeEffect = new StatusEffect(StatusType.Upgrade, statusTime, upgradeAttackPower);
                    target.ApplyStatusEffect(upgradeEffect);
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ���ݷ� {upgradeAttackPower} ��ŭ ��� ���׽��ϴ�.");
                    break;
                case StatusType.OnceInvincibility: //����
                    target = attacker; //�ڱ��ڽ��� ���
                    target.setOnceInvincibility(true); //1�� ���� Ȱ��ȭ
                    break;
                case StatusType.Curse:
                    double curseAttackPower = target.getAttackPower() - (target.getAttackPower() * 0.2); //��� ���ݷ� 20% ����
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}��(��) �����ϰ� �����մϴ�.");
                    StatusEffect curseEffect = new StatusEffect(StatusType.Upgrade, statusTime, curseAttackPower);
                    target.ApplyStatusEffect(curseEffect);
                    target.takeDamage(attacker.getSpecialAttackStrategy()[Arrayindex].getSpecialDamage()); // ������ ����
                    break;
                case StatusType.Stun:
                    StatusEffect stunEffect = new StatusEffect(StatusType.Stun, statusTime);
                    target.ApplyStatusEffect(stunEffect);
                    Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ������ �����߽��ϴ�.");
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
