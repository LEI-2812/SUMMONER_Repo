// 역할: AttackCooldownStore의 책임을 정의한다.
public class AttackCooldownStore
{
    private readonly int cooldownDuration;
    private int currentCooldown;

    public AttackCooldownStore(int cooldownDuration)
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
