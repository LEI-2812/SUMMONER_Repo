// 역할: StatusType 값의 종류를 정의한다.
public enum StatusType
{
    None,
    Stun,
    Poison, //중독
    Heal, // 회복
    Curse,
    LifeDrain,
    Burn,
    Shield,
    Upgrade, //강화
    OnceInvincibility // 1회 무적
}

[System.Serializable]
// 역할: StatusData의 책임을 정의한다.
public class StatusData
{
    public StatusType statusType { get; private set; }
    public int effectTime { get; set; }
    public double damagePerTurn { get; set; }

    private IStatusTarget attacker; // 공격자

    private bool applyOnce = false;

    private double originAttack;
    public StatusData(StatusType type, int eTime, double damage=0, IStatusTarget attacker = null)
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

    public IStatusTarget GetAttacker()
    {
        return attacker;
    }
    public void SetAttacker(IStatusTarget attackerTarget)
    {
        attacker = attackerTarget;
    }

    public bool ShouldApplyOnce()
    {
        return damagePerTurn > 0 && !applyOnce;
    }

    public void SetApplyOnce()
    {
        applyOnce = true;
    }
}
