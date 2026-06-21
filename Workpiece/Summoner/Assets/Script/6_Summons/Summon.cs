using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: 소환수 등급과 적 등급을 구분하는 값이다.
public enum SummonRank
{
    Low, Medium, High, // 아군 소환수 등급
    Normal, Special, Boss // 적 소환수 등급
}

[RequireComponent(typeof(SummonStatusView))]
[RequireComponent(typeof(SummonSoundView))]
[RequireComponent(typeof(SummonImageView))]
// 역할: 소환수의 체력, 공격, 상태 효과, UI 알림을 관리하는 기본 전투 유닛이다.
public class Summon : MonoBehaviour, UpdateStateObserver, IStatusEffectTarget
{
    [SerializeField] private SummonData summonData;
    [SerializeField] private GameObject shieldImage;
    [SerializeField] private Animator animator;

    private string summonName; // 이름
    [SerializeField] private Sprite normalAttackSprite; // 일반 공격 스프라이트
    [SerializeField] private Sprite specialAttackSprite; // 특수 공격 스프라이트
    [SerializeField] private double attackPower; // 일반 공격력
    [SerializeField] private double heavyAttakPower; // 강공격력
    private SummonRank summonRank; // 등급
    private double maxHP; // 최대 체력
    [SerializeField] protected double nowHP; // 현재 체력
    private double shield = 0; // 보호막
    private double initialShield; // 초기 보호막 값
    private bool onceInvincibility = false;
    [SerializeField] private bool isAttack = true; // 상태이상 중 공격 가능 여부

