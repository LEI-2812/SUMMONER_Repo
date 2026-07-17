// 역할: 플레이어 행동 결과를 전투 화면과 결과 흐름에 반영한다.
public class PlayerCommandOutput : IPlayerCommandOutput
{
    private readonly BattleResultController battleResultController;
    private readonly PlateBoardView plateBoardView;
    private readonly PlayerHudView playerHudView;
    private readonly ManaView manaView;
    private readonly PlayerFeedbackView feedbackView;

    public PlayerCommandOutput(
        BattleResultController battleResultController,
        PlateBoardView plateBoardView,
        PlayerHudView playerHudView,
        ManaView manaView,
        PlayerFeedbackView feedbackView)
    {
        this.battleResultController = battleResultController;
        this.plateBoardView = plateBoardView;
        this.playerHudView = playerHudView;
        this.manaView = manaView;
        this.feedbackView = feedbackView;
    }

    public void ShowPlayerActionState(int mana, bool canSummon, bool canRedraw)
    {
        manaView.ShowMana(mana);
        playerHudView.ShowSummonAvailable(canSummon);
        playerHudView.ShowRedrawAvailable(canRedraw);
    }

    public void PlayClick() => feedbackView.PlayClick();
    public void PlayFail() => feedbackView.PlayFail();
    public void Log(string message) => feedbackView.Log(message);

    public void StartFailResult(int clearTurn, int currentTurn)
    {
        battleResultController.TryStartFailResult(clearTurn, currentTurn);
    }

    public bool TryStartClearResult(bool isEnemyBoardClear, int clearTurn, int currentTurn)
    {
        return battleResultController.TryStartClearResult(isEnemyBoardClear, clearTurn, currentTurn);
    }

    public bool IsEnemyBoardClear() => plateBoardView.IsEnemyPlateClear();
    public void CompactEnemyBoard() => plateBoardView.CompactEnemyPlates();
    public void HideSummonStatePanel() => playerHudView.HideStatePanel();
}
