using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High, //아군 소환수
    Normal, Special, Boss //적 소환수
}

public enum SummonType
{
    Cat, Rabbit, Wolf, Eagle, Snake, Fox
}

public class Summon : MonoBehaviour, UpdateStateObserver
{
    [SerializeField] protected Image image; //이미지
    [SerializeField] protected Sprite[] sprites; // 스프라이트 모음
    [SerializeField] protected GameObject shieldImage;
    [SerializeField] protected Animator animator;

    [Header("효과음")]
    [SerializeField] public AudioSource attackSound;
    [SerializeField] private AudioSource downHitSound;
    [SerializeField] private AudioSource upAttackSound;

    protected string summonName; //이름
    public Sprite normalAttackSprite; // 일반 공격 스프라이트
    public Sprite specialAttackSprite; // 특수 공격 스프라이트
    public double attackPower; //일반공격
    public double heavyAttakPower; //강 공격력
    protected SummonRank summonRank; //등급
    protected SummonType summonType;
    protected double maxHP; //최대체력
    public double nowHP; //현재 체력
    protected double shield = 0; //쉴드량
    private double initialShield; // 초기 쉴드 양
    protected bool onceInvincibility = false;
    public bool isAttack = true; // 상태이상중 공격가능 여부

    private bool attakingMotion = false;

    [Header("상태이상")]
    [SerializeField] private List<StatusEffect> activeStatusEffects = new List<StatusEffect>(); //상태이상
    protected IAttackStrategy attackStrategy;
    protected IAttackStrategy[] specialAttackStrategies;

    private List<stateObserver> observers = new List<stateObserver>();


    private void Awake()
    {
        image = GetComponent<Image>();
        nowHP = maxHP;
    }

    void Update()
    {
        if(!attakingMotion) ApplyStatusEffectBlink();
    }

    public void SetSprite(int index)
    {
        image.sprite = sprites[index];
    }


    public void normalAttack(List<Plate> targetPlates, int selectedPlateIndex)
    {
        if (attackStrategy == null || attackStrategy.getCurrentCooldown() > 0)
        {
            Debug.Log("일반 공격을 해당턴에 사용했습니다.");
            return;
        }
        attackStrategy.Attack(this, targetPlates, selectedPlateIndex, 0); // 일반 공격 수행
        animator.SetTrigger("attack");
        attackSound.Play();
        
        StartCoroutine(ColorChange(1)); // 검정색
        
        // 해당 공격에 쿨타임 적용
        attackStrategy.ApplyCooldown();
        isAttack = false;
    }

    protected IEnumerator ColorChange(int color)    // 색이 변했다가 돌아옴
    {
        //
        attakingMotion = true;
        switch (color)
        {
            case 1: // 검정색
                image.color = new Color(0f, 0f, 0f); // #000000
                break;
            case 2: // 빨강색
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
        if (SpecialAttackArrayIndex < 0 || SpecialAttackArrayIndex >= specialAttackStrategies.Length)
        {
            Debug.Log("유효하지 않은 특수 공격 인덱스입니다.");
            return;
        }

        var specialAttack = specialAttackStrategies[SpecialAttackArrayIndex];
        
        if (specialAttack == null || specialAttack.getCurrentCooldown() > 0)
        {
            Debug.Log("특수 스킬이 쿨타임 중입니다.");
            return;
        }

        // 공격 수행
        specialAttack.Attack(this, targetPlates, selectedPlateIndex, SpecialAttackArrayIndex);
        animator.SetTrigger("attack");
        StartCoroutine(ColorChange(1)); // 검정색
        // 해당 공격에 쿨타임 적용
        specialAttack.ApplyCooldown();
        isAttack = false;
    }

    // 상태이상 적용 메소드 (여러 상태이상 중복 허용) //덮어씌어지는 로직
    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        var existingEffect = activeStatusEffects.FirstOrDefault(e => e.statusType == statusEffect.statusType); //이미 같은 상태이상이 있는지 가져옴

        // 상태이상 타입에 따라 로직을 다르게 처리
        switch (statusEffect.statusType)
        {
            case StatusType.Poison:
                if (existingEffect != null)
                {
                    // 중독 상태가 이미 있으면 지속시간과 피해량을 갱신
                    Debug.Log($"{summonName}은 이미 중독 상태입니다.");
                }
                else
                {
                    // 새로운 중독 상태이상 추가
                    StartCoroutine(ColorChange(3)); // 보라색
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용 //데미지를 받음
                    NotifyObservers(); // 상태 적용 후 알림
                    downHitSound.Play();
                    Debug.Log($"{summonName}에게 중독 상태이상이 적용되었습니다.");
                }
                break;

            case StatusType.Burn:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}은 이미 화상 상태입니다.");
                }
                else
                {
                    // 새로운 화상 상태이상 추가
                    StartCoroutine(ColorChange(3)); // 보라색
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    NotifyObservers(); // 상태 적용 후 알림
                    downHitSound.Play();
                    Debug.Log($"{summonName}에게 화상 상태이상이 적용되었습니다.");
                }
                break;

