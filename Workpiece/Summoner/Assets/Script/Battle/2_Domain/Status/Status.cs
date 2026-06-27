// 역할: Status의 책임을 정의한다.
public abstract class Status : StatusData
{
    protected Status(
        StatusType type,
        int remainingTurns,
        double value = 0,
        IStatusTarget source = null)
        : base(type, remainingTurns, value, source)
    {
    }

    public abstract StatusType GetStatusType();
    public abstract int GetRemainingTurn();
    public abstract bool SameStatusCanApply(StatusData existingStatus);
    public abstract bool ExistingApply(StatusData existingStatus, IStatusTarget target);
    public abstract bool StatusTurnCanUpdate(StatusTiming timing);
    public abstract string GetAlreadyAppliedMessage(string targetName);
    public abstract void StatusApply(IStatusTarget target);
    public abstract void StatusTurnUpdate(IStatusTarget target);
    public abstract void StatusExpire(IStatusTarget target);
}
