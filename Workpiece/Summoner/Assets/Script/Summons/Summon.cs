using System;
using System.Collections;
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

public class Summon : MonoBehaviour, UpdateStateObserver, IStatusEffectTarget
{
    [SerializeField] protected Image image; // 이미지
    [SerializeField] protected Sprite[] sprites; // 스프라이트 목록
    [SerializeField] protected GameObject shieldImage;
    [SerializeField] protected Animator animator;

    [Header("효과음")]
    [SerializeField] public AudioSource attackSound;
    [SerializeField] private AudioSource downHitSound;
    [SerializeField] private AudioSource upAttackSound;

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

    private bool attakingMotion = false;
    private int currentEffectIndex = 0;
    private float blinkTimer = 0f;
    private float blinkInterval = 1f; // 색상 변경 간격

    [Header("상태이상")]
    [SerializeField] private List<StatusEffect> activeStatusEffects = new List<StatusEffect>(); // 현재 적용된 상태이상
    protected IAttackStrategy attackStrategy;
    protected IAttackStrategy[] specialAttackStrategies;
    private StatusEffectController statusEffectController;

    private List<stateObserver> observers = new List<stateObserver>();


    private void Awake()
    {
        image = GetComponent<Image>();
        nowHP = maxHP;
        statusEffectController = new StatusEffectController(activeStatusEffects);
    }

    void Update()
    {
        if(!attakingMotion) ApplyStatusEffectBlink();
    }

    public void SetSprite(int index) => image.sprite = sprites[index];


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

    protected IEnumerator ColorChange(int color)    // 색이 바뀐 뒤 원래 색으로 돌아옴
    {
        //
        attakingMotion = true;
        switch (color)
        {
            case 1: // 검은색
                image.color = new Color(0f, 0f, 0f); // #000000
                break;
            case 2: // 빨간색
                image.color = new Color(1f, 0.431f, 0.431f); // #FF6E6E
                break;
            case 3: // 보라색
                image.color = new Color(0.639f, 0.192f, 0.839f); // #A331D6
                break;
            case 4: // 초록색
                image.color = new Color(0.192f, 0.835f, 0.318f); // #31D551
                break;
            default:
                image.color = Color.white; // 기본값 설정
                break;

        }

        yield return new WaitForSeconds(1f);

        image.color = Color.white;
        attakingMotion = false;
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

    protected void AttackMotionPlay(bool attackSoundPlay)
    {
        animator.SetTrigger("attack");

        if (attackSoundPlay)
        {
            attackSound.Play();
        }

        StartCoroutine(ColorChange(1)); // 검은색
    }

    protected void AttackCooldownApply(IAttackStrategy attackStrategy)
    {
        attackStrategy.ApplyCooldown();
    }

    protected bool SpecialAttackIndexCheck(int specialAttackArrayIndex)
    {
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
            statusEffectController = new StatusEffectController(activeStatusEffects);
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

    private void SetColorByStatus(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.Burn:
                image.color = new Color(1f, 0.3f, 0.3f); // 붉은색(Burn)
                break;
            case StatusType.Poison:
                image.color = new Color(0.3f, 1f, 0.3f); // 녹색(Poison)
                break;
            case StatusType.Stun:
                image.color = new Color(0.2f, 0.2f, 0.2f); // 검은색(Stun)
                break;
            case StatusType.LifeDrain:
                image.color = new Color(1f, 1f, 0.5f); // 노란색(LifeDrain)
                break;
            default:
                image.color = Color.white; // 기본 색상
                break;
        }
    }


    private void ApplyStatusEffectBlink()
    {
        if (StatusColorResetIfEmpty())
        {
            return;
        }

        if (activeStatusEffects.Count == 1)
        {
            SingleStatusColorShow();
        }
        else
        {
            MultipleStatusColorBlink();
        }
    }

    private bool StatusColorResetIfEmpty()
    {
        if (activeStatusEffects.Count > 0)
        {
            return false;
        }

        image.color = Color.white;
        return true;
    }

    private void SingleStatusColorShow() => SetColorByStatus(activeStatusEffects[0].statusType);

    private void MultipleStatusColorBlink()
    {
        blinkTimer += Time.deltaTime;

        if (blinkTimer < blinkInterval)
        {
            return;
        }

        blinkTimer = 0f;
        currentEffectIndex = (currentEffectIndex + 1) % activeStatusEffects.Count;
        SetColorByStatus(activeStatusEffects[currentEffectIndex].statusType);
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
    public void StatusHitColorShow() => StartCoroutine(ColorChange(3));

    public void DebuffSoundPlay()
    {
        if (downHitSound != null)
        {
            downHitSound.Play();
        }
    }

    public void BuffSoundPlay()
    {
        if (upAttackSound != null)
        {
            upAttackSound.Play();
        }
    }

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
        StartCoroutine(ColorChange(4)); // 초록색
        upAttackSound.Play();
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
        StartCoroutine((ColorChange(2)));   // 빨간색
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

    public void setImage(Image image) => this.image = image;
    public Image getImage() => image;


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
        List<StatusType> statusTypes = new List<StatusType>();

        // activeStatusEffects 리스트에서 각 StatusEffect의 statusType을 추가
        foreach (StatusEffect effect in activeStatusEffects)
        {
            statusTypes.Add(effect.statusType);
        }

        return statusTypes;
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
        foreach (StatusEffect effect in activeStatusEffects)
        {
            if (effect.statusType == statusType)
            {
                return true;
            }
        }

        return false;
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


    public int getSpecialAttackCount() => specialAttackStrategies.Length;

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
