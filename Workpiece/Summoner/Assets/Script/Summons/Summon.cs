using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High, // 아군 소환수 등급
    Normal, Special, Boss // 적 소환수 등급
}

public enum SummonType
{
    Cat, Rabbit, Wolf, Eagle, Snake, Fox
}

[RequireComponent(typeof(SummonStatusView))]
[RequireComponent(typeof(SummonSoundView))]
[RequireComponent(typeof(SummonImageView))]
public class Summon : MonoBehaviour, UpdateStateObserver, IStatusEffectTarget
{
    [SerializeField] private SummonData summonData;
    [SerializeField] protected GameObject shieldImage;
    [SerializeField] protected Animator animator;

    protected string summonName; // 이름
    public Sprite normalAttackSprite; // 일반 공격 스프라이트
    public Sprite specialAttackSprite; // 특수 공격 스프라이트
    public double attackPower; // 일반 공격력
    public double heavyAttakPower; // 강공격력
    protected SummonRank summonRank; // 등급
    protected SummonType summonType;
    protected double maxHP; // 최대 체력
    public double nowHP; // 현재 체력
    protected double shield = 0; // 보호막
    private double initialShield; // 초기 보호막 값
    protected bool onceInvincibility = false;
    public bool isAttack = true; // 상태이상 중 공격 가능 여부

    protected IAttackStrategy attackStrategy;
    protected IAttackStrategy[] specialAttackStrategies;
    private StatusEffectController statusEffectController;
    private SummonImageView imageView;
    private SummonStatusView statusView;
    private SummonSoundView soundView;

    private List<stateObserver> observers = new List<stateObserver>();


    protected virtual void Awake()
    {
        imageView = GetComponent<SummonImageView>();
        if (imageView == null)
        {
            imageView = gameObject.AddComponent<SummonImageView>();
        }

        statusView = GetComponent<SummonStatusView>();
        if (statusView == null)
        {
            statusView = gameObject.AddComponent<SummonStatusView>();
        }

        soundView = GetComponent<SummonSoundView>();
        if (soundView == null)
        {
            soundView = gameObject.AddComponent<SummonSoundView>();
        }

        nowHP = maxHP;
        statusEffectController = new StatusEffectController();
    }

    void Update()
    {
        StatusEffectControllerEnsure();

        statusView.StatusEffectsShow(statusEffectController.ActiveStatusEffectsGet());
    }

    public void SetSprite(int index) => imageView.SpriteSet(index);


    public void normalAttack(List<Plate> targetPlates, int selectedPlateIndex)
    {
        if (!AttackCanUse(attackStrategy))
        {
            Debug.Log("일반 공격은 쿨타임 중이라 사용할 수 없습니다.");
            return;
        }
        attackStrategy.Attack(this, targetPlates, selectedPlateIndex, 0); // 일반 공격 실행
        AttackMotionPlay(true);
        AttackCooldownApply(attackStrategy);
        isAttack = false;
    }

    public virtual void SpecialAttack(List<Plate> targetPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (!SpecialAttackIndexCheck(SpecialAttackArrayIndex))
        {
            Debug.Log("유효하지 않은 특수 공격 인덱스입니다.");
            return;
        }

        var specialAttack = specialAttackStrategies[SpecialAttackArrayIndex];
        
        if (!AttackCanUse(specialAttack))
        {
            Debug.Log("특수 공격은 쿨타임 중입니다.");
            return;
        }

        // 공격 실행
        specialAttack.Attack(this, targetPlates, selectedPlateIndex, SpecialAttackArrayIndex);
        AttackMotionPlay(false);
        AttackCooldownApply(specialAttack);
        isAttack = false;
    }

    protected void AttackMotionPlay(bool shouldPlayAttackSound)
    {
        animator.SetTrigger("attack");

        if (shouldPlayAttackSound)
        {
            soundView.AttackSoundPlay();
        }

        statusView.AttackColorShow();
    }

    protected void AttackCooldownApply(IAttackStrategy attackStrategy)
    {
        attackStrategy.ApplyCooldown();
    }

