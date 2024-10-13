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
    protected double maxHP; //최대체력
    protected double nowHP; //현재 체력
    protected double attackPower; //일반공격
    protected double specialPower;  //특수공격
    protected SummonRank summonRank; //등급

    public bool CanAttack { get; set; } = true; // 상태이상중 공격가능 여부

    private List<StatusEffect> activeStatusEffects = new List<StatusEffect>(); //상태이상
    protected IAttackStrategy AttackStrategy { get; set; } // 일반 공격
    protected IAttackStrategy[] specialAttackStrategies;

    private Dictionary<string, int> skillCooldowns = new Dictionary<string, int>();

    private void Start()
    {
        image = GetComponent<Image>();
        nowHP = maxHP;
    }

    // 스킬 쿨타임 확인 메소드
    public virtual int SpecialAttackCooldown()
    {
        return 3; // 기본적으로 3턴 쿨타임
    }

    public void normalAttack(List<Plate> enemyPlates, int selectedPlateIndex,  int SpecialAttackArrayIndex)
    {
        AttackStrategy?.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);
    }

    public void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackArrayIndex)
    {
        if (IsSkillOnCooldown("SpecialAttack"))
        {
            Debug.Log("특수 스킬이 쿨타임 중입니다. 사용할 수 없습니다.");
            return;
        }

        getSpecialAttackStrategy()[SpecialAttackArrayIndex]?.Attack(this, enemyPlates, selectedPlateIndex, SpecialAttackArrayIndex);

        // 스킬 사용 후 쿨타임 적용
        ApplySkillCooldown("SpecialAttack", SpecialAttackCooldown());
    }

    // 상태이상 적용 메소드 (여러 상태이상 중복 허용)
    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        var existingEffect = activeStatusEffects.FirstOrDefault(e => e.statusType == statusEffect.statusType);

        if (existingEffect != null)
        {
            // 기존 상태이상이 있을 경우, 지속시간을 갱신 (즉시 효과는 제외)
            existingEffect.effectTime = statusEffect.effectTime - 1;  // 즉시 효과를 반영했으므로 지속시간을 1 감소
            existingEffect.damagePerTurn = statusEffect.damagePerTurn;
            Debug.Log($"{summonName}에게 중복된 {statusEffect.statusType} 상태이상이 갱신되었습니다.");
        }
        else
        {
            // 새로운 상태이상 추가
            activeStatusEffects.Add(statusEffect);
            statusEffect.ApplyStatus(this);  // 즉시 효과 적용
            statusEffect.effectTime--;       // 즉시 효과 반영 후 지속시간을 1 감소시킴
            Debug.Log($"{summonName}에게 {statusEffect.statusType} 상태이상이 적용되었습니다.");
        }
    }

    // 상태이상 및 쿨타임 업데이트 메소드
    public void UpdateStatusEffectsAndCooldowns()
    {
        List<StatusEffect> expiredEffects = new List<StatusEffect>();

        // 모든 상태이상의 지속시간을 감소시키고, 지속 데미지 처리
        foreach (var effect in activeStatusEffects)
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

        // 만료된 상태이상 제거
        foreach (var expired in expiredEffects)
        {
            activeStatusEffects.Remove(expired);
            Debug.Log($"{summonName}의 {expired.statusType} 상태이상이 종료되었습니다.");
        }

        // 스킬 쿨타임 처리 (기존 방식)
        foreach (var skill in skillCooldowns.Keys.ToList())
        {
            skillCooldowns[skill]--;
            if (skillCooldowns[skill] > 0)
            {
                Debug.Log($"{summonName}의 {skill} 스킬의 남은 쿨타임: {skillCooldowns[skill]} 턴");
            }

            if (skillCooldowns[skill] <= 0)
            {
                Debug.Log($"{summonName}의 {skill} 스킬 쿨타임이 종료되었습니다.");
            }
        }
    }

    // 스킬 쿨타임 적용 메소드
    public void ApplySkillCooldown(string skillName, int cooldown)
    {
        skillCooldowns[skillName] = cooldown;
        Debug.Log($"{summonName}의 {skillName} 스킬이 {cooldown}턴 동안 쿨타임에 들어갑니다.");
    }

    public void CheckCanAttack()
    {
        if (!CanAttack)
        {
            Debug.Log($"{summonName}은(는) 현재 공격할 수 없습니다.");
        }
    }

    public void ModifyAttackPower(double multiplier)
    {
        AttackPower *= (1 + multiplier);
        Debug.Log($"{summonName}의 공격력이 {multiplier * 100}% 변경되었습니다. 현재 공격력: {AttackPower}");

    }

    public void ApplyDamage(double damage)
    {
        nowHP -= damage;  // 잘못된 연산 수정
        Debug.Log($"{summonName}이(가) {damage} 피해를 입었습니다. 남은 체력: {NowHP}");

        if (NowHP <= 0)
        {
            die(); // 사망 처리
        }
    }


    // 체력 회복
    public void Heal(double healAmount)
    {
        nowHP += healAmount;
        Debug.Log($"{summonName}이(가) {healAmount}만큼 체력을 회복했습니다.");
    }


    public virtual void attack()
    {
        Debug.Log($"{summonName} attacks with {attackPower} power!");
    }

    public virtual void takeDamage(double damage) //데미지 입기
    {
        nowHP -= damage;

        if (nowHP <= 0)
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

    public bool IsSkillOnCooldown(string skillName)
    {
        if (skillCooldowns.ContainsKey(skillName) && skillCooldowns[skillName] > 0)
        {
            return true;
        }
        return false;
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
