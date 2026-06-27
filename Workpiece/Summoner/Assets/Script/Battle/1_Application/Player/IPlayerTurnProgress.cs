public interface IPlayerTurnProgress
{
    int GetTurnCount();
    int GetClearTurn();
    bool IsPlayerTurn();
    void EndPlayerTurn();
}
