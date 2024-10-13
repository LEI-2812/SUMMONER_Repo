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
    protected IAttackStrategy[] specialAttackStrategies;

    private Dictionary<string, int> skillCooldowns = new Dictionary<string, int>();

    private void Start()
    {
        image = GetComponent<Image>();
        nowHP = maxHP;
    }

    // ��ų ��Ÿ�� Ȯ�� �޼ҵ�
    public virtual int SpecialAttackCooldown()
    {
        return 3; // �⺻������ 3�� ��Ÿ��
    }

    public void normalAttack(List<Plate> enemyPlates, int selectedPlateIndex,  int SpecialAttackArrayIndex)
    {
        AttackStrategy?.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);
    }

    public void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (IsSkillOnCooldown("SpecialAttack"))
        {
            Debug.Log("Ư�� ��ų�� ��Ÿ�� ���Դϴ�. ����� �� �����ϴ�.");
            return;
        }

        getSpecialAttackStrategy()[SpecialAttackArrayIndex]?.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);

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
            Debug.Log($"{summonName}���� �ߺ��� {statusEffect.statusType} �����̻��� ���ŵǾ����ϴ�.");
        }
        else
        {
            // ���ο� �����̻� �߰�
            activeStatusEffects.Add(statusEffect);
            statusEffect.ApplyStatus(this);  // ��� ȿ�� ����
            statusEffect.effectTime--;       // ��� ȿ�� �ݿ� �� ���ӽð��� 1 ���ҽ�Ŵ
            Debug.Log($"{summonName}���� {statusEffect.statusType} �����̻��� ����Ǿ����ϴ�.");
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
                Debug.Log($"{summonName}��(��) {effect.statusType} ���·� ���� {effect.damagePerTurn} �������� �Խ��ϴ�.");
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
            Debug.Log($"{summonName}�� {expired.statusType} �����̻��� ����Ǿ����ϴ�.");
        }

        // ��ų ��Ÿ�� ó�� (���� ���)
        foreach (var skill in skillCooldowns.Keys.ToList())
        {
            skillCooldowns[skill]--;
            if (skillCooldowns[skill] > 0)
            {
                Debug.Log($"{summonName}�� {skill} ��ų�� ���� ��Ÿ��: {skillCooldowns[skill]} ��");
            }

            if (skillCooldowns[skill] <= 0)
            {
                Debug.Log($"{summonName}�� {skill} ��ų ��Ÿ���� ����Ǿ����ϴ�.");
            }
        }
    }

    // ��ų ��Ÿ�� ���� �޼ҵ�
    public void ApplySkillCooldown(string skillName, int cooldown)
    {
        skillCooldowns[skillName] = cooldown;
        Debug.Log($"{summonName}�� {skillName} ��ų�� {cooldown}�� ���� ��Ÿ�ӿ� ���ϴ�.");
    }

    public void CheckCanAttack()
    {
        if (!CanAttack)
        {
            Debug.Log($"{summonName}��(��) ���� ������ �� �����ϴ�.");
        }
    }

    public void ModifyAttackPower(double multiplier)
    {
        AttackPower *= (1 + multiplier);
        Debug.Log($"{summonName}�� ���ݷ��� {multiplier * 100}% ����Ǿ����ϴ�. ���� ���ݷ�: {AttackPower}");

    }

    public void ApplyDamage(double damage)
    {
        nowHP -= damage;  // �߸��� ���� ����
        Debug.Log($"{summonName}��(��) {damage} ���ظ� �Ծ����ϴ�. ���� ü��: {NowHP}");

        if (NowHP <= 0)
        {
            die(); // ��� ó��
        }
    }


    // ü�� ȸ��
    public void Heal(double healAmount)
    {
        nowHP += healAmount;
        Debug.Log($"{summonName}��(��) {healAmount}��ŭ ü���� ȸ���߽��ϴ�.");
    }


    public virtual void attack()
    {
        Debug.Log($"{summonName} attacks with {attackPower} power!");
    }

    public virtual void takeDamage(double damage) //������ �Ա�
    {
        nowHP -= damage;

        if (nowHP <= 0)
        {
            nowHP = 0;  // ü���� 0 ���Ϸ� ������ ����
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
            die();  // ��� ó��
        }
        else
        {
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
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

    // ��ȯ�� �ʱ�ȭ �޼���
    public virtual void summonInitialize(){ }

    public virtual void die()
    {
        Debug.Log($"{summonName} �� ü���� �Ҹ�Ǿ� ������ϴ�.");
        // Plate���� ��ȯ���� �����ϱ� ���� ��ȯ���� ��ġ�� Plate�� ������
        Plate plate = GetComponentInParent<Plate>(); // ��ȯ���� ���� �θ� Plate ��������
        if (plate != null)
        {
            plate.RemoveSummon(); // ��ȯ�� ����
        }

        // ��ȯ�� ������Ʈ ����
        Destroy(gameObject); // ��ȯ�� ������Ʈ�� ������ ����
    }

    public virtual void takeSkill() { }

    public string getSummonName(){ 
        return summonName; 
    }
    public void setSummonName(string name)
    {
        this.summonName = name;
    }

    public IAttackStrategy[] getSpecialAttackStrategy()
    {
        return specialAttackStrategies;
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

    public SummonRank getSummonRank()
    {
        return summonRank;
    }
    public void setSummonRank(SummonRank rank)
    {
        this.summonRank = rank;
    }

}
