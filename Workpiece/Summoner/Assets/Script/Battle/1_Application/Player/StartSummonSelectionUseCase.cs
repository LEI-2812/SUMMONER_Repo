// 역할: StartSummonSelectionUseCase의 책임을 정의한다.
using System;

public readonly struct SummonSelectionStartResult
{
    private SummonSelectionStartResult(bool didStart, int plateIndex, bool isRedraw)
    {
        DidStart = didStart;
        PlateIndex = plateIndex;
        IsRedraw = isRedraw;
    }

    public bool DidStart { get; }
    public int PlateIndex { get; }
    public bool IsRedraw { get; }

    public static SummonSelectionStartResult Failed()
    {
        return new SummonSelectionStartResult(false, -1, false);
    }

    public static SummonSelectionStartResult StartSummon(int plateIndex)
    {
        return new SummonSelectionStartResult(true, plateIndex, false);
    }

    public static SummonSelectionStartResult StartRedraw()
    {
        return new SummonSelectionStartResult(true, -1, true);
    }
}

public class StartSummonSelectionUseCase
{
    private readonly Func<int> findFirstEmptyPlayerPlateIndex;
    private readonly Func<bool> hasRedrawTarget;
    private readonly PlayerActionStateMachine playerActionStateMachine;

    public StartSummonSelectionUseCase(
        Func<int> findFirstEmptyPlayerPlateIndex,
        Func<bool> hasRedrawTarget,
        PlayerActionStateMachine playerActionStateMachine)
    {
        this.findFirstEmptyPlayerPlateIndex = findFirstEmptyPlayerPlateIndex;
        this.hasRedrawTarget = hasRedrawTarget;
        this.playerActionStateMachine = playerActionStateMachine;
    }

    public SummonSelectionStartResult PrepareSummon()
    {
        int emptyPlateIndex = findFirstEmptyPlayerPlateIndex();
        if (emptyPlateIndex < 0)
        {
            return SummonSelectionStartResult.Failed();
        }

        return SummonSelectionStartResult.StartSummon(emptyPlateIndex);
    }

    public SummonSelectionStartResult PrepareRedraw()
    {
        if (!hasRedrawTarget())
        {
            return SummonSelectionStartResult.Failed();
        }

        return SummonSelectionStartResult.StartRedraw();
    }

    public void ConfirmSelectionStarted(SummonSelectionStartResult startResult)
    {
        if (!startResult.DidStart)
        {
            return;
        }

        if (startResult.IsRedraw)
        {
            playerActionStateMachine.UseRedrawMana();
            return;
        }

        playerActionStateMachine.UseSummonMana();
    }

    public void CompleteSelection()
    {
        playerActionStateMachine.SetHasSummonedThisTurn(true);
        playerActionStateMachine.CompleteAction();
    }
}
