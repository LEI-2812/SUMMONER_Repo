public enum BattleResultPhase
{
    Playing,
    ClearStarted,
    FailStarted
}

public sealed class BattleResultStateMachine
{
    public BattleResultPhase CurrentState { get; private set; } = BattleResultPhase.Playing;

    public bool TryStartClear(bool isEnemyPlateClear, int clearTurn, int currentTurn)
    {
        if (!isEnemyPlateClear || clearTurn < currentTurn)
        {
            return false;
        }

        return TryStartClear();
    }

    public bool TryStartClear()
    {
        if (CurrentState != BattleResultPhase.Playing)
        {
            return false;
        }

        CurrentState = BattleResultPhase.ClearStarted;
        return true;
    }

    public bool TryStartFail(int clearTurn, int currentTurn)
    {
        if (clearTurn >= currentTurn)
        {
            return false;
        }

        return TryStartFail();
    }

    public bool TryStartFail()
    {
        if (CurrentState != BattleResultPhase.Playing)
        {
            return false;
        }

        CurrentState = BattleResultPhase.FailStarted;
        return true;
    }
}
