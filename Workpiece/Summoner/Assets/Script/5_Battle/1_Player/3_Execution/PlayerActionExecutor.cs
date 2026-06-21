// 역할: 플레이어 소환 시작과 재소환 비용 지불을 실행한다.
public class PlayerActionExecutor
{
    public void StartSummon(
        SummonController summonController,
        PlayerTurnResourceState resourceState,
        PlayerView playerView,
        PlayerFeedbackView feedbackView,
        int plateIndex)
    {
        feedbackView.Log(plateIndex + "번째 플레이트에 소환 예정");
        summonController.StartSummon(plateIndex, false);
        resourceState.UseSummonMana();
        playerView.UpdateMana(resourceState.Mana, resourceState.CanShowSummonAvailable());
        feedbackView.PlayClick();
        playerView.ShowSummonButtonDisabled();
    }

    public void UseRedrawMana(
        PlayerTurnResourceState resourceState,
        PlayerView playerView,
        PlayerFeedbackView feedbackView)
    {
        resourceState.UseRedrawMana();
        playerView.UpdateMana(resourceState.Mana, resourceState.CanShowSummonAvailable());
        feedbackView.PlayClick();
    }
}
