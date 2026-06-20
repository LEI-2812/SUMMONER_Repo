// 역할: 전투에서 사용할 상태 효과 종류를 나타낸다.
public enum StatusType
{
    None, //단순 공격
    Stun, //혼란
    Poison, //중독
    Heal, //힐
    Curse, //저주
    LifeDrain, //흡혈
    Burn, //화상
    Shield, //보호막
    Upgrade, //강화
    OnceInvincibility //1회 무적
}

[System.Serializable]
// 역할: 상태 효과의 종류, 지속 턴, 턴당 수치, 소유자를 담는 기본 데이터다.
public class StatusEffect
{
    public StatusType statusType { get; private set; }
    public int effectTime { get; set; } // 지속 시간
    public double damagePerTurn { get; set; } // 매 턴마다 줄 데미지

    private IStatusEffectTarget attacker; // 공격자

    private bool applyOnce = false;

    private double originAttack;
    public StatusEffect(StatusType type, int eTime, double damage=0, IStatusEffectTarget attacker = null)
    {
        statusType = type;
        effectTime = eTime;
        damagePerTurn = damage;
        this.attacker = attacker;
        applyOnce = false;
    }


    public double GetOriginAttack()
    {
        return originAttack;
    }
    public void SetOriginAttack(double originAttack)
    {
        this.originAttack = originAttack;
    }

    public IStatusEffectTarget GetAttacker()
    {
        return attacker;
    }
    public void SetAttacker(IStatusEffectTarget attackerTarget)
    {
        attacker = attackerTarget;
    }

    public bool ShouldApplyOnce()
    {
        return damagePerTurn > 0 && !applyOnce;
    }

    public void SetApplyOnce()
    {
        applyOnce = true; // 한 번만 적용되도록 표시
    }
}
