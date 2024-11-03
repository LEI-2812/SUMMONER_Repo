using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High, //�Ʊ� ��ȯ��
    Normal, Special, Boss //�� ��ȯ��
}

public enum SummonType
{
    Cat, Rabbit, Wolf, Eagle, Snake, Fox
}

public class Summon : MonoBehaviour, UpdateStateObserver
{
    [SerializeField] private Image image; //�̹���
    [SerializeField] private Sprite[] sprites; // ��������Ʈ ����
    [SerializeField] private Animator animator;

    [Header("ȿ����")]
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource downHitSound;
    [SerializeField] private AudioSource upAttackSound;

    protected string summonName; //�̸�
    public Sprite normalAttackSprite; // �Ϲ� ���� ��������Ʈ
    public Sprite specialAttackSprite; // Ư�� ���� ��������Ʈ
    public double attackPower; //�Ϲݰ���
    public double heavyAttakPower; //�� ���ݷ�
    protected SummonRank summonRank; //���
    protected SummonType summonType;
    protected double maxHP; //�ִ�ü��
    public double nowHP; //���� ü��
    protected double shield = 0; //���差
    private double initialShield; // �ʱ� ���� ��
    protected bool onceInvincibility = false;
    public bool isAttack = true; // �����̻��� ���ݰ��� ����

    private List<StatusEffect> activeStatusEffects = new List<StatusEffect>(); //�����̻�
    protected IAttackStrategy attackStrategy;
    protected IAttackStrategy[] specialAttackStrategies;

    private List<stateObserver> observers = new List<stateObserver>();


    private void Awake()
    {
        image = GetComponent<Image>();
        nowHP = maxHP;
    }

    public void SetSprite(int index)
    {
        image.sprite = sprites[index];        
    }


    public void normalAttack(List<Plate> targetPlates, int selectedPlateIndex)
    {
        if (attackStrategy == null || attackStrategy.getCurrentCooldown() > 0)
        {
            Debug.Log("�Ϲ� ������ �ش��Ͽ� ����߽��ϴ�.");
            return;
        }
        attackStrategy.Attack(this, targetPlates, selectedPlateIndex, 0); // �Ϲ� ���� ����
        animator.SetTrigger("attack");
        attackSound.Play();
        
        StartCoroutine(ColorChange(1)); // ������
        
        // �ش� ���ݿ� ��Ÿ�� ����
        attackStrategy.ApplyCooldown();
        isAttack = false;
    }

    IEnumerator ColorChange(int color)    // ���� ���ߴٰ� ���ƿ�
    {
        switch (color)
        {
            case 1: // ������
                image.color = new Color(0f, 0f, 0f); // #000000
                break;
            case 2: // ������
                image.color = new Color(1f, 0.431f, 0.431f); // #FF6E6E
                break;
            case 3: // �����
                image.color = new Color(0.639f, 0.192f, 0.839f); // #A331D6
                break;
            case 4: // �ʷϻ�
                image.color = new Color(0.192f, 0.835f, 0.318f); // #31D551
                break;
            default:
                image.color = Color.white; // �⺻�� ����
                break;

        }

        yield return new WaitForSeconds(1f);

        image.color = Color.white;
    }

    public virtual void SpecialAttack(List<Plate> targetPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
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
        specialAttack.Attack(this, targetPlates, selectedPlateIndex, SpecialAttackArrayIndex);
        animator.SetTrigger("attack");
        StartCoroutine(ColorChange(1)); // ������
        // �ش� ���ݿ� ��Ÿ�� ����
        specialAttack.ApplyCooldown();
        isAttack = false;
    }

    // �����̻� ���� �޼ҵ� (���� �����̻� �ߺ� ���) //��������� ����
    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        var existingEffect = activeStatusEffects.FirstOrDefault(e => e.statusType == statusEffect.statusType); //�̹� ���� �����̻��� �ִ��� ������

