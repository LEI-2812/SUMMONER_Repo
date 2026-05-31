public static class StatusEffectInstanceCreate
{
    public static StatusEffect Create(StatusType statusType, int statusTime, double value, IStatusEffectTarget attacker = null)
    {
        switch (statusType)
        {
            case StatusType.Poison:
                return new PoisonStatusEffect(statusTime, value);

            case StatusType.Burn:
                return new BurnStatusEffect(statusTime, value);

            case StatusType.Heal:
                return new HealStatusEffect(value);

            case StatusType.Upgrade:
                return new UpgradeStatusEffect(statusTime, value);

            case StatusType.Curse:
                return new CurseStatusEffect(statusTime, value);

            case StatusType.Stun:
                return new StunStatusEffect(statusTime);

            case StatusType.Shield:
                return new ShieldStatusEffect(statusTime, value);

            case StatusType.LifeDrain:
                return new LifeDrainStatusEffect(statusTime, value, attacker);

            case StatusType.OnceInvincibility:
                return new OnceInvincibilityStatusEffect();

            default:
                return null;
        }
    }
}
