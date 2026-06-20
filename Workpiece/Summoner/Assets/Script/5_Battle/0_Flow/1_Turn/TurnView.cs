using TMPro;

// 역할: 턴 수와 클리어 턴 UI 텍스트 표시만 담당한다.
internal sealed class TurnView
{
    private readonly TextMeshProUGUI turnCountText;
    private readonly TextMeshProUGUI turnClearText;

    public TurnView(TextMeshProUGUI turnCountText, TextMeshProUGUI turnClearText)
    {
        this.turnCountText = turnCountText;
        this.turnClearText = turnClearText;
    }

    public void ShowTurnCount(int turnCount)
    {
        turnCountText.text = $"Current Turn : {turnCount}";
    }

    public void ShowClearTurn(int clearTurn)
    {
        turnClearText.text = $"Clear Turn : {clearTurn}";
    }
}
