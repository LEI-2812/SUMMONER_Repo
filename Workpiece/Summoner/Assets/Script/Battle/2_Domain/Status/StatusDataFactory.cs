// 역할: StatusDataFactory의 책임을 정의한다.
public static class StatusDataFactory
{
    public static StatusData Create(StatusType statusType, int statusTime, double value, IStatusTarget attacker = null)
    {
        switch (statusType)
        {
            case StatusType.Poison:
                return new PoisonStatus(statusTime, value);

            case StatusType.Burn:
                return new BurnStatus(statusTime, value);

            case StatusType.Heal:
                return new HealStatus(value);

            case StatusType.Upgrade:
                return new UpgradeStatus(statusTime, value);

            case StatusType.Curse:
                return new CurseStatus(statusTime, value);

            case StatusType.Stun:
                return new StunStatus(statusTime);

            case StatusType.Shield:
                return new ShieldStatus(statusTime, value);

            case StatusType.LifeDrain:
                return new LifeDrainStatus(statusTime, value, attacker);

            case StatusType.OnceInvincibility:
                return new OnceInvincibilityStatus();

            default:
                return null;
        }
    }
}
