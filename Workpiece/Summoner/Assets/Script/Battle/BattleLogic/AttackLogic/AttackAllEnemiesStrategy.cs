using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackAllEnemiesStrategy : IAttackStrategy
{
    private StatusType statusType = StatusType.None;
    private double Damage;
    private int cooltime;
    private int currentCooldown;
    private int statusTime; //���ӽð�
    public AttackAllEnemiesStrategy(StatusType statusType, double damage, int cooltime, int statusTime=0)
    {
        this.statusType = statusType;
        Damage = damage;
        this.cooltime = cooltime;
        this.currentCooldown = 0;
        this.statusTime = statusTime;
    }

    public void Attack(Summon attacker, List<Plate> enemyPlates,int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        foreach (var plate in enemyPlates)
        {
            Summon target = plate.getSummon();
            if (target != null)
            {
                switch (statusType)
                {
                    case StatusType.None:
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}��(��) ��ü �����մϴ�.");
                        target.takeDamage(attacker.getSpecialAttackStrategy()[SpecialAttackArrayIndex].getSpecialDamage());
                        break;
                    case StatusType.Poison:
                        double poisonDamage = target.getMaxHP() * 0.1; // �ִ� ü���� 10% ������
                        StatusEffect poisonEffect = new StatusEffect(StatusType.Poison, statusTime, poisonDamage);
                        target.ApplyStatusEffect(poisonEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� �ߵ� ���¸� �ο��Ͽ� �� �� {poisonDamage} �������� �����ϴ�.");
                        break;
                    case StatusType.Burn:
                        double burnDamage = target.getMaxHP() * 0.2; // �ִ� ü���� 20% ������
                        StatusEffect burnEffect = new StatusEffect(StatusType.Burn, statusTime, burnDamage);
                        target.ApplyStatusEffect(burnEffect);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ȭ���� ���� �� �� {burnDamage} �������� �����ϴ�.");
                        break;
                    case StatusType.Upgrade:
                        double upgradeAttackPower = target.getAttackPower() * 0.1;
                        StatusEffect upgradeEffect = new StatusEffect(StatusType.Upgrade, statusTime, upgradeAttackPower);
                        target.ApplyStatusEffect(upgradeEffect);
                        target.UpgradeAttackPower(upgradeAttackPower);
                        Debug.Log($"{attacker.getSummonName()}��(��) {target.getSummonName()}���� ���ݷ� {upgradeAttackPower} ��ŭ ��� ���׽��ϴ�.");
                        break;
                }

            }
        }
    }

    public double getSpecialDamage()
    {
        return Damage;
    }
    public StatusType getStatusType()
    {
        return statusType;
    }
    public int getCooltime()
    {
        return cooltime;
    }

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
