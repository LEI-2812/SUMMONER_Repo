// 역할: 턴 시작과 종료에 맞춰 소환수 상태를 갱신한다.
class UpdateTurnSummonStateUseCase
{
    private readonly ITurnSummonBoard turnSummonBoard;

    public UpdateTurnSummonStateUseCase(ITurnSummonBoard turnSummonBoard)
    {
        this.turnSummonBoard = turnSummonBoard;
    }

    public void ApplyEnemyTurnStartEffects()
    {
        foreach (Summon summon in turnSummonBoard.GetEnemySummons())
        {
            ApplyTurnStartEffects(summon);
        }
    }

    public void ApplyPlayerTurnStartEffects()
    {
        foreach (Summon summon in turnSummonBoard.GetPlayerSummons())
        {
            ApplyTurnStartEffects(summon);
        }
    }

    public void UpdatePlayerSpecialCooldowns()
    {
        foreach (Summon summon in turnSummonBoard.GetPlayerSummons())
        {
            summon.UpdateSpecialAttackCooldowns();
        }
    }

    public void UpdateEnemySpecialCooldowns()
    {
        foreach (Summon summon in turnSummonBoard.GetEnemySummons())
        {
            summon.UpdateSpecialAttackCooldowns();
        }
    }

    public void UpdatePlayerUpgradeStatus()
    {
        foreach (Summon summon in turnSummonBoard.GetPlayerSummons())
        {
            summon.UpdateStatus(StatusTiming.Upgrade);
        }
    }

    public void UpdateEnemyUpgradeStatus()
    {
        foreach (Summon summon in turnSummonBoard.GetEnemySummons())
        {
            summon.UpdateStatus(StatusTiming.Upgrade);
        }
    }

    public void ResetPlayerAttackReady()
    {
        foreach (Summon summon in turnSummonBoard.GetPlayerSummons())
        {
            if (!summon.IsStun())
            {
                summon.SetIsAttack(true);
            }
        }
    }

    private void ApplyTurnStartEffects(Summon summon)
    {
        ApplyTurnStartDamageEffects(summon);
        ApplyTurnStartControlEffects(summon);
        UpdateNormalAttackCooldown(summon);
    }

    private void ApplyTurnStartDamageEffects(Summon summon)
    {
        summon.UpdateStatus(StatusTiming.Damage);
    }

    private void ApplyTurnStartControlEffects(Summon summon)
    {
        summon.UpdateStatus(StatusTiming.StunAndCurse);
    }

    private void UpdateNormalAttackCooldown(Summon summon)
    {
        AttackData normalAttack = summon.GetAttackStrategy();

        if (normalAttack == null)
        {
            return;
        }

        normalAttack.ReduceCooldown();
    }
}
