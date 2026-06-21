// 역할: 플레이어 턴의 일반 소환과 재소환 시작 조건을 확인하고 실행 흐름에 넘긴다.
public class PlayerSummonActions
{
    private readonly SummonController summonController;
    private readonly PlateController plateController;
    private readonly PlayerTurnResourceState resourceState;
    private readonly PlayerActionExecutor actionExecutor;
    private readonly PlayerView playerView;
    private readonly PlayerFeedbackView feedbackView;

    public PlayerSummonActions(
        SummonController summonController,
        PlateController plateController,
        PlayerTurnResourceState resourceState,
        PlayerActionExecutor actionExecutor,
        PlayerView playerView,
        PlayerFeedbackView feedbackView)
    {
        this.summonController = summonController;
        this.plateController = plateController;
        this.resourceState = resourceState;
        this.actionExecutor = actionExecutor;
        this.playerView = playerView;
        this.feedbackView = feedbackView;
    }

    public void TryStartSummon()
    {
        if (!CanSummonThisTurn())
        {
            return;
        }

        if (!CanUseSummonMana())
        {
            return;
        }

        if (!TryStartSummonOnEmptyPlate())
        {
            feedbackView.Log("모든 플레이트에 소환수가 있습니다.");
        }
    }

    public void TryStartRedraw()
    {
        if (!CanUseRedrawMana())
        {
            return;
        }

        if (!summonController.StartRedraw())
        {
            return;
        }

        actionExecutor.UseRedrawMana(resourceState, playerView, feedbackView);
    }

    private bool CanSummonThisTurn()
    {
        if (!resourceState.HasSummonedThisTurn)
        {
            return true;
        }

        feedbackView.PlayFail();
        feedbackView.Log("이 턴에서는 이미 소환을 했습니다. 다음 턴에 소환할 수 있습니다.");
        return false;
    }

    private bool CanUseSummonMana()
    {
        if (resourceState.Mana > 0)
        {
            return true;
        }

        feedbackView.PlayFail();
        feedbackView.Log("마나가 부족하여 소환 불가능");
        return false;
    }

    private bool TryStartSummonOnEmptyPlate()
    {
        int emptyPlateIndex = plateController.GetFirstEmptyPlayerPlateIndex();
        if (emptyPlateIndex < 0)
        {
            return false;
        }

        actionExecutor.StartSummon(
            summonController,
            resourceState,
            playerView,
            feedbackView,
            emptyPlateIndex);
        return true;
    }

    private bool CanUseRedrawMana()
    {
        if (resourceState.CanRedraw())
        {
            return true;
        }

        feedbackView.PlayFail();
        feedbackView.Log("재소환시 필요한 마나가 모자랍니다.");
        return false;
    }
}
