using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: 소환수 등급 값을 정의한다.
public enum SummonRank
{
    Low, Medium, High,
    Normal, Special, Boss
}

[RequireComponent(typeof(SummonStatusView))]
[RequireComponent(typeof(SummonSoundView))]
[RequireComponent(typeof(SummonImageView))]
// 역할: 소환수의 전투 스탯, 공격 실행, 상태 효과, 사망 처리를 관리한다.
public class Summon : MonoBehaviour, IStatusTarget
{
    [SerializeField] private SummonData summonData;
    [SerializeField] private GameObject shieldImage;
    [SerializeField] private Animator animator;

    [SerializeField] private Sprite normalAttackSprite;
    [SerializeField] private Sprite specialAttackSprite;
    private SummonEntity entity;
    private SummonImageView imageView;
    private SummonStatusView statusView;
    private SummonSoundView soundView;
    private Action<Summon> deathHandler;
    private Action stateChanged;


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

        EntityEnsure();
    }

    void Update()
    {
        EntityEnsure();

        statusView.ShowStatuses(entity.GetActiveStatuses());
    }

    public void SetSprite(int index) => imageView.SpriteSet(index);
    public Sprite GetNormalAttackSprite() => normalAttackSprite;
    public Sprite GetSpecialAttackSprite() => specialAttackSprite;

    public Sprite GetSpecialAttackSprite(int specialAttackIndex)
    {
        SummonAttackData attackData = GetSpecialAttackData(specialAttackIndex);
        Sprite displaySprite = attackData == null ? null : attackData.GetDisplaySprite();
        return displaySprite == null ? specialAttackSprite : displaySprite;
    }

    public string GetSpecialAttackName(int specialAttackIndex)
    {
        SummonAttackData attackData = GetSpecialAttackData(specialAttackIndex);
        return attackData == null ? "SpecialAttack_" + specialAttackIndex : attackData.GetDisplayName();
    }

    public string GetSpecialAttackTooltip(int specialAttackIndex)
    {
        SummonAttackData attackData = GetSpecialAttackData(specialAttackIndex);
        return attackData == null ? GetSpecialAttackName(specialAttackIndex) : attackData.GetTooltipText();
    }


    public void NormalAttack(IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        if (!AttackCanUse(entity.NormalAttack))
        {
            Debug.Log("일반 공격이 쿨타임 중이라 사용할 수 없습니다.");
            return;
        }
        entity.NormalAttack.SelectTargets(this, targetPlates, selectedPlateIndex);
        AttackMotionPlay(true);
        AttackCooldownApply(entity.NormalAttack);
        entity.SetAttackAvailable(false);
    }

    public virtual void SpecialAttack(IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (!SpecialAttackIndexCheck(SpecialAttackArrayIndex))
        {
            Debug.Log("유효하지 않은 특수 공격 인덱스입니다.");
            return;
        }

        var specialAttack = entity.SpecialAttacks[SpecialAttackArrayIndex];
        
        if (!AttackCanUse(specialAttack))
        {
            Debug.Log("특수 공격이 쿨타임 중입니다.");
            return;
        }

        SpecialAttackExecute(specialAttack, targetPlates, selectedPlateIndex);
    }

    protected void SpecialAttackExecute(
        AttackData specialAttack,
        IReadOnlyList<BattleBoardInputController> targetPlates,
        int selectedPlateIndex)
    {
        specialAttack.SelectTargets(this, targetPlates, selectedPlateIndex);
        AttackMotionPlay(false);
        AttackCooldownApply(specialAttack);
        entity.SetAttackAvailable(false);
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

    protected void AttackCooldownApply(AttackData attackStrategy)
    {
        attackStrategy.ApplyCooldown();
    }

    protected bool SpecialAttackIndexCheck(int specialAttackArrayIndex)
    {
        if (entity.SpecialAttacks == null)
        {
            return false;
        }

        return specialAttackArrayIndex >= 0 && specialAttackArrayIndex < entity.SpecialAttacks.Length;
    }

    public void ApplyStatus(StatusData statusEffect)
    {
        EntityEnsure();

        entity.ApplyStatus(statusEffect, this);
    }

    private void EntityEnsure()
    {
        if (entity == null)
        {
            entity = new SummonEntity();
        }
    }

    public void UpdateStatus(StatusTiming updateTiming)
    {
        EntityEnsure();

        entity.UpdateStatus(updateTiming, this);
    }



    public void UpdateSpecialAttackCooldowns()
    {
        EntityEnsure();
        entity.UpdateSpecialAttackCooldowns(Debug.Log);
    }

    public bool GetIsAttack() => entity.CanAttack;
    public void SetIsAttack(bool isAttack) => entity.SetAttackAvailable(isAttack);

    public string GetStatusTargetName() => entity.Name;
    public double GetHealth() => entity.CurrentHp;
    public void DamageTake(double damage) => TakeDamage(damage);
    public void HealReceive(double healAmount) => Heal(healAmount);
    public void SetAttackAvailable(bool canAttack) => entity.SetAttackAvailable(canAttack);
    public void ShieldAdd(double shieldAmount) => AddShield(shieldAmount);
    public void SetShield(double shieldAmount) => entity.SetShield(shieldAmount);
    public void AttackPowerUpgrade(double multiplier) => UpgradeAttackPower(multiplier);
    public void AttackPowerCurse(double curseRate) => Cursed(curseRate);
    public void AttackPowerRestore(double originAttack) => entity.RestoreAttackPower(originAttack);
    public void StatusHitColorShow() => statusView.StatusHitColorShow();

    public void DebuffSoundPlay() => soundView.DebuffSoundPlay();

    public void BuffSoundPlay() => soundView.BuffSoundPlay();

    public void AttackSoundPlay() => soundView.AttackSoundPlay();

    public bool CanUseAttackStrategy(AttackData attackStrategy) => AttackCanUse(attackStrategy);

    public void PlayAttackMotion(bool shouldPlayAttackSound) => AttackMotionPlay(shouldPlayAttackSound);

    public void ApplyAttackCooldown(AttackData attackStrategy) => AttackCooldownApply(attackStrategy);

    public void StatusChangedNotify() => NotifyStateChanged();

    public void UpgradeAttackPower(double multiplier)
    {
        entity.ApplyAttackPowerUpgrade(multiplier);
        Debug.Log($"{entity.Name}의 공격력이 {multiplier * 100}% 강화되었습니다. 현재 공격력: {entity.AttackPower}");
    }

    public void Cursed(double curse)
    {
        entity.ApplyAttackPowerCurse(curse);
        Debug.Log($"{entity.Name}의 공격력이 {curse * 100}% 감소했습니다. 현재 공격력: {entity.AttackPower}");
    }


    public void Heal(double healAmount)
    {
        entity.Heal(healAmount);
        Debug.Log($"{entity.Name}이 {healAmount}만큼 체력을 회복했습니다.");
        NotifyStateChanged();
        animator.SetTrigger("hitted");
        statusView.HealColorShow();
        soundView.BuffSoundPlay();
    }


    public virtual void TakeDamage(double damage)
    {
        damage = DamageRoundDown(damage);

        if (OnceInvincibilityTryBlock())
        {
            return;
        }

        DamageApply(damage);
        DeathHandle(damage);
        NotifyStateChanged();
    }

    private double DamageRoundDown(double damage)
    {
        damage = (int)damage;
        return Math.Floor(damage);
    }

    private bool OnceInvincibilityTryBlock()
    {
        if (entity.HasOnceInvincibility == false)
        {
            return false;
        }

        entity.SetOnceInvincibility(false);
            Debug.Log("1회 무적 보호막으로 공격을 막았습니다.");
        return true;
    }

    private void DamageApply(double damage)
    {
        if (entity.Shield > 0)
        {
            ShieldDamageApply(damage);
            return;
        }

        HealthDamageApply(damage);
    }

    private void ShieldDamageApply(double damage)
    {
        if (entity.Shield >= damage)
        {
            entity.SetShield(entity.Shield - damage);
            animator.SetTrigger("hitted");
            Debug.Log("보호막으로 피해를 막았습니다. 남은 보호막: " + entity.Shield);
            return;
        }

        double remainingDamage = damage - entity.Shield;
        entity.SetShield(0);
        entity.TakeHealthDamage(remainingDamage);
        animator.SetTrigger("hitted");
        shieldImage.SetActive(false);

        EntityEnsure();
        entity.RemoveStatus(StatusType.Shield, this);
        Debug.Log("보호막이 깨졌습니다. 남은 체력: " + entity.CurrentHp);
    }

    private void HealthDamageApply(double damage)
    {
        entity.TakeHealthDamage(damage);
        animator.SetTrigger("hitted");
        statusView.DamageColorShow();
    }

    private void DeathHandle(double damage)
    {
        if (entity.CurrentHp > 0)
        {
            Debug.Log($"{entity.Name}이 {damage}의 피해를 받았습니다. 남은 체력: {entity.CurrentHp}");
            return;
        }

        entity.SetCurrentHp(0);
        Debug.Log($"{entity.Name}이 {damage}의 피해를 받았습니다. 남은 체력: {entity.CurrentHp}");
        Die();
    }

    public virtual void SummonInitialize()
    {
        if (!TryApplyAssignedSummonData())
        {
            ApplyFallbackData();
        }

        NotifyStateChanged();
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

        EntityEnsure();
        entity.SetBattleStats(
            summonData.GetSummonName(),
            summonData.GetSummonRank(),
            summonData.GetMaxHp(),
            summonData.GetAttackPower(),
            summonData.GetHeavyAttackPower());
        entity.SetAttackStrategies(
            summonData.CreateNormalAttackStrategy(),
            summonData.CreateSpecialAttackStrategies());
        return true;
    }

    protected void SetFallbackStatus(
        string name,
        SummonRank rank,
        double maxHp,
        double normalAttackPower,
        double heavyAttackPower)
    {
        EntityEnsure();
        entity.SetBattleStats(name, rank, maxHp, normalAttackPower, heavyAttackPower);
    }

    protected void SetAttackStrategies(AttackData normalAttack, params AttackData[] specialAttacks)
    {
        EntityEnsure();
        entity.SetAttackStrategies(normalAttack, specialAttacks);
    }

    private static double multiple = 1;
    public static double GetStatMultiplier() => multiple;
    public static void StatMultiplierSet(double value) => multiple = value;

    public virtual void ApplyStageMultiplier(double multiplier)
    {
        EntityEnsure();
        entity.ScaleStats(multiplier);
        NotifyStateChanged();
    }


    public virtual void Die()
    {
        Debug.Log($"{entity.Name}의 체력이 모두 소진되어 사라집니다.");
        deathHandler?.Invoke(this);

        Destroy(gameObject);

    }


    public void AddShield(double shieldAmount)
    {
        entity.AddShield(shieldAmount);
        shieldImage.SetActive(true);
        Debug.Log("보호막 부여. 현재 보호막: " + entity.Shield);
        NotifyStateChanged();
    }
    public double GetShield() => entity.Shield;
    public double GetInitialShield() => entity.InitialShield;

    public string GetSummonName() => entity.Name;

    public double GetHeavyAttackPower() => entity.HeavyAttackPower;

    public AttackData[] GetSpecialAttackStrategy() => entity.SpecialAttacks;
    public AttackData GetAttackStrategy() => entity.NormalAttack;

    public bool GetInvincibilityOnce() => entity.HasOnceInvincibility;
    public void SetOnceInvincibility(bool isinvincibility) => entity.SetOnceInvincibility(isinvincibility);

    public double GetMaxHP() => entity.MaxHp;

    // 현재 HP 접근 메서드
    public void SetNowHP(double hp) => entity.SetCurrentHp(hp);
    public double GetNowHP() => entity.CurrentHp;

    // 공격력 접근 메서드
    public void SetAttackPower(double power) => entity.SetAttackPower(power);
    public double GetAttackPower() => entity.AttackPower;

    public SummonRank GetSummonRank() => entity.Rank;
    public SummonRank GetDrawRank()
    {
        EntityEnsure();
        return summonData == null ? entity.Rank : summonData.GetSummonRank();
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


    public bool IsSpecialAttackCool(AttackData specialAttack)
    {
        return entity.IsSpecialAttackCoolingDown(specialAttack);
    }

    protected bool AttackCanUse(AttackData attackStrategy)
    {
        return entity.CanUseAttack(attackStrategy);
    }

    private bool AttackCooldownCheck(AttackData attackStrategy)
    {
        return entity.IsSpecialAttackCoolingDown(attackStrategy);
    }

    public List<StatusType> GetAllStatusTypes()
    {
        EntityEnsure();

        return entity.GetStatusTypes();
    }

    public IReadOnlyList<StatusData> GetActiveStatuses()
    {
        EntityEnsure();

        return entity.GetActiveStatuses();
    }

    public bool IsCursed()
    {
        return Contains(StatusType.Curse);
    }

    public bool IsStun()
    {
        return Contains(StatusType.Stun);
    }

    private bool Contains(StatusType statusType)
    {
        EntityEnsure();

        return entity.ContainsStatus(statusType);
    }

    public bool IsCooltime()
    {
        if (entity.SpecialAttacks == null || entity.SpecialAttacks.Length == 0)
        {
        Debug.Log("사용 가능한 특수 공격이 없습니다.");
            return false;
        }

        foreach (var specialAttack in entity.SpecialAttacks)
        {
            if (AttackCooldownCheck(specialAttack))
            {
        Debug.Log($"{entity.Name}의 {specialAttack.GetType().Name} 특수 공격이 쿨타임 중입니다.");
                return true;
            }
        }
        return false;
    }

    public AttackData[] GetAvailableSpecialAttacks()
    {
        return entity.GetAvailableSpecialAttacks();
    }

    public bool TryGetFirstAvailableSpecialAttack(out AttackData specialAttack, out int specialAttackIndex)
    {
        specialAttack = null;
        specialAttackIndex = -1;

        AttackData[] specialAttacks = GetSpecialAttackStrategy();
        if (specialAttacks == null)
        {
            return false;
        }

        for (int i = 0; i < specialAttacks.Length; i++)
        {
            AttackData currentSpecialAttack = specialAttacks[i];
            if (currentSpecialAttack != null && currentSpecialAttack.GetCurrentCooldown() <= 0)
            {
                specialAttack = currentSpecialAttack;
                specialAttackIndex = i;
                return true;
            }
        }

        return false;
    }

    public SummonEntity GetEntity()
    {
        EntityEnsure();
        return entity;
    }


    public int GetSpecialAttackCount() => entity.SpecialAttacks == null ? 0 : entity.SpecialAttacks.Length;

    private SummonAttackData GetSpecialAttackData(int specialAttackIndex)
    {
        if (summonData == null)
        {
            return null;
        }

        return summonData.GetSpecialAttack(specialAttackIndex);
    }

    public void SetDeathHandler(Action<Summon> deathHandler) => this.deathHandler = deathHandler;

    public void ClearDeathHandler() => deathHandler = null;

    public void AddStateChangedHandler(Action handler) => stateChanged += handler;

    public void RemoveStateChangedHandler(Action handler) => stateChanged -= handler;

    private void NotifyStateChanged()
    {
        stateChanged?.Invoke();
    }

    public Summon Clone()
    {
        Summon clone = (Summon)this.MemberwiseClone();
        clone.entity = entity.CloneForPrediction();
        clone.deathHandler = null;
        return clone;
    }
}
