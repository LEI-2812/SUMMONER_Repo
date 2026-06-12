public class AttackCooldownState
{
    private readonly int cooldownDuration;
    private int currentCooldown;

    public AttackCooldownState(int cooldownDuration)
    {
        this.cooldownDuration = cooldownDuration;
        currentCooldown = 0;
    }

    public int GetCooltime()
    {
        return cooldownDuration;
    }

    public int GetCurrentCooldown()
    {
        return currentCooldown;
    }

    public void ApplyCooldown()
    {
        currentCooldown = cooldownDuration;
    }

    public void ReduceCooldown()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }
    }
}