            case StatusType.Upgrade:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}은 이미 강화 상태입니다.");
                }
                else
                {
                    // 기존 공격력을 저장합니다.
                    statusEffect.setOriginAttack(this.attackPower);
                    // 새로운 강화 상태이상 추가
                    activeStatusEffects.Add(statusEffect);
                    if (statusEffect.shouldApplyOnce())
                    {
                        statusEffect.ApplyStatus(this); // 한 번만 적용
                        statusEffect.setApplyOnce(); // 적용된 상태 표시
                        NotifyObservers(); // 상태 적용 후 알림
                        upAttackSound.Play();
                    }

       
                    Debug.Log($"{summonName}의 공격력이 강화되었습니다.");
                }
                break;

            case StatusType.Curse:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}은 이미 저주 상태입니다.");
                }
                else
                {
                    // 새로운 저주 상태이상 추가
                    StartCoroutine(ColorChange(3)); // 보라색
                    activeStatusEffects.Add(statusEffect);
                    if (statusEffect.shouldApplyOnce())
                    {
                        statusEffect.ApplyStatus(this); // 한 번만 적용
                        statusEffect.setApplyOnce(); // 적용된 상태 표시
                        NotifyObservers(); // 상태 적용 후 알림
                    }
                    downHitSound.Play();
                    Debug.Log($"{summonName}에게 저주 상태이상이 적용되었습니다.");
                }
                break;

            case StatusType.Stun:
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}은 이미 스턴 상태입니다.");
                }
                else
                {
                    // 새로운 스턴 상태이상 추가
                    StartCoroutine(ColorChange(3)); // 보라색
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    NotifyObservers(); // 상태 적용 후 알림
                    downHitSound.Play();
                    Debug.Log($"{summonName}이(가) 스턴 상태에 빠졌습니다.");
                }
                break;

            case StatusType.Shield: //쉴드 덮어씌우기
                if (existingEffect != null)
                {
                    shield = existingEffect.damagePerTurn; //보호막을 스킬 수치만큼 다시 채우기
                    upAttackSound.Play();
                    Debug.Log($"{summonName}의 보호막을 덮씌웁니다.");
                }
                else
                {
                    // 새로운 쉴드 추가
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    NotifyObservers(); // 상태 적용 후 알림
                    upAttackSound.Play();
                    Debug.Log($"{summonName}에게 보호막이 생겼습니다.");
                }
                break;
            case StatusType.LifeDrain: //흡혈
                if (existingEffect != null)
                {
                    Debug.Log($"{summonName}은 이미 흡혈 당하고있습니다.");
                }
                else
                {
                    StartCoroutine(ColorChange(3)); // 보라색
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    NotifyObservers(); // 상태 적용 후 알림
                    downHitSound.Play();
                    Debug.Log($"{summonName}이 흡혈 당합니다.");
                }
                break;

            default:
                Debug.Log($"{summonName}에게 알 수 없는 상태이상이 적용되었습니다.");
                break;
        }
    }

    // 데미지를 주는 상태이상 업데이트 메소드 (예: Poison, Burn 등)
    //public void UpdateDamageStatusEffects()
    //{
    //    List<StatusEffect> expiredEffects = new List<StatusEffect>();

    //    foreach (var effect in activeStatusEffects)
    //    {
    //        if (effect != null && effect.effectTime > 0)
    //        {
    //            // 데미지 주는 상태 확인 및 처리
    //            if (effect.damagePerTurn > 0 && effect.statusType != StatusType.Upgrade && effect.statusType != StatusType.Curse)
    //            {
    //                Debug.Log($"{summonName}이(가) {effect.statusType} 상태로 인해 {effect.damagePerTurn} 데미지를 입습니다. 남은 상태이상시간: {effect.effectTime} 턴");
    //                takeDamage(effect.damagePerTurn); // 피해 적용
    //            }

    //            // 흡혈 상태일 경우 회복 처리
    //            if (effect.statusType == StatusType.LifeDrain && effect.getAttacker() != null)
    //            {
    //                double healAmount = effect.damagePerTurn; // 흡혈 데미지만큼 회복
    //                Debug.Log($"{effect.getAttacker()}이(가) {effect.statusType} 상태로 인해 {healAmount}만큼 회복합니다.");
    //                effect.getAttacker().Heal(effect.damagePerTurn);
    //            }

    //            // 지속시간 감소
    //            effect.effectTime--;

    //            // 상태가 만료될 경우 만료 리스트에 추가
    //            if (effect.effectTime <= 0)
    //            {
    //                expiredEffects.Add(effect);
    //            }
    //        }
    //    }

    //    RemoveExpiredEffects(expiredEffects);
    //}
    public void UpdateDamageStatusEffects()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        // 역순으로 리스트 순회
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeStatusEffects[i];

            if (effect != null && effect.effectTime > 0)
            {
                // 데미지 주는 상태 확인 및 처리
                if (effect.damagePerTurn > 0 && effect.statusType != StatusType.Upgrade && effect.statusType != StatusType.Curse)
                {
                    Debug.Log($"{summonName}이(가) {effect.statusType} 상태로 인해 {effect.damagePerTurn} 데미지를 입습니다. 남은 상태이상시간: {effect.effectTime} 턴");
                    takeDamage(effect.damagePerTurn); // 피해 적용
                }

                // 흡혈 상태일 경우 회복 처리
                if (effect.statusType == StatusType.LifeDrain && effect.getAttacker() != null)
                {
                    double healAmount = effect.damagePerTurn; // 흡혈 데미지만큼 회복
                    Debug.Log($"{effect.getAttacker()}이(가) {effect.statusType} 상태로 인해 {healAmount}만큼 회복합니다.");
                    effect.getAttacker().Heal(effect.damagePerTurn);
                }

                // 지속시간 감소
                effect.effectTime--;

                // 상태가 만료될 경우 만료 리스트에 추가
                if (effect.effectTime <= 0)
                {
                    expiredEffects.Add(effect);
                }
            }
        }

        RemoveExpiredEffects(expiredEffects); // 만료된 효과 제거
    }

    // 스턴 상태 업데이트 메소드
    public void UpdateStunAndCurseStatus()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        foreach (var effect in activeStatusEffects)
        {
            if (effect != null && (effect.statusType == StatusType.Stun || effect.statusType == StatusType.Curse))
            {
                // 스턴 상태는 공격 불가능하게 설정
                if (effect.statusType == StatusType.Stun)
                {
                    setIsAttack(false);
                    Debug.Log($"{summonName}은 스턴과 저주 상태로 공격할 수 없습니다.");
                }

                // 지속시간 감소
                effect.effectTime--;

                // 상태가 만료될 경우 만료 리스트에 추가
                if (effect.effectTime <= 0)
                {
                    expiredEffects.Add(effect);
                }
            }
        }

        RemoveExpiredEffects(expiredEffects);
    }

    // 강화 상태 업데이트 메소드
    public void UpdateUpgradeStatus()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        foreach (var effect in activeStatusEffects)
        {
            if (effect != null && effect.statusType == StatusType.Upgrade)
            {
                // 강화는 지속시간만 관리하며, 데미지를 주지 않음
                effect.effectTime--;

                // 상태가 만료될 경우 만료 리스트에 추가
                if (effect.effectTime <= 0)
                {
                    Debug.Log("공격력복원");
                    attackPower = effect.getOriginAttack(); // 원래 공격력으로 복원
                    expiredEffects.Add(effect);
                }
            }
        }

        RemoveExpiredEffects(expiredEffects);
    }

    // 만료된 상태이상 제거 메소드
    private void RemoveExpiredEffects(List<StatusEffect> expiredEffects)
    {
        foreach (var expired in expiredEffects)
        {
            activeStatusEffects.Remove(expired);
            Debug.Log($"{summonName}의 {expired.statusType} 상태이상이 종료되었습니다.");

            // 스턴 해제 시 공격 가능하도록 설정
            if (expired.statusType == StatusType.Stun)
            {
                setIsAttack(true);
                Debug.Log($"{summonName}의 스턴이 해제되었습니다. 공격 가능.");
            }
            NotifyObservers(); // 상태 적용 후 알림
        }
    }



    // 특수 공격 쿨타임 업데이트 메소드
    public void UpdateSpecialAttackCooldowns()
    {
        if (specialAttackStrategies == null) return; // 배열이 null인 경우 체크

        foreach (var specialAttack in specialAttackStrategies)
        {
            if (specialAttack != null)
            {
                if (specialAttack.getCurrentCooldown() > 0)
                {
                    specialAttack.ReduceCooldown(); // 쿨타임 감소
                    Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 스킬의 남은 쿨타임: {specialAttack.getCurrentCooldown()} 턴");
                }
                else
                {
                    Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 스킬 쿨타임이 종료되었습니다.");
                }
            }
        }
    }

    private void SetColorByStatus(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.Burn:
                image.color = new Color(1f, 0.3f, 0.3f); // 붉은색 (Burn)
                break;
            case StatusType.Poison:
                image.color = new Color(0.3f, 1f, 0.3f); // 녹색 (Poison)
                break;
            case StatusType.Stun:
                image.color = new Color(0.2f, 0.2f, 0.2f); // 검은색 (Stun)
                break;
            case StatusType.LifeDrain:
                image.color = new Color(1f, 1f, 0.5f); // 노란색 (흡혈)
                break;
            default:
                image.color = Color.white; // 기본 색상
                break;
        }
    }


    private int currentEffectIndex = 0;
    private float blinkTimer = 0f;
    private float blinkInterval = 1f; // 색상 변경 간격
    private void ApplyStatusEffectBlink()
    {
        if (activeStatusEffects.Count == 0)
        {
            image.color = Color.white; // 상태이상이 없으면 기본 색으로 설정
            return;
        }

        if (activeStatusEffects.Count == 1)
        {
            // 상태이상이 1개일 때는 해당 색상을 유지
            SetColorByStatus(activeStatusEffects[0].statusType);
        }
        else
        {
            // 상태이상이 여러 개일 때는 일정 간격으로 색상 변경
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;
                currentEffectIndex = (currentEffectIndex + 1) % activeStatusEffects.Count;
                SetColorByStatus(activeStatusEffects[currentEffectIndex].statusType);
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
        attackPower = Math.Floor(attackPower); // 소수점 아래를 버림

        attackPower = (int)attackPower; // double을 int로 변환
        Debug.Log($"{summonName}의 공격력이 {multiplier * 100}% 강화 되었습니다. 현재 공격력: {attackPower}");
    }

    public void Cursed(double curse)
    {
        attackPower *= (1 - curse);
        Debug.Log($"{summonName}의 공격력이 {curse * 100}% 다운 되었습니다. 현재 공격력: {attackPower}");
    }


    // 체력 회복
    public void Heal(double healAmount)
    {
        nowHP += healAmount;
        if(nowHP >= maxHP)
        {
            nowHP = maxHP;
        }
        Debug.Log($"{summonName}이(가) {healAmount}만큼 체력을 회복했습니다.");
        // 체력 변경 시 옵저버들에게 알림
        NotifyObservers();
        animator.SetTrigger("hitted");
        StartCoroutine(ColorChange(4)); // 초록색
        upAttackSound.Play();
    }


    public virtual void takeDamage(double damage) //데미지 입기
    {
        damage = (int)damage; //데미지의 소숫점 제거
        damage = Math.Floor(damage); // 소수점 아래를 버림

        if (onceInvincibility)
        {
            onceInvincibility = false;
            Debug.Log("1회 무적보호막으로 공격을 보호했습니다.");
            return;
        }
        if (shield > 0) //쉴드가 있을때 데미지 받게
        {
            if (shield >= damage)
            {
                // 쉴드가 데미지를 모두 막아줌
                shield -= damage;
                animator.SetTrigger("hitted");
                Debug.Log("쉴드로 피해 방어. 남은 쉴드: " + shield);
            }
            else
            {
                // 쉴드가 일부만 막고 나머지는 체력에 적용
                double remainingDamage = damage - shield;
                shield = 0;
                nowHP -= remainingDamage;
                animator.SetTrigger("hitted");
                shieldImage.SetActive(false);

                // 쉴드가 파괴될 때 Shield 상태 제거
                var shieldEffect = activeStatusEffects.FirstOrDefault(e => e.statusType == StatusType.Shield);
                RemoveExpiredEffects(new List<StatusEffect> { shieldEffect });
                Debug.Log("쉴드가 파괴됨. 남은 체력: " + nowHP);
            }
        }
        else //쉴드가 없을경우
        {
            nowHP -= damage;
            animator.SetTrigger("hitted");
            StartCoroutine((ColorChange(2)));   // 빨간 색
        }

        if (nowHP <= 0) //죽음처리
        {
            nowHP = 0;  // 체력을 0 이하로 내리지 않음
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
            die();  // 사망 처리
        }
        else
        {
            Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
        }

        // 체력 변경 시 옵저버들에게 알림
        NotifyObservers();
    }

    // 소환수 초기화 메서드
    public virtual void summonInitialize()
    {
        NotifyObservers();
    }

    public static double multiple=5; //배수설정
    public virtual void ApplayMultiple(double m) {
        maxHP = (int)(maxHP * m);
        nowHP = maxHP;
        attackPower = (int)(attackPower * m); // 일반공격 배수 적용
        heavyAttakPower = (int)(heavyAttakPower * m); // 강공격 배수 적용
    }


    public virtual void die()
    {
        Debug.Log($"{summonName} 가 체력이 소모되어 사라집니다.");
        // Plate에서 소환수를 제거하기 위해 소환수를 배치한 Plate를 가져옴
        Plate plate = GetComponentInParent<Plate>(); // 소환수가 속한 부모 Plate 가져오기
        if (plate != null)
        {
            plate.RemoveSummon(); // 소환수 제거
        }

        // 소환수 오브젝트 삭제
        Destroy(gameObject); // 소환수 오브젝트를 씬에서 제거

    }


    public void AddShield(double shieldAmount)
    {
        if (shield == 0) // 현재 쉴드가 0일 때만 초기화
        {
            initialShield = shieldAmount;
        }
        shield += shieldAmount;
        shieldImage.SetActive(true);
        Debug.Log("쉴드 부여. 현재 쉴드: " + shield);
        NotifyObservers();
    }
    public double getShield()
    {
        return shield; // 현재 쉴드 값을 반환
    }
    public double GetInitialShield()
    {
        return initialShield; // 초기 쉴드 양을 반환
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
    // nowHP 관련 메서드
    public void setNowHP(double hp)
    {
        this.nowHP = hp;
    }

    public double getNowHP()
    {
        return nowHP;
    }

    // attackPower 관련 메서드
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


    //특수공격이 쿨타임인지
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

        // activeStatusEffects 리스트의 각 StatusEffect에서 statusType을 가져와 추가
        foreach (StatusEffect effect in activeStatusEffects)
        {
            statusTypes.Add(effect.statusType);
        }

        return statusTypes;
    }

    public bool IsCursed()
    {
        // activeStatusEffects 리스트를 하나씩 순회
        foreach (StatusEffect effect in activeStatusEffects)
        {
            // 각 상태이상의 상태 타입이 StatusType.Curse인지 확인
            if (effect.statusType == StatusType.Curse)
            {
                // Curse 상태가 발견되면 true 반환
                return true;
            }
        }

        // Curse 상태가 없으면 false 반환
        return false;
    }

    public bool IsStun()
    {
        // activeStatusEffects 리스트를 하나씩 순회
        foreach (StatusEffect effect in activeStatusEffects)
        {
            // 각 상태이상의 상태 타입이 StatusType.Stun 확인
            if (effect.statusType == StatusType.Stun)
            {
                // Stun 상태가 발견되면 true 반환
                return true;
            }
        }

        // Stun 상태가 없으면 false 반환
        return false;
    }

    public bool IsCooltime() //쿨타임인지 확인
    {
        // 먼저 특수 공격 전략 배열이 있는지 확인
        if (specialAttackStrategies == null || specialAttackStrategies.Length == 0)
        {
            Debug.Log("특수 공격이 없습니다.");
            return false;
        }

        // 각 특수 공격 전략에 대해 쿨타임 여부 확인
        foreach (var specialAttack in specialAttackStrategies)
        {
            if (specialAttack != null && specialAttack.getCurrentCooldown() > 0)
            {
                // 하나라도 쿨타임 중인 전략이 있으면 true 반환
                Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 스킬이 쿨타임 중입니다.");
                return true;
            }
        }
        return false;
    }

    public IAttackStrategy[] getAvailableSpecialAttacks()
    {
        List<IAttackStrategy> availableSpecialAttacks = new List<IAttackStrategy>();

        // 특수 공격 중 쿨타임이 없는 공격을 필터링하여 추가
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

    public Summon Clone() //얉은 복사
    {
        return (Summon)this.MemberwiseClone();
    }
}