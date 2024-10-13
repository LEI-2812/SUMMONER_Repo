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
    protected IAttackStrategy attackStrategy { get; set; } // �Ϲ� ����
    protected IAttackStrategy[] specialAttackStrategies;


    private void Start()
    {
        image = GetComponent<Image>();
        nowHP = maxHP;
    }


    public void normalAttack(List<Plate> enemyPlates, int selectedPlateIndex,  int SpecialAttackArrayIndex)
    {
        if (attackStrategy == null) return;

        attackStrategy.Attack(this, enemyPlates, selectedPlateIndex, 0); // �Ϲ� ���� ����
    }

    public virtual void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
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

        // ���� ����
        specialAttack.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);

        // �ش� ���ݿ� ��Ÿ�� ����
        specialAttack.ApplyCooldown();
    }

    // �����̻� ���� �޼ҵ� (���� �����̻� �ߺ� ���)
    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        var existingEffect = activeStatusEffects.FirstOrDefault(e => e.statusType == statusEffect.statusType);

        if (existingEffect != null)
        {
            // ���� �����̻��� ���� ���, ���ӽð��� ���� (��� ȿ���� ����)
            existingEffect.effectTime = statusEffect.effectTime -1;  // �����Ͽ� Ǯ�����ϹǷ� -1�� �����ʴ´�.
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
            if (effect != null)
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
        }

        // ����� �����̻� ����
        foreach (var expired in expiredEffects)
        {
            activeStatusEffects.Remove(expired);
            Debug.Log($"{summonName}�� {expired.statusType} �����̻��� ����Ǿ����ϴ�.");
        }

        // Ư�� ���ݵ��� ��Ÿ�� ó��
        if (specialAttackStrategies != null) // �迭�� null�� �ƴ��� Ȯ��
        {
            foreach (var specialAttack in specialAttackStrategies)
            {
                if (specialAttack != null) // �� ������ null���� Ȯ��
                {
                    if (specialAttack.getCurrentCooldown() > 0){
                        specialAttack.ReduceCooldown(); // ��Ÿ�� ����
                        Debug.Log($"{summonName}�� {specialAttack.GetType().Name} ��ų�� ���� ��Ÿ��: {specialAttack.getCurrentCooldown()} ��");
                    }
                    else
                        Debug.Log($"{summonName}�� {specialAttack.GetType().Name} ��ų ��Ÿ���� ����Ǿ����ϴ�.");
                }
            }
        }
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
        attackPower *= (1 + multiplier);
        Debug.Log($"{summonName}�� ���ݷ��� {multiplier * 100}% ����Ǿ����ϴ�. ���� ���ݷ�: {attackPower}");

    }

    public void ApplyDamage(double damage)
    {
        nowHP -= damage; 
        Debug.Log($"{summonName}��(��) {damage} ���ظ� �Ծ����ϴ�. ���� ü��: {nowHP}");

        if (nowHP <= 0)
        {
            die(); // ��� ó��
        }
    }


    // ü�� ȸ��
    public void Heal(double healAmount)
    {
        nowHP += healAmount;
        if(nowHP >= maxHP)
        {
            nowHP = maxHP;
        }
        Debug.Log($"{summonName}��(��) {healAmount}��ŭ ü���� ȸ���߽��ϴ�.");
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


    public void setMaxHP(double hp)
    {
        this.maxHP = hp;
    }
    public double getMaxHP()
    {
        return maxHP;
    }
    // nowHP ���� �޼���
    public void setNowHP(double hp)
    {
        this.nowHP = hp;
    }

    public double getNowHP()
    {
        return nowHP;
    }

    // attackPower ���� �޼���
    public void setAttackPower(double power)
    {
        this.attackPower = power;
    }

    public double getAttackPower()
    {
        return attackPower;
    }

    // specialPower ���� �޼���
    public void setSpecialPower(double power)
    {
        this.specialPower = power;
    }

    public double getSpecialPower()
    {
        return specialPower;
    }


    public SummonRank getSummonRank()
    {
        return summonRank;
    }
    public void setSummonRank(SummonRank rank)
    {
        this.summonRank = rank;
    }

    public List<StatusType> getAllStatusTypes()
    {
        List<StatusType> statusTypes = new List<StatusType>();

        // activeStatusEffects ����Ʈ�� �� StatusEffect���� statusType�� ������ �߰�
        foreach (StatusEffect effect in activeStatusEffects)
        {
            statusTypes.Add(effect.statusType);
        }

        return statusTypes;
    }

    public bool IsCursed()
    {
        // activeStatusEffects ����Ʈ�� �ϳ��� ��ȸ
        foreach (StatusEffect effect in activeStatusEffects)
        {
            // �� �����̻��� ���� Ÿ���� StatusType.Curse���� Ȯ��
            if (effect.statusType == StatusType.Curse)
            {
                // Curse ���°� �߰ߵǸ� true ��ȯ
                return true;
            }
        }

        // Curse ���°� ������ false ��ȯ
        return false;
    }
}