    protected bool SpecialAttackIndexCheck(int specialAttackArrayIndex)
    {
        if (specialAttackStrategies == null)
        {
            return false;
        }

        return specialAttackArrayIndex >= 0 && specialAttackArrayIndex < specialAttackStrategies.Length;
    }

    // 상태이상을 적용한다. 같은 상태이상은 중복 적용하지 않는다.
    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        StatusEffectControllerEnsure();

        statusEffectController.StatusEffectApply(statusEffect, this);
    }

    private void StatusEffectControllerEnsure()
    {
        if (statusEffectController == null)
        {
            statusEffectController = new StatusEffectController();
        }
    }

    // 피해를 주는 상태이상을 업데이트한다. 예: Poison, Burn
    public void UpdateDamageStatusEffects()
    {
        StatusEffectControllerEnsure();

        statusEffectController.DamageStatusEffectsUpdate(this);
    }

    // 스턴과 저주 상태를 업데이트한다.
    public void UpdateStunAndCurseStatus()
    {
        StatusEffectControllerEnsure();

        statusEffectController.StunAndCurseStatusUpdate(this);
    }

    // 강화 상태를 업데이트한다.
    public void UpdateUpgradeStatus()
    {
        StatusEffectControllerEnsure();

        statusEffectController.UpgradeStatusUpdate(this);
    }



    // 특수 공격 쿨타임을 업데이트한다.
    public void UpdateSpecialAttackCooldowns()
    {
        if (specialAttackStrategies == null) return; // 배열이 null인 경우 체크

        foreach (IAttackStrategy specialAttack in specialAttackStrategies)
        {
            SpecialAttackCooldownUpdate(specialAttack);
        }
    }

    private void SpecialAttackCooldownUpdate(IAttackStrategy specialAttack)
    {
        if (specialAttack == null)
        {
            return;
        }

        if (AttackCooldownCheck(specialAttack))
        {
            specialAttack.ReduceCooldown();
            Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 특수 공격 남은 쿨타임: {specialAttack.getCurrentCooldown()}턴");
            return;
        }

        Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 특수 공격 쿨타임이 종료되었습니다.");
    }

    public bool getIsAttack() => isAttack;
    public void setIsAttack(bool isAttack) => this.isAttack = isAttack;

    public string StatusTargetNameGet() => summonName;
    public double HealthGet() => nowHP;
    public double AttackPowerGet() => attackPower;
    public void DamageTake(double damage) => takeDamage(damage);
    public void HealReceive(double healAmount) => Heal(healAmount);
    public void AttackAvailableSet(bool canAttack) => setIsAttack(canAttack);
    public void ShieldAdd(double shieldAmount) => AddShield(shieldAmount);
    public void ShieldSet(double shieldAmount) => shield = shieldAmount;
    public void AttackPowerUpgrade(double multiplier) => UpgradeAttackPower(multiplier);
    public void AttackPowerCurse(double curseRate) => Cursed(curseRate);
    public void AttackPowerRestore(double originAttack) => attackPower = originAttack;
    public void OnceInvincibilitySet(bool isInvincibility) => setOnceInvincibility(isInvincibility);
    public void StatusHitColorShow() => statusView.StatusHitColorShow();

    public void DebuffSoundPlay() => soundView.DebuffSoundPlay();

    public void BuffSoundPlay() => soundView.BuffSoundPlay();

    public void AttackSoundPlay() => soundView.AttackSoundPlay();

    public void StatusChangedNotify() => NotifyObservers();

    public SummonType getSummonType() => summonType;
    

    public void UpgradeAttackPower(double multiplier)
    {
        attackPower *= (1 + multiplier);
        attackPower = Math.Floor(attackPower); // 소수점 아래를 버림

        attackPower = (int)attackPower; // double을 int로 변환
        Debug.Log($"{summonName}의 공격력이 {multiplier * 100}% 강화되었습니다. 현재 공격력: {attackPower}");
    }

    public void Cursed(double curse)
    {
        attackPower *= (1 - curse);
        Debug.Log($"{summonName}의 공격력이 {curse * 100}% 감소했습니다. 현재 공격력: {attackPower}");
    }


    // 체력 회복
    public void Heal(double healAmount)
    {
        nowHP += healAmount;
        if(nowHP >= maxHP)
        {
            nowHP = maxHP;
        }
        Debug.Log($"{summonName}이 {healAmount}만큼 체력을 회복했습니다.");
        // 체력 변경을 옵저버에게 알림
        NotifyObservers();
        animator.SetTrigger("hitted");
        statusView.HealColorShow();
        soundView.BuffSoundPlay();
    }


    public virtual void takeDamage(double damage) // 피해 받기
    {
        damage = DamageRoundDown(damage);

        if (OnceInvincibilityTryBlock())
        {
            return;
        }

        if (shield > 0) // 보호막이 있으면 피해를 먼저 막음
        {
            ShieldDamageApply(damage);
        }
        else // 보호막이 없는 경우
        {
            HealthDamageApply(damage);
        }

        DeathCheck(damage);

        // 체력 변경을 옵저버에게 알림
        NotifyObservers();
    }

    private double DamageRoundDown(double damage)
    {
        damage = (int)damage; // 피해량의 소수점 제거
        return Math.Floor(damage); // 소수점 아래를 버림
    }

    private bool OnceInvincibilityTryBlock()
    {
        if (onceInvincibility == false)
        {
            return false;
        }

        onceInvincibility = false;
        Debug.Log("1회 무적 보호막으로 공격을 막았습니다.");
        return true;
    }

    private void ShieldDamageApply(double damage)
    {
        if (shield >= damage)
        {
            shield -= damage;
            animator.SetTrigger("hitted");
            Debug.Log("보호막으로 피해를 막았습니다. 남은 보호막: " + shield);
            return;
        }

        double remainingDamage = damage - shield;
        shield = 0;
        nowHP -= remainingDamage;
        animator.SetTrigger("hitted");
        shieldImage.SetActive(false);

        StatusEffectControllerEnsure();
        statusEffectController.StatusEffectRemove(StatusType.Shield, this);
        Debug.Log("보호막이 깨졌습니다. 남은 체력: " + nowHP);
    }

    private void HealthDamageApply(double damage)
    {
        nowHP -= damage;
        animator.SetTrigger("hitted");
        statusView.DamageColorShow();
    }

    private void DeathCheck(double damage)
    {
        if (nowHP <= 0)
        {
            nowHP = 0;
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
            die();
            return;
        }

        Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
    }

    // 소환수 초기화 메서드
    public virtual void summonInitialize()
    {
        NotifyObservers();
    }

    protected SummonData SummonDataGet() => summonData;

    protected bool SummonDataApply(SummonData data)
    {
        if (data == null)
        {
            return false;
        }

        summonName = data.SummonNameGet();
        summonRank = data.SummonRankGet();
        summonType = data.SummonTypeGet();
        maxHP = data.MaxHpGet();
        nowHP = maxHP;
        attackPower = data.AttackPowerGet();
        heavyAttakPower = data.HeavyAttackPowerGet();
        attackStrategy = data.NormalAttackStrategyCreate();
        specialAttackStrategies = data.SpecialAttackStrategiesCreate();
        NotifyObservers();
        return true;
    }

    public static double multiple=5; // 배수 설정
    public virtual void ApplayMultiple(double m)
    {
        maxHP = (int)(maxHP * m);
        nowHP = maxHP;
        attackPower = (int)(attackPower * m); // 일반 공격 배수 적용
        heavyAttakPower = (int)(heavyAttakPower * m); // 강공격 배수 적용
    }


    public virtual void die()
    {
        Debug.Log($"{summonName}의 체력이 모두 소진되어 사라집니다.");
        // Plate에서 소환수를 제거하기 위해 부모 Plate를 가져옴
        Plate plate = GetComponentInParent<Plate>(); // 소환수가 배치된 부모 Plate 가져오기
        if (plate != null)
        {
            plate.RemoveSummon(); // 소환수 제거
        }

        // 소환수 오브젝트 제거
        Destroy(gameObject); // 씬에서 소환수 오브젝트 제거

    }


    public void AddShield(double shieldAmount)
    {
        if (shield == 0) // 현재 보호막이 0일 때만 초기값 설정
        {
            initialShield = shieldAmount;
        }
        shield += shieldAmount;
        shieldImage.SetActive(true);
        Debug.Log("보호막 부여. 현재 보호막: " + shield);
        NotifyObservers();
    }
    public double getShield() => shield; // 현재 보호막 값을 반환
    public double GetInitialShield() => initialShield; // 초기 보호막 값을 반환

    public string getSummonName() => summonName;
    public void setSummonName(string name) => this.summonName = name;

    public double getHeavyAttackPower() => heavyAttakPower;
    public void setHeavyAttackPower(double value) => this.heavyAttakPower = value;

    public IAttackStrategy[] getSpecialAttackStrategy() => specialAttackStrategies;
    public IAttackStrategy getAttackStrategy() => attackStrategy;

    public bool getInvincibilityOnce() => onceInvincibility;
    public void setOnceInvincibility(bool isinvincibility) => this.onceInvincibility = isinvincibility;

    public void setMaxHP(double hp) => this.maxHP = hp;
    public double getMaxHP() => maxHP;

    // nowHP 관련 메서드
    public void setNowHP(double hp) => this.nowHP = hp;
    public double getNowHP() => nowHP;

    // attackPower 관련 메서드
    public void setAttackPower(double power) => this.attackPower = power;
    public double getAttackPower() => attackPower;

    public SummonRank getSummonRank() => summonRank;
    public void setSummonRank(SummonRank rank) => this.summonRank = rank;

    public void setImage(Image image)
    {
        imageView.ImageSet(image);
    }

    public Image getImage() => imageView.ImageGet();


    // 특수 공격 쿨타임 확인
    public bool isSpecialAttackCool(IAttackStrategy specialAttack)
    {
        return AttackCooldownCheck(specialAttack);
    }

    protected bool AttackCanUse(IAttackStrategy attackStrategy)
    {
        return attackStrategy != null && !AttackCooldownCheck(attackStrategy);
    }

    private bool AttackCooldownCheck(IAttackStrategy attackStrategy)
    {
        return attackStrategy != null && attackStrategy.getCurrentCooldown() > 0;
    }

    public List<StatusType> getAllStatusTypes()
    {
        StatusEffectControllerEnsure();

        return statusEffectController.StatusTypesGet();
    }

    public bool IsCursed()
    {
        return StatusTypeContains(StatusType.Curse);
    }

    public bool IsStun()
    {
        return StatusTypeContains(StatusType.Stun);
    }

    private bool StatusTypeContains(StatusType statusType)
    {
        StatusEffectControllerEnsure();

        return statusEffectController.StatusTypeContains(statusType);
    }

    public bool IsCooltime() // 쿨타임인지 확인
    {
        // 먼저 특수 공격 전략 배열이 있는지 확인
        if (specialAttackStrategies == null || specialAttackStrategies.Length == 0)
        {
            Debug.Log("특수 공격이 없습니다.");
            return false;
        }

        // 각 특수 공격 전략의 현재 쿨타임 여부 확인
        foreach (var specialAttack in specialAttackStrategies)
        {
            if (AttackCooldownCheck(specialAttack))
            {
                // 하나라도 쿨타임 중인 전략이 있으면 true 반환
                Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 특수 공격은 쿨타임 중입니다.");
                return true;
            }
        }
        return false;
    }

    public IAttackStrategy[] getAvailableSpecialAttacks()
    {
        List<IAttackStrategy> availableSpecialAttacks = new List<IAttackStrategy>();

        if (specialAttackStrategies == null)
        {
            return availableSpecialAttacks.ToArray();
        }

        // 특수 공격 중 쿨타임이 없는 공격만 필터링하여 추가
        foreach (IAttackStrategy specialAttack in specialAttackStrategies)
        {
            if (AttackCanUse(specialAttack))
            {
                availableSpecialAttacks.Add(specialAttack);
            }
        }

        return availableSpecialAttacks.ToArray();
    }


    public int getSpecialAttackCount() => specialAttackStrategies == null ? 0 : specialAttackStrategies.Length;

    public void AddObserver(stateObserver observer) => observers.Add(observer);

    public void RemoveObserver(stateObserver observer) => observers.Remove(observer);

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.StateUpdate();
        }
    }

    public Summon Clone() => (Summon)this.MemberwiseClone(); // 얕은 복사
}
