// 역할: 턴 시작/종료 시 플레이어와 적 소환수의 상태 갱신만 담당한다.
internal sealed class TurnSummonStateUpdater
{
    private readonly PlateController plateController;

    public TurnSummonStateUpdater(PlateController plateController)
    {
        this.plateController = plateController;
    }

    public void ApplyEnemyTurnStartEffects()
    {
        foreach (var summon in plateController.GetEnermySummons())
        {
            ApplyTurnStartEffects(summon);
        }
    }

    public void ApplyPlayerTurnStartEffects()
    {
        foreach (var summon in plateController.GetPlayerSummons())
        {
            ApplyTurnStartEffects(summon);
        }
    }

    public void UpdatePlayerSpecialCooldowns()
    {
        foreach (var summon in plateController.GetPlayerSummons())
        {
            summon.UpdateSpecialAttackCooldowns();
        }
    }

    public void UpdateEnemySpecialCooldowns()
    {
        foreach (var summon in plateController.GetEnermySummons())
        {
            summon.UpdateSpecialAttackCooldowns();
        }
    }

    public void UpdatePlayerUpgradeStatus()
    {
        foreach (var summon in plateController.GetPlayerSummons())
        {
            summon.UpdateUpgradeStatus();
        }
    }

    public void UpdateEnemyUpgradeStatus()
    {
        foreach (var summon in plateController.GetEnermySummons())
        {
            summon.UpdateUpgradeStatus();
        }
    }

    public void ResetPlayerAttackReady()
    {
        foreach (var summon in plateController.GetPlayerSummons())
        {
            if (!summon.IsStun())
            {
                summon.SetIsAttack(true);
            }
        }
    }

    private void ApplyTurnStartEffects(Summon summon)
    {
        summon.UpdateDamageStatusEffects();
        summon.UpdateStunAndCurseStatus();
        summon.GetAttackStrategy().ReduceCooldown();
    }
}
