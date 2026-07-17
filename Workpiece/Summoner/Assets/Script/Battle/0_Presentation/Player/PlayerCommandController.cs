using UnityEngine;

// 역할: PlayerCommandController의 책임을 정의한다.
public class PlayerCommandController : MonoBehaviour, ICoroutineRunner
{
    private HandlePlayerCommandUseCase handlePlayerCommandUseCase;
    private SummonSelectionController summonSelectionController;

    internal void Initialize(
        HandlePlayerCommandUseCase useCase,
        SummonSelectionController selectionController)
    {
        handlePlayerCommandUseCase = useCase;
        summonSelectionController = selectionController;
    }

    public void OnClickSummon()
    {
        if (handlePlayerCommandUseCase == null || summonSelectionController == null)
        {
            return;
        }

        SummonSelectionStartResult startResult = handlePlayerCommandUseCase.ExecuteSummon();
        if (!startResult.DidStart)
        {
            return;
        }

        summonSelectionController.StartSummon(
            startResult.PlateIndex,
            false,
            handlePlayerCommandUseCase.CompleteSummonSelection);
        handlePlayerCommandUseCase.ConfirmSummonSelectionStarted(startResult);
    }

    public void OnClickEndTurn()
    {
        handlePlayerCommandUseCase?.ExecuteEndTurn();
    }

    public void OnClickRedraw()
    {
        if (handlePlayerCommandUseCase == null || summonSelectionController == null)
        {
            return;
        }

        SummonSelectionStartResult startResult = handlePlayerCommandUseCase.ExecuteRedraw();
        if (!startResult.DidStart)
        {
            return;
        }

        if (!summonSelectionController.StartRedraw(handlePlayerCommandUseCase.CompleteSummonSelection))
        {
            handlePlayerCommandUseCase.CancelSummonSelectionStart();
            return;
        }

        handlePlayerCommandUseCase.ConfirmSummonSelectionStarted(startResult);
    }

    public void OnClickNormalAttack()
    {
        handlePlayerCommandUseCase?.ExecuteNormalAttack();
    }

    public void OnClickSpecialAttack()
    {
        handlePlayerCommandUseCase?.ExecuteSpecialAttack();
    }

    public void OnClickSpecialAttack(int specialAttackIndex)
    {
        handlePlayerCommandUseCase?.ExecuteSpecialAttack(specialAttackIndex);
    }
}
