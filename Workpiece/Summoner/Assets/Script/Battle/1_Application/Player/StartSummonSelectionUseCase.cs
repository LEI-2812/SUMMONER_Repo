// 역할: StartSummonSelectionUseCase의 책임을 정의한다.
using System;

public class StartSummonSelectionUseCase
{
    private readonly SummonSelectionController summonSelectionController;
    private readonly Func<int> findFirstEmptyPlayerPlateIndex;
    private readonly PlayerActionStateMachine playerActionStateMachine;
    private readonly PlayerFeedbackView feedbackView;

    public StartSummonSelectionUseCase(
        SummonSelectionController summonSelectionController,
        Func<int> findFirstEmptyPlayerPlateIndex,
        PlayerActionStateMachine playerActionStateMachine,
        PlayerFeedbackView feedbackView)
    {
        this.summonSelectionController = summonSelectionController;
        this.findFirstEmptyPlayerPlateIndex = findFirstEmptyPlayerPlateIndex;
        this.playerActionStateMachine = playerActionStateMachine;
        this.feedbackView = feedbackView;
    }

    public PlayerActionResult ExecuteSummon()
    {
        if (!TryStartSummonOnEmptyPlate())
        {
            feedbackView.Log("모든 플레이트에 소환수가 있습니다.");
            return PlayerActionResult.Failed;
        }

        return PlayerActionResult.WaitingForTarget;
    }

    private bool TryStartSummonOnEmptyPlate()
    {
        int emptyPlateIndex = findFirstEmptyPlayerPlateIndex();
        if (emptyPlateIndex < 0)
        {
            return false;
        }

        StartSummonOnPlate(emptyPlateIndex);
        return true;
    }

    private void StartSummonOnPlate(int plateIndex)
    {
        feedbackView.Log(plateIndex + "번째 플레이트에서 소환을 시작합니다.");
        summonSelectionController.StartSummon(plateIndex, false, CompleteSummonSelection);
        playerActionStateMachine.UseSummonMana();
        feedbackView.PlayClick();
    }

    public PlayerActionResult ExecuteRedraw()
    {
        if (!summonSelectionController.StartRedraw(CompleteSummonSelection))
        {
            return PlayerActionResult.Failed;
        }

        UseRedrawMana();
        return PlayerActionResult.WaitingForTarget;
    }

    private void UseRedrawMana()
    {
        playerActionStateMachine.UseRedrawMana();
        feedbackView.PlayClick();
    }

    private void CompleteSummonSelection()
    {
        playerActionStateMachine.SetHasSummonedThisTurn(true);
        playerActionStateMachine.CompleteAction();
    }
}