    private IAttackStrategy attackStrategy;
    private IAttackStrategy[] specialAttackStrategies;
    private StatusEffectState statusEffectState;
    private SummonImageView imageView;
    private SummonStatusView statusView;
    private SummonSoundView soundView;
    private Action<Summon> deathHandler;

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
        statusEffectState = new StatusEffectState();
    }

    void Update()
    {
        StatusEffectStateEnsure();

        statusView.StatusEffectsShow(statusEffectState.GetActiveStatusEffects());
    }

    public void SetSprite(int index) => imageView.SpriteSet(index);
    public Sprite GetNormalAttackSprite() => normalAttackSprite;
    public Sprite GetSpecialAttackSprite() => specialAttackSprite;


    public void NormalAttack(IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
    {
        if (!AttackCanUse(attackStrategy))
        {
            Debug.Log("일반 공격은 쿨타임 중이라 사용할 수 없습니다.");
            return;
        }
        attackStrategy.Attack(this, targetPlates, selectedPlateIndex); // 일반 공격 실행
        AttackMotionPlay(true);
        AttackCooldownApply(attackStrategy);
        isAttack = false;
    }

    public virtual void SpecialAttack(IReadOnlyList<Plate> targetPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
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

        SpecialAttackExecute(specialAttack, targetPlates, selectedPlateIndex);
    }

    protected void SpecialAttackExecute(
        IAttackStrategy specialAttack,
        IReadOnlyList<Plate> targetPlates,
        int selectedPlateIndex)
    {
        specialAttack.Attack(this, targetPlates, selectedPlateIndex);
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
        StatusEffectStateEnsure();

        statusEffectState.StatusEffectApply(statusEffect, this);
    }

    private void StatusEffectStateEnsure()
    {
        if (statusEffectState == null)
        {
            statusEffectState = new StatusEffectState();
        }
    }

    public void UpdateStatusEffects(StatusUpdateTiming updateTiming)
    {
        StatusEffectStateEnsure();

        statusEffectState.UpdateStatusEffects(updateTiming, this);
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
            Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 특수 공격 남은 쿨타임: {specialAttack.GetCurrentCooldown()}턴");
            return;
        }

        Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 특수 공격 쿨타임이 종료되었습니다.");
    }

    public bool GetIsAttack() => isAttack;
    public void SetIsAttack(bool isAttack) => this.isAttack = isAttack;

    public string GetStatusTargetName() => summonName;
    public double GetHealth() => nowHP;
    public void DamageTake(double damage) => TakeDamage(damage);
    public void HealReceive(double healAmount) => Heal(healAmount);
    public void SetAttackAvailable(bool canAttack) => SetIsAttack(canAttack);
    public void ShieldAdd(double shieldAmount) => AddShield(shieldAmount);
    public void SetShield(double shieldAmount) => shield = shieldAmount;
    public void AttackPowerUpgrade(double multiplier) => UpgradeAttackPower(multiplier);
    public void AttackPowerCurse(double curseRate) => Cursed(curseRate);
    public void AttackPowerRestore(double originAttack) => attackPower = originAttack;
    public void StatusHitColorShow() => statusView.StatusHitColorShow();

    public void DebuffSoundPlay() => soundView.DebuffSoundPlay();

    public void BuffSoundPlay() => soundView.BuffSoundPlay();

    public void AttackSoundPlay() => soundView.AttackSoundPlay();

    public void StatusChangedNotify() => NotifyObservers();

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


    public virtual void TakeDamage(double damage) // 피해 받기
    {
        damage = DamageRoundDown(damage);

        if (OnceInvincibilityTryBlock())
        {
            return;
        }

        DamageApply(damage);
        DeathHandle(damage);
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

    private void DamageApply(double damage)
    {
        if (shield > 0)
        {
            ShieldDamageApply(damage);
            return;
        }

        HealthDamageApply(damage);
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

        StatusEffectStateEnsure();
        statusEffectState.StatusEffectRemove(StatusType.Shield, this);
        Debug.Log("보호막이 깨졌습니다. 남은 체력: " + nowHP);
    }

    private void HealthDamageApply(double damage)
    {
        nowHP -= damage;
        animator.SetTrigger("hitted");
        statusView.DamageColorShow();
    }

    private void DeathHandle(double damage)
    {
        if (nowHP > 0)
        {
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
            return;
        }

        nowHP = 0;
        Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
        Die();
    }

    // 소환수 초기화 메서드
    public virtual void SummonInitialize()
    {
        if (!TryApplyAssignedSummonData())
        {
            ApplyFallbackData();
        }

        NotifyObservers();
    }

    protected virtual void ApplyFallbackData()
    {
    }

    protected bool TryApplyAssignedSummonData()
    {
        if (summonData == null)
        {
            return false;
        }

        summonName = summonData.GetSummonName();
        summonRank = summonData.GetSummonRank();
        maxHP = summonData.GetMaxHp();
        nowHP = maxHP;
        attackPower = summonData.GetAttackPower();
        heavyAttakPower = summonData.GetHeavyAttackPower();
        attackStrategy = summonData.CreateNormalAttackStrategy();
        specialAttackStrategies = summonData.CreateSpecialAttackStrategies();
        return true;
    }

    protected void SetFallbackStatus(
        string name,
        SummonRank rank,
        double maxHp,
        double normalAttackPower,
        double heavyAttackPower)
    {
        summonName = name;
        summonRank = rank;
        maxHP = maxHp;
        nowHP = maxHP;
        attackPower = normalAttackPower;
        heavyAttakPower = heavyAttackPower;
    }

    protected void SetAttackStrategies(IAttackStrategy normalAttack, params IAttackStrategy[] specialAttacks)
    {
        attackStrategy = normalAttack;
        specialAttackStrategies = specialAttacks;
    }

    private static double multiple = 1; // 배수 설정
    public static double GetStatMultiplier() => multiple;
    public static void StatMultiplierSet(double value) => multiple = value;

    public virtual void ApplayMultiple(double m)
    {
        maxHP = (int)(maxHP * m);
        nowHP = maxHP;
        attackPower = (int)(attackPower * m); // 일반 공격 배수 적용
        heavyAttakPower = (int)(heavyAttakPower * m); // 강공격 배수 적용
    }


    public virtual void Die()
    {
        Debug.Log($"{summonName}의 체력이 모두 소진되어 사라집니다.");
        deathHandler?.Invoke(this);

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
    public double GetShield() => shield; // 현재 보호막 값을 반환
    public double GetInitialShield() => initialShield; // 초기 보호막 값을 반환

    public string GetSummonName() => summonName;

    public double GetHeavyAttackPower() => heavyAttakPower;

    public IAttackStrategy[] GetSpecialAttackStrategy() => specialAttackStrategies;
    public IAttackStrategy GetAttackStrategy() => attackStrategy;

    public bool GetInvincibilityOnce() => onceInvincibility;
    public void SetOnceInvincibility(bool isinvincibility) => this.onceInvincibility = isinvincibility;

    public double GetMaxHP() => maxHP;

    // nowHP 관련 메서드
    public void SetNowHP(double hp) => this.nowHP = hp;
    public double GetNowHP() => nowHP;

    // attackPower 관련 메서드
    public void SetAttackPower(double power) => this.attackPower = power;
    public double GetAttackPower() => attackPower;

    public SummonRank GetSummonRank() => summonRank;
    public SummonRank GetDrawRank()
    {
        return summonData == null ? summonRank : summonData.GetSummonRank();
    }

    public Image GetImage()
    {
        ImageViewEnsure();
        return imageView.GetImage();
    }

    private void ImageViewEnsure()
    {
        if (imageView != null)
        {
            return;
        }

        imageView = GetComponent<SummonImageView>();
        if (imageView == null)
        {
            imageView = gameObject.AddComponent<SummonImageView>();
        }
    }


    // 특수 공격 쿨타임 확인
    public bool IsSpecialAttackCool(IAttackStrategy specialAttack)
    {
        return AttackCooldownCheck(specialAttack);
    }

    protected bool AttackCanUse(IAttackStrategy attackStrategy)
    {
        return attackStrategy != null && !AttackCooldownCheck(attackStrategy);
    }

    private bool AttackCooldownCheck(IAttackStrategy attackStrategy)
    {
        return attackStrategy != null && attackStrategy.GetCurrentCooldown() > 0;
    }

    public List<StatusType> GetAllStatusTypes()
    {
        StatusEffectStateEnsure();

        return statusEffectState.GetStatusTypes();
    }

    public IReadOnlyList<StatusEffect> GetActiveStatusEffects()
    {
        StatusEffectStateEnsure();

        return statusEffectState.GetActiveStatusEffects();
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
        StatusEffectStateEnsure();

        return statusEffectState.StatusTypeContains(statusType);
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

    public IAttackStrategy[] GetAvailableSpecialAttacks()
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


    public int GetSpecialAttackCount() => specialAttackStrategies == null ? 0 : specialAttackStrategies.Length;

    public void SetDeathHandler(Action<Summon> deathHandler) => this.deathHandler = deathHandler;

    public void ClearDeathHandler() => deathHandler = null;

    public void AddObserver(stateObserver observer) => observers.Add(observer);

    public void RemoveObserver(stateObserver observer) => observers.Remove(observer);

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.StateUpdate();
        }
    }

    public Summon Clone()
    {
        Summon clone = (Summon)this.MemberwiseClone(); // 얕은 복사
        clone.deathHandler = null;
        return clone;
    }
}
