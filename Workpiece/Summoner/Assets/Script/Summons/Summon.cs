using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High, //아군 소환수
    Normal, Special, Boss //적 소환수
}

public class Summon : MonoBehaviour
{
    public Image image; //이미지
    protected string summonName; //이름
    protected double attackPower; //일반공격
    protected double specialPower;  //특수공격
    protected SummonRank summonRank; //등급
    protected double maxHP; //최대체력
    protected double nowHP; //현재 체력
    protected double shield = 0; //쉴드량
    protected bool invincibilityOnce = false;

    public bool CanAttack { get; set; } = true; // 상태이상중 공격가능 여부

    private List<StatusEffect> activeStatusEffects = new List<StatusEffect>(); //상태이상
    protected IAttackStrategy attackStrategy;
    protected IAttackStrategy[] specialAttackStrategies;

    

    private void Start()
    {
        image = GetComponent<Image>();
        nowHP = maxHP;
    }


    public void normalAttack(List<Plate> enemyPlates, int selectedPlateIndex,  int SpecialAttackArrayIndex)
    {
        if (attackStrategy == null || attackStrategy.getCurrentCooldown() > 0)
        {
            Debug.Log("일반 공격을 해당턴에 사용했습니다.");
            return;
        }
        attackStrategy.Attack(this, enemyPlates, selectedPlateIndex, 0); // 일반 공격 수행

        // 해당 공격에 쿨타임 적용
        attackStrategy.ApplyCooldown();
    }