        // �����̻� Ÿ�Կ� ���� ������ �ٸ��� ó��
        switch (statusEffect.statusType)
        {
            case StatusType.Poison:
                if (existingEffect != null)
                {
                    // �ߵ� ���°� �̹� ������ ���ӽð��� ���ط��� ����
                    Debug.Log($"{summonName}�� �̹� �ߵ� �����Դϴ�.");
                }
                else
                {
                    // ���ο� �ߵ� �����̻� �߰�
                    StartCoroutine(ColorChange(3)); // �����
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // ��� ȿ�� ���� //�������� ����
                    NotifyObservers(); // ���� ���� �� �˸�
                    downHitSound.Play();
                    Debug.Log($"{summonName}���� �ߵ� �����̻��� ����Ǿ����ϴ�.");
                }
                break;

            case StatusType.Burn:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}�� �̹� ȭ�� �����Դϴ�.");
                }
                else
                {
                    // ���ο� ȭ�� �����̻� �߰�
                    StartCoroutine(ColorChange(3)); // �����
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // ��� ȿ�� ����
                    NotifyObservers(); // ���� ���� �� �˸�
                    downHitSound.Play();
                    Debug.Log($"{summonName}���� ȭ�� �����̻��� ����Ǿ����ϴ�.");
                }
                break;

            case StatusType.Upgrade:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}�� �̹� ��ȭ �����Դϴ�.");
                }
                else
                {
                    // ���� ���ݷ��� �����մϴ�.
                    statusEffect.setOriginAttack(this.attackPower);
                    // ���ο� ��ȭ �����̻� �߰�
                    activeStatusEffects.Add(statusEffect);
                    if (statusEffect.shouldApplyOnce())
                    {
                        statusEffect.ApplyStatus(this); // �� ���� ����
                        statusEffect.setApplyOnce(); // ����� ���� ǥ��
                        NotifyObservers(); // ���� ���� �� �˸�
                        upAttackSound.Play();
                    }

       
                    Debug.Log($"{summonName}�� ���ݷ��� ��ȭ�Ǿ����ϴ�.");
                }
                break;

            case StatusType.Curse:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}�� �̹� ���� �����Դϴ�.");
                }
                else
                {
                    // ���ο� ���� �����̻� �߰�
                    StartCoroutine(ColorChange(3)); // �����
                    activeStatusEffects.Add(statusEffect);
                    if (statusEffect.shouldApplyOnce())
                    {
                        statusEffect.ApplyStatus(this); // �� ���� ����
                        statusEffect.setApplyOnce(); // ����� ���� ǥ��
                        NotifyObservers(); // ���� ���� �� �˸�
                    }
                    downHitSound.Play();
                    Debug.Log($"{summonName}���� ���� �����̻��� ����Ǿ����ϴ�.");
                }
                break;

            case StatusType.Stun:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}�� �̹� ���� �����Դϴ�.");
                }
                else
                {
                    // ���ο� ���� �����̻� �߰�
                    StartCoroutine(ColorChange(3)); // �����
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // ��� ȿ�� ����
                    NotifyObservers(); // ���� ���� �� �˸�
                    downHitSound.Play();
                    Debug.Log($"{summonName}��(��) ���� ���¿� �������ϴ�.");
                }
                break;

            case StatusType.Shield: //���� ������
                if (existingEffect != null)
                {
                    shield = existingEffect.damagePerTurn; //��ȣ���� ��ų ��ġ��ŭ �ٽ� ä���
                    upAttackSound.Play();
                    Debug.Log($"{summonName}�� ��ȣ���� ������ϴ�.");
                }
                else
                {
                    // ���ο� ���� �߰�
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // ��� ȿ�� ����
                    NotifyObservers(); // ���� ���� �� �˸�
                    upAttackSound.Play();
                    Debug.Log($"{summonName}���� ��ȣ���� ������ϴ�.");
                }
                break;
            case StatusType.LifeDrain: //����
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}�� �̹� ���� ���ϰ��ֽ��ϴ�.");
                }
                else
                {
                    StartCoroutine(ColorChange(3)); // �����
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // ��� ȿ�� ����
                    NotifyObservers(); // ���� ���� �� �˸�
                    downHitSound.Play();
                    Debug.Log($"{summonName}�� ���� ���մϴ�.");
                }
                break;

            default:
                Debug.Log($"{summonName}���� �� �� ���� �����̻��� ����Ǿ����ϴ�.");
                break;
        }
    }

    // �������� �ִ� �����̻� ������Ʈ �޼ҵ� (��: Poison, Burn ��)
    public void UpdateDamageStatusEffects()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        foreach (var effect in activeStatusEffects)
        {
            if (effect != null && effect.effectTime > 0)
            {
                // ������ �ִ� ���� Ȯ�� �� ó��
                if (effect.damagePerTurn > 0 && effect.statusType != StatusType.Upgrade && effect.statusType != StatusType.Curse)
                {
                    Debug.Log($"{summonName}��(��) {effect.statusType} ���·� ���� {effect.damagePerTurn} �������� �Խ��ϴ�. ���� �����̻�ð�: {effect.effectTime} ��");
                    takeDamage(effect.damagePerTurn); // ���� ����
                }

                // ���� ������ ��� ȸ�� ó��
                if (effect.statusType == StatusType.LifeDrain && effect.getAttacker() != null)
                {
                    double healAmount = effect.damagePerTurn; // ���� ��������ŭ ȸ��
                    Debug.Log($"{effect.getAttacker()}��(��) {effect.statusType} ���·� ���� {healAmount}��ŭ ȸ���մϴ�.");
                    effect.getAttacker().Heal(effect.damagePerTurn);
                }

                // ���ӽð� ����
                effect.effectTime--;

                // ���°� ����� ��� ���� ����Ʈ�� �߰�
                if (effect.effectTime <= 0)
                {
                    expiredEffects.Add(effect);
                }
            }
        }

        RemoveExpiredEffects(expiredEffects);
    }

    // ���� ���� ������Ʈ �޼ҵ�
    public void UpdateStunAndCurseStatus()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        foreach (var effect in activeStatusEffects)
        {
            if (effect != null && (effect.statusType == StatusType.Stun || effect.statusType == StatusType.Curse))
            {
                // ���� ���´� ���� �Ұ����ϰ� ����
                if (effect.statusType == StatusType.Stun)
                {
                    setIsAttack(false);
                    Debug.Log($"{summonName}�� ���ϰ� ���� ���·� ������ �� �����ϴ�.");
                }

                // ���ӽð� ����
                effect.effectTime--;

                // ���°� ����� ��� ���� ����Ʈ�� �߰�
                if (effect.effectTime <= 0)
                {
                    expiredEffects.Add(effect);
                }
            }
        }

        RemoveExpiredEffects(expiredEffects);
    }

    // ��ȭ ���� ������Ʈ �޼ҵ�
    public void UpdateUpgradeStatus()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        foreach (var effect in activeStatusEffects)
        {
            if (effect != null && effect.statusType == StatusType.Upgrade)
            {
                // ��ȭ�� ���ӽð��� �����ϸ�, �������� ���� ����
                effect.effectTime--;

                // ���°� ����� ��� ���� ����Ʈ�� �߰�
                if (effect.effectTime <= 0)
                {
                    Debug.Log("���ݷº���");
                    attackPower = effect.getOriginAttack(); // ���� ���ݷ����� ����
                    expiredEffects.Add(effect);
                }
            }
        }

        RemoveExpiredEffects(expiredEffects);
    }

    // ����� �����̻� ���� �޼ҵ�
    private void RemoveExpiredEffects(List<StatusEffect> expiredEffects)
    {
        foreach (var expired in expiredEffects)
        {
            activeStatusEffects.Remove(expired);
            Debug.Log($"{summonName}�� {expired.statusType} �����̻��� ����Ǿ����ϴ�.");

            // ���� ���� �� ���� �����ϵ��� ����
            if (expired.statusType == StatusType.Stun)
            {
                setIsAttack(true);
                Debug.Log($"{summonName}�� ������ �����Ǿ����ϴ�. ���� ����.");
            }
            NotifyObservers(); // ���� ���� �� �˸�
        }
    }



    // Ư�� ���� ��Ÿ�� ������Ʈ �޼ҵ�
    public void UpdateSpecialAttackCooldowns()
    {
        if (specialAttackStrategies == null) return; // �迭�� null�� ��� üũ

        foreach (var specialAttack in specialAttackStrategies)
        {
            if (specialAttack != null)
            {
                if (specialAttack.getCurrentCooldown() > 0)
                {
                    specialAttack.ReduceCooldown(); // ��Ÿ�� ����
                    Debug.Log($"{summonName}�� {specialAttack.GetType().Name} ��ų�� ���� ��Ÿ��: {specialAttack.getCurrentCooldown()} ��");
                }
                else
                {
                    Debug.Log($"{summonName}�� {specialAttack.GetType().Name} ��ų ��Ÿ���� ����Ǿ����ϴ�.");
                }
            }
        }
    }



    public bool getIsAttack()
    {
        return isAttack;
    }
    public void setIsAttack(bool isAttack)
    {
        this.isAttack = isAttack;
    }

    public SummonType getSummonType()
    {
        return summonType;
    }
    

    public void UpgradeAttackPower(double multiplier)
    {
        attackPower *= (1 + multiplier);
        attackPower = Math.Floor(attackPower); // �Ҽ��� �Ʒ��� ����

        attackPower = (int)attackPower; // double�� int�� ��ȯ
        Debug.Log($"{summonName}�� ���ݷ��� {multiplier * 100}% ��ȭ �Ǿ����ϴ�. ���� ���ݷ�: {attackPower}");
    }

    public void Cursed(double curse)
    {
        attackPower *= (1 - curse);
        Debug.Log($"{summonName}�� ���ݷ��� {curse * 100}% �ٿ� �Ǿ����ϴ�. ���� ���ݷ�: {attackPower}");
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
        // ü�� ���� �� �������鿡�� �˸�
        NotifyObservers();
        animator.SetTrigger("hitted");
        StartCoroutine(ColorChange(4)); // �ʷϻ�
        upAttackSound.Play();
    }


    public virtual void takeDamage(double damage) //������ �Ա�
    {
        damage = (int)damage; //�������� �Ҽ��� ����
        damage = Math.Floor(damage); // �Ҽ��� �Ʒ��� ����

        if (onceInvincibility)
        {
            onceInvincibility = false;
            Debug.Log("1ȸ ������ȣ������ ������ ��ȣ�߽��ϴ�.");
            return;
        }
        if (shield > 0) //���尡 ������ ������ �ް�
        {
            if (shield >= damage)
            {
                // ���尡 �������� ��� ������
                shield -= damage;
                animator.SetTrigger("hitted");
                Debug.Log("����� ���� ���. ���� ����: " + shield);
            }
            else
            {
                // ���尡 �Ϻθ� ���� �������� ü�¿� ����
                double remainingDamage = damage - shield;
                shield = 0;
                nowHP -= remainingDamage;
                animator.SetTrigger("hitted");
                Debug.Log("���尡 �ı���. ���� ü��: " + nowHP);
            }
        }
        else //���尡 �������
        {
            nowHP -= damage;
            animator.SetTrigger("hitted");
            StartCoroutine((ColorChange(2)));   // ���� ��
        }

        if (nowHP <= 0) //����ó��
        {
            nowHP = 0;  // ü���� 0 ���Ϸ� ������ ����
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
            die();  // ��� ó��
        }
        else
        {
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
        }

        // ü�� ���� �� �������鿡�� �˸�
        NotifyObservers();
    }


    // ��ȯ�� �ʱ�ȭ �޼���
    public virtual void summonInitialize()
    {
        NotifyObservers();
    }

    public static double multiple=5; //�������
    public virtual void ApplayMultiple(double m) {
        maxHP = (int)(maxHP * m);
        nowHP = maxHP;
        attackPower = (int)(attackPower * m); // �Ϲݰ��� ��� ����
        heavyAttakPower = (int)(heavyAttakPower * m); // ������ ��� ����
    }


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


    public void AddShield(double shieldAmount)
    {
        if (shield == 0) // ���� ���尡 0�� ���� �ʱ�ȭ
        {
            initialShield = shieldAmount;
        }
        shield += shieldAmount;
        Debug.Log("���� �ο�. ���� ����: " + shield);
        NotifyObservers();
    }
    public double getShield()
    {
        return shield; // ���� ���� ���� ��ȯ
    }
    public double GetInitialShield()
    {
        return initialShield; // �ʱ� ���� ���� ��ȯ
    }

    public string getSummonName(){ 
        return summonName; 
    }
    public void setSummonName(string name)
    {
        this.summonName = name;
    }

    public double getHeavyAttackPower()
    {
        return heavyAttakPower;
    }
    public void setHeavyAttackPower(double value)
    {
        this.heavyAttakPower = value;
    }

    public IAttackStrategy[] getSpecialAttackStrategy()
    {
        return specialAttackStrategies;
    }

    public IAttackStrategy getAttackStrategy()
    {
        return attackStrategy;
    }

    public bool getInvincibilityOnce()
    {
        return onceInvincibility;
    }
    public void setOnceInvincibility(bool isinvincibility)
    {
        this.onceInvincibility = isinvincibility;
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


    public SummonRank getSummonRank()
    {
        return summonRank;
    }
    public void setSummonRank(SummonRank rank)
    {
        this.summonRank = rank;
    }

    public void setImage(Image image)
    {
        this.image = image;
    }
    public Image getImage()
    {
        return image;
    }


    //Ư�������� ��Ÿ������
    public bool isSpecialAttackCool(IAttackStrategy specialAttack)
    {
        if (specialAttack!= null && specialAttack.getCurrentCooldown() > 0)
        {
            return true;
        }

        return false;
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

    public bool IsStun()
    {
        // activeStatusEffects ����Ʈ�� �ϳ��� ��ȸ
        foreach (StatusEffect effect in activeStatusEffects)
        {
            // �� �����̻��� ���� Ÿ���� StatusType.Stun Ȯ��
            if (effect.statusType == StatusType.Stun)
            {
                // Stun ���°� �߰ߵǸ� true ��ȯ
                return true;
            }
        }

        // Stun ���°� ������ false ��ȯ
        return false;
    }

    public bool IsCooltime() //��Ÿ������ Ȯ��
    {
        // ���� Ư�� ���� ���� �迭�� �ִ��� Ȯ��
        if (specialAttackStrategies == null || specialAttackStrategies.Length == 0)
        {
            Debug.Log("Ư�� ������ �����ϴ�.");
            return false;
        }

        // �� Ư�� ���� ������ ���� ��Ÿ�� ���� Ȯ��
        foreach (var specialAttack in specialAttackStrategies)
        {
            if (specialAttack != null && specialAttack.getCurrentCooldown() > 0)
            {
                // �ϳ��� ��Ÿ�� ���� ������ ������ true ��ȯ
                Debug.Log($"{summonName}�� {specialAttack.GetType().Name} ��ų�� ��Ÿ�� ���Դϴ�.");
                return true;
            }
        }
        return false;
    }

    public IAttackStrategy[] getAvailableSpecialAttacks()
    {
        List<IAttackStrategy> availableSpecialAttacks = new List<IAttackStrategy>();

        // Ư�� ���� �� ��Ÿ���� ���� ������ ���͸��Ͽ� �߰�
        foreach (IAttackStrategy specialAttack in specialAttackStrategies)
        {
            if (specialAttack != null && specialAttack.getCurrentCooldown() == 0)
            {
                availableSpecialAttacks.Add(specialAttack);
            }
        }

        return availableSpecialAttacks.ToArray();
    }


    public int getSpecialAttackCount()
    {
        return specialAttackStrategies.Length;
    }


    public void AddObserver(stateObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(stateObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.StateUpdate();
        }
    }

    public Summon Clone() //���� ����
    {
        return (Summon)this.MemberwiseClone();
    }
}