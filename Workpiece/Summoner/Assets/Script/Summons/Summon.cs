using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High, //�Ʊ� ��ȯ��
    Normal, Special, Boss //�� ��ȯ��
}

public class Summon : MonoBehaviour
{
    public Image image; //�̹���
    protected string summonName; //�̸�
    protected double maxHP; //�ִ�ü��
    protected double nowHP; //���� ü��
    protected double attackPower; //�Ϲݰ���
    protected double specialPower;  //Ư������
    protected SummonRank summonRank; //���

    public bool CanAttack { get; set; } = true; // �����̻��� ���ݰ��� ����

    private List<StatusEffect> activeStatusEffects = new List<StatusEffect>(); //�����̻�
    protected IAttackStrategy AttackStrategy { get; set; } // �Ϲ� ����
    protected IAttackStrategy SpecialAttackStrategy { get; set; } // Ư�� ����

    private Dictionary<string, int> skillCooldowns = new Dictionary<string, int>();


    private void Start()
    {
        nowHP = maxHP;
    }

    // ��ų ��Ÿ�� Ȯ�� �޼ҵ�
    public virtual int SpecialAttackCooldown()
    {
        return 3; // �⺻������ 3�� ��Ÿ��
    }

    public void normalAttack(List<Plate> enemyPlates, int selectedPlateIndex)
    {
        AttackStrategy?.Attack(this, enemyPlates, selectedPlateIndex);
    }

    public void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex)
    {
        if (IsSkillOnCooldown("SpecialAttack"))
        {
            Debug.Log("Ư�� ��ų�� ��Ÿ�� ���Դϴ�. ����� �� �����ϴ�.");
            return;
        }

        SpecialAttackStrategy?.Attack(this, enemyPlates, selectedPlateIndex);

        // ��ų ��� �� ��Ÿ�� ����
        ApplySkillCooldown("SpecialAttack", SpecialAttackCooldown());
    }

    // �����̻� ���� �޼ҵ� (���� �����̻� �ߺ� ���)
    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        var existingEffect = activeStatusEffects.FirstOrDefault(e => e.statusType == statusEffect.statusType);

        if (existingEffect != null)
        {
            // ���� �����̻��� ���� ���, ���ӽð��� ���� (��� ȿ���� ����)
            existingEffect.effectTime = statusEffect.effectTime - 1;  // ��� ȿ���� �ݿ������Ƿ� ���ӽð��� 1 ����
            existingEffect.damagePerTurn = statusEffect.damagePerTurn;
            Debug.Log($"{SummonName}���� �ߺ��� {statusEffect.statusType} �����̻��� ���ŵǾ����ϴ�.");
        }
        else
        {
            // ���ο� �����̻� �߰�
            activeStatusEffects.Add(statusEffect);
            statusEffect.ApplyStatus(this);  // ��� ȿ�� ����
            statusEffect.effectTime--;       // ��� ȿ�� �ݿ� �� ���ӽð��� 1 ���ҽ�Ŵ
            Debug.Log($"{SummonName}���� {statusEffect.statusType} �����̻��� ����Ǿ����ϴ�.");
        }
    }

    // �����̻� �� ��Ÿ�� ������Ʈ �޼ҵ�
    public void UpdateStatusEffectsAndCooldowns()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        // ��� �����̻��� ���ӽð��� ���ҽ�Ű��, ���� ������ ó��
        foreach (var effect in activeStatusEffects)
        {
            effect.effectTime--; // �����̻� ���ӽð� ����

            if (effect.damagePerTurn > 0)
            {
                ApplyDamage(effect.damagePerTurn); // ������ ����
                Debug.Log($"{SummonName}��(��) {effect.statusType} ���·� ���� {effect.damagePerTurn} �������� �Խ��ϴ�.");
            }

            if (effect.effectTime <= 0)
            {
                expiredEffects.Add(effect); // ���� �ð��� ���� �����̻��� ���� ó��
            }
        }

        // ����� �����̻� ����
        foreach (var expired in expiredEffects)
        {
            activeStatusEffects.Remove(expired);
            Debug.Log($"{SummonName}�� {expired.statusType} �����̻��� ����Ǿ����ϴ�.");
        }

        // ��ų ��Ÿ�� ó�� (���� ���)
        foreach (var skill in skillCooldowns.Keys.ToList())
        {
            skillCooldowns[skill]--;
            if (skillCooldowns[skill] > 0)
            {
                Debug.Log($"{SummonName}�� {skill} ��ų�� ���� ��Ÿ��: {skillCooldowns[skill]} ��");
            }

            if (skillCooldowns[skill] <= 0)
            {
                Debug.Log($"{SummonName}�� {skill} ��ų ��Ÿ���� ����Ǿ����ϴ�.");
            }
        }
    }

    // ��ų ��Ÿ�� ���� �޼ҵ�
    public void ApplySkillCooldown(string skillName, int cooldown)
    {
        skillCooldowns[skillName] = cooldown;
        Debug.Log($"{SummonName}�� {skillName} ��ų�� {cooldown}�� ���� ��Ÿ�ӿ� ���ϴ�.");
    }

    public void CheckCanAttack()
    {
        if (!CanAttack)
        {
            Debug.Log($"{SummonName}��(��) ���� ������ �� �����ϴ�.");
        }
    }

    public void ModifyAttackPower(double multiplier)
    {
        AttackPower *= (1 + multiplier);
        Debug.Log($"{SummonName}�� ���ݷ��� {multiplier * 100}% ����Ǿ����ϴ�. ���� ���ݷ�: {AttackPower}");

    }

    public void ApplyDamage(double damage)
    {
        NowHP -= damage;
        Debug.Log($"{SummonName}��(��) {damage} ���ظ� �Ծ����ϴ�. ���� ü��: {NowHP}");

        if (NowHP <= 0)
        {
            die(); // ��� ó��
        }
    }


    // ü�� ȸ��
    public void Heal(double healAmount)
    {
        nowHP += healAmount;
        Debug.Log($"{SummonName}��(��) {healAmount}��ŭ ü���� ȸ���߽��ϴ�.");
    }


    public virtual void attack()
    {
        Debug.Log($"{summonName} attacks with {attackPower} power!");
    }

    public virtual void takeDamage(double damage) //������ �Ա�
    {
        nowHP = nowHP - damage;
        Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
        if (nowHP <= 0)
        {
            die();
        }
    }

    public bool IsSkillOnCooldown(string skillName)
    {
        if (skillCooldowns.ContainsKey(skillName) && skillCooldowns[skillName] > 0)
        {
            return true;
        }
        return false;
    }



    public virtual void takeSkill() //��ų���
    {
        Debug.Log($"{summonName} takes skill.");
    }

    public virtual void die()
    {
        Debug.Log($"{summonName} �� ü���� �Ҹ�Ǿ� ������ϴ�.");
        Destroy(gameObject); // ��ȯ�� ������Ʈ ����
    }

    public string SummonName
    {
        get { return summonName; }
        set { summonName = value; }
    }

    public double MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public double NowHP
    {
        get { return nowHP; }
        set { nowHP = value; }
    }

    public double AttackPower
    {
        get { return attackPower; }
        set { attackPower = value; }
    }

    public double SpecialPower
    {
        get { return specialPower; }
        set { specialPower = value; }
    }

    public SummonRank SummonRank
    {
        get { return summonRank; }
        set { summonRank = value; }
    }

}