    public virtual void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
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
        specialAttack.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);

        // 해당 공격에 쿨타임 적용
        specialAttack.ApplyCooldown();
    }

    // 상태이상 적용 메소드 (여러 상태이상 중복 허용)
    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        var existingEffect = activeStatusEffects.FirstOrDefault(e => e.statusType == statusEffect.statusType);

        // 상태이상 타입에 따라 로직을 다르게 처리
        switch (statusEffect.statusType)
        {
            case StatusType.Poison:
                if (existingEffect != null)
                {
                    // 중독 상태가 이미 있으면 지속시간과 피해량을 갱신
                    existingEffect.effectTime = statusEffect.effectTime;
                    existingEffect.damagePerTurn = statusEffect.damagePerTurn;
                    Debug.Log($"{summonName}에게 중복된 중독 상태이상이 갱신되었습니다.");
                }
                else
                {
                    // 새로운 중독 상태이상 추가
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    Debug.Log($"{summonName}에게 중독 상태이상이 적용되었습니다.");
                }
                break;

            case StatusType.Burn:
                if (existingEffect != null)
                {
                    // 화상 상태가 이미 있으면 지속시간과 피해량을 갱신
                    existingEffect.effectTime = statusEffect.effectTime;
                    existingEffect.damagePerTurn = statusEffect.damagePerTurn;
                    Debug.Log($"{summonName}에게 중복된 화상 상태이상이 갱신되었습니다.");
                }
                else
                {
                    // 새로운 화상 상태이상 추가
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    Debug.Log($"{summonName}에게 화상 상태이상이 적용되었습니다.");
                }
                break;

            case StatusType.Upgrade:
                if (existingEffect != null)
                {
                    // 강화 상태가 이미 있으면 지속시간을 갱신하고 효과를 추가 적용
                    existingEffect.effectTime = statusEffect.effectTime;
                    Debug.Log($"{summonName}의 공격력 강화 상태가 갱신되었습니다.");
                }
                else
                {
                    // 새로운 강화 상태이상 추가
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    Debug.Log($"{summonName}의 공격력이 강화되었습니다.");
                }
                break;

            case StatusType.Curse:
                if (existingEffect != null)
                {
                    // 저주 상태가 이미 있으면 지속시간을 갱신
                    existingEffect.effectTime = statusEffect.effectTime;
                    Debug.Log($"{summonName}에게 중복된 저주 상태이상이 갱신되었습니다.");
                }
                else
                {
                    // 새로운 저주 상태이상 추가
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    Debug.Log($"{summonName}에게 저주 상태이상이 적용되었습니다.");
                }
                break;

            case StatusType.Stun:
                if (existingEffect != null)
                {
                    // 스턴 상태가 이미 있으면 지속시간을 갱신
                    existingEffect.effectTime = statusEffect.effectTime;
                    Debug.Log($"{summonName}의 스턴 상태가 갱신되었습니다.");
                }
                else
                {
                    // 새로운 스턴 상태이상 추가
                    activeStatusEffects.Add(statusEffect);
                    statusEffect.ApplyStatus(this);  // 즉시 효과 적용
                    Debug.Log($"{summonName}이(가) 스턴 상태에 빠졌습니다.");
                }
                break;

            default:
                Debug.Log($"{summonName}에게 알 수 없는 상태이상이 적용되었습니다.");
                break;
        }
    }


    // 상태이상 및 쿨타임 업데이트 메소드
    public void UpdateStatusEffectsAndCooldowns()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        // 모든 상태이상의 지속시간을 감소시키고, 지속 데미지 처리
        foreach (var effect in activeStatusEffects)
        {
            if (effect != null)
            {
                effect.effectTime--; // 상태이상 지속시간 감소

                if (effect.damagePerTurn > 0)
                {
                    ApplyDamage(effect.damagePerTurn); // 데미지 적용
                    Debug.Log($"{summonName}이(가) {effect.statusType} 상태로 인해 {effect.damagePerTurn} 데미지를 입습니다.");
                }

                if (effect.effectTime <= 0)
                {
                    expiredEffects.Add(effect); // 지속 시간이 끝난 상태이상은 만료 처리
                }
            }
        }

        // 만료된 상태이상 제거
        foreach (var expired in expiredEffects)
        {
            activeStatusEffects.Remove(expired);
            Debug.Log($"{summonName}의 {expired.statusType} 상태이상이 종료되었습니다.");
        }

        // 특수 공격들의 쿨타임 처리
        if (specialAttackStrategies != null) // 배열이 null이 아닌지 확인
        {
            foreach (var specialAttack in specialAttackStrategies)
            {
                if (specialAttack != null) // 각 공격이 null인지 확인
                {
                    if (specialAttack.getCurrentCooldown() > 0){
                        specialAttack.ReduceCooldown(); // 쿨타임 감소
                        Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 스킬의 남은 쿨타임: {specialAttack.getCurrentCooldown()} 턴");
                    }
                    else
                        Debug.Log($"{summonName}의 {specialAttack.GetType().Name} 스킬 쿨타임이 종료되었습니다.");
                }
            }
        }
    }

    public void CheckCanAttack()
    {
        if (!CanAttack)
        {
            Debug.Log($"{summonName}은(는) 현재 공격할 수 없습니다.");
        }
    }

    public void UpgradeAttackPower(double multiplier)
    {
        attackPower *= (1 + multiplier);
        Debug.Log($"{summonName}의 공격력이 {multiplier * 100}% 변경되었습니다. 현재 공격력: {attackPower}");

    }

    public void ApplyDamage(double damage)
    {
        nowHP -= damage; 
        Debug.Log($"{summonName}이(가) {damage} 피해를 입었습니다. 남은 체력: {nowHP}");

        if (nowHP <= 0)
        {
            die(); // 사망 처리
        }
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
    }


    public virtual void takeDamage(double damage) //데미지 입기
    {
        if (invincibilityOnce)
        {
            invincibilityOnce = false;
            Debug.Log("1회 무적보호막으로 공격을 보호했습니다.");
            return;
        }
        if (shield > 0) //쉴드가 있을때 데미지 받게
        {
            if (shield >= damage)
            {
                // 쉴드가 데미지를 모두 막아줌
                shield -= damage;
                Debug.Log("쉴드로 피해 방어. 남은 쉴드: " + shield);
            }
            else
            {
                // 쉴드가 일부만 막고 나머지는 체력에 적용
                double remainingDamage = damage - shield;
                shield = 0;
                nowHP -= remainingDamage;
                Debug.Log("쉴드가 파괴됨. 남은 체력: " + nowHP);
            }
        }
        else //쉴드가 없을경우
        {
            nowHP -= damage;
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
    }

    // 소환수 초기화 메서드
    public virtual void summonInitialize(){ }


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

    public void AddShield(int shieldAmount)
    {
        shield += shieldAmount;

        Debug.Log("쉴드 부여. 현재 쉴드: " + shield);
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

    public IAttackStrategy getAttackStrategy()
    {
        return attackStrategy;
    }

    public bool getInvincibilityOnce()
    {
        return invincibilityOnce;
    }
    public void setInvincibilityOnce(bool invincibilityOnce)
    {
        this.invincibilityOnce = invincibilityOnce;
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

    // specialPower 관련 메서드
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
}
