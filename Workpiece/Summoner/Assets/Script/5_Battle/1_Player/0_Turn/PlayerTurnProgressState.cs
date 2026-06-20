// 역할: 플레이어 턴에서 전투 결과 확인에 필요한 턴 진행 상태를 저장한다.
public class PlayerTurnProgressState
{
    public int ClearTurn { get; private set; }
    public int CurrentTurn { get; private set; }
    public bool IsEnemyPlateClear { get; private set; }

    public void SetTurnProgress(int clearTurn, int currentTurn)
    {
        ClearTurn = clearTurn;
        CurrentTurn = currentTurn;
    }

    public void SetEnemyPlateClear(bool isEnemyPlateClear)
    {
        IsEnemyPlateClear = isEnemyPlateClear;
    }
}
