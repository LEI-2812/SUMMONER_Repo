// 역할: 공격 쿨타임의 원래 값과 현재 남은 턴을 관리한다.
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
