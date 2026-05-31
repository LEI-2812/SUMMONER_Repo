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


    public double getOriginAttack()
    {
        return originAttack;
    }
    public void setOriginAttack(double originAttack)
    {
        this.originAttack = originAttack;
    }

    public IStatusEffectTarget getAttacker()
    {
        return attacker;
    }
    public void setAttacker(IStatusEffectTarget attackerTarget)
    {
        attacker = attackerTarget;
    }

    public bool shouldApplyOnce()
    {
        return damagePerTurn > 0 && !applyOnce;
    }

    public void setApplyOnce()
    {
        applyOnce = true; // 한 번만 적용되도록 표시
    }
}
