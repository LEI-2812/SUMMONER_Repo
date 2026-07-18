using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: PlayerCommandController의 책임을 정의한다.
public class PlayerCommandController : MonoBehaviour
{
    private HandlePlayerCommandUseCase handlePlayerCommandUseCase;
    private SummonSelectionController summonSelectionController;
    private IPlayerTurnProgress turnProgress;
    private BattleResultController battleResultController;
    private PlateBoardView plateBoardView;
    private PlayerHudView playerHudView;
    private ManaView manaView;
    private PlayerFeedbackView feedbackView;
    private IReadOnlyList<BattleBoardInputController> playerPlates;
    private IReadOnlyList<BattleBoardInputController> enemyPlates;

    public void ConnectPlayerCommand(
        HandlePlayerCommandUseCase useCase,
        SummonSelectionController selectionController,
        IPlayerTurnProgress playerTurnProgress,
        BattleResultController resultController,
        PlateBoardView boardView,
        PlayerHudView hudView,
        ManaView playerManaView,
        PlayerFeedbackView playerFeedbackView,
        IReadOnlyList<BattleBoardInputController> playerBoardInputs,
        IReadOnlyList<BattleBoardInputController> enemyBoardInputs)
    {
        handlePlayerCommandUseCase = useCase;
        summonSelectionController = selectionController;
        turnProgress = playerTurnProgress;
        battleResultController = resultController;
        plateBoardView = boardView;
        playerHudView = hudView;
        manaView = playerManaView;
        feedbackView = playerFeedbackView;
        playerPlates = playerBoardInputs;
        enemyPlates = enemyBoardInputs;
    }

    public void ResetPlayerSetting()
    {
        if (handlePlayerCommandUseCase == null)
        {
            return;
        }

        handlePlayerCommandUseCase.ResetPlayerSetting();
        ShowPlayerActionState();
    }

    public void StartPlayerTurn()
    {
        if (handlePlayerCommandUseCase == null)
        {
            return;
        }

        handlePlayerCommandUseCase.StartPlayerTurn();
        feedbackView?.Log("플레이어 턴을 시작했습니다.");
        feedbackView?.Log($"{gameObject.name} current mana: {handlePlayerCommandUseCase.GetMana()}");
        ShowPlayerActionState();

        if (battleResultController != null && turnProgress != null)
        {
            battleResultController.TryStartFailResult(
                turnProgress.GetClearTurn(),
                turnProgress.GetTurnCount());
        }
    }

    public void AddMana()
    {
        if (handlePlayerCommandUseCase == null)
        {
            return;
        }

        handlePlayerCommandUseCase.AddMana();
        ShowPlayerActionState();
    }

    public bool TryStopTurnForClearResult(bool isEnemyBoardClear)
    {
        if (battleResultController == null || turnProgress == null)
        {
            return false;
        }

        return battleResultController.TryStartClearResult(
            isEnemyBoardClear,
            turnProgress.GetClearTurn(),
            turnProgress.GetTurnCount());
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
            feedbackView?.PlayFail();
            feedbackView?.Log("모든 플레이트에 소환수가 있거나 지금은 소환할 수 없습니다.");
            return;
        }

        summonSelectionController.StartSummon(
            startResult.PlateIndex,
            false,
            CompleteSummonSelection);
        handlePlayerCommandUseCase.ConfirmSummonSelectionStarted(startResult);
        feedbackView?.Log(startResult.PlateIndex + "번째 플레이트에서 소환을 시작합니다.");
        feedbackView?.PlayClick();
        ShowPlayerActionState();
    }

    public void OnClickEndTurn()
    {
        if (handlePlayerCommandUseCase == null)
        {
            return;
        }

        PlayerCommandResult result = handlePlayerCommandUseCase.ExecuteEndTurn();
        if (!result.DidStart)
        {
            feedbackView?.PlayFail();
            return;
        }

        if (result.ActionResult == PlayerActionResult.Failed)
        {
            feedbackView?.PlayFail();
            feedbackView?.Log("플레이어 턴을 종료할 수 없습니다.");
            return;
        }

        feedbackView?.Log("플레이어 턴을 종료했습니다.");
        feedbackView?.PlayClick();
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
            feedbackView?.PlayFail();
            feedbackView?.Log("다시 뽑기할 소환수가 없거나 지금은 다시 뽑을 수 없습니다.");
            return;
        }

        if (!summonSelectionController.StartRedraw(CompleteSummonSelection))
        {
            handlePlayerCommandUseCase.CancelSummonSelectionStart();
            feedbackView?.PlayFail();
            ShowPlayerActionState();
            return;
        }

        handlePlayerCommandUseCase.ConfirmSummonSelectionStarted(startResult);
        feedbackView?.PlayClick();
        ShowPlayerActionState();
    }

    public void OnClickNormalAttack()
    {
        if (handlePlayerCommandUseCase == null)
        {
            return;
        }

        HandlePlayerAttackResult(handlePlayerCommandUseCase.ExecuteNormalAttack());
    }

    public void OnClickSpecialAttack()
    {
        ExecuteSpecialAttack(0);
    }

    public void OnClickSpecialAttack(int specialAttackIndex)
    {
        ExecuteSpecialAttack(specialAttackIndex);
    }

    private void ExecuteSpecialAttack(int specialAttackIndex)
    {
        if (handlePlayerCommandUseCase == null)
        {
            return;
        }

        PlayerCommandResult result = handlePlayerCommandUseCase.ExecuteSpecialAttack(specialAttackIndex);
        HandlePlayerAttackResult(result);
    }

    private void HandlePlayerAttackResult(PlayerCommandResult result)
    {
        if (!result.DidStart)
        {
            feedbackView?.PlayFail();
            return;
        }

        ShowAttackFeedback(result.AttackResult);
        if (result.ActionResult == PlayerActionResult.WaitingForTarget)
        {
            StartCoroutine(WaitForTargetPlateSelection(result.AttackResult.TargetBoard));
            ShowPlayerActionState();
            return;
        }

        plateBoardView?.ResetAllPlateHighlight();
        if (result.ActionResult == PlayerActionResult.Completed)
        {
            ProcessAfterPlayerAttack();
        }

        ShowPlayerActionState();
    }

    private void CompleteSummonSelection()
    {
        handlePlayerCommandUseCase?.CompleteSummonSelection();
        ShowPlayerActionState();
    }

    private IEnumerator WaitForTargetPlateSelection(PlayerAttackTargetBoard targetBoard)
    {
        bool targetsPlayerPlates = targetBoard == PlayerAttackTargetBoard.Player;
        ShowTargetSelection(targetsPlayerPlates);

        while (handlePlayerCommandUseCase.GetSelectedTargetPlateIndex() < 0)
        {
            if (handlePlayerCommandUseCase.IsTargetSelectionActive()
                && IsOutsideTargetPlatesClick(targetsPlayerPlates))
            {
                feedbackView?.Log("대상 선택이 취소되었습니다.");
                HideTargetSelection();
                CancelSelectedPlayerAttack();
                yield break;
            }

            yield return null;
        }

        int selectedTargetPlateIndex = handlePlayerCommandUseCase.GetSelectedTargetPlateIndex();
        string targetName = targetsPlayerPlates ? "플레이어" : "적";
        feedbackView?.Log($"특수 공격 대상으로 {targetName} 플레이트 {selectedTargetPlateIndex}번을 선택했습니다.");
        HideTargetSelection();
        PlayerCommandResult result = handlePlayerCommandUseCase.CompleteSelectedPlayerAttack(
            selectedTargetPlateIndex);
        HandlePlayerAttackResult(result);
    }

    private void CancelSelectedPlayerAttack()
    {
        handlePlayerCommandUseCase?.CancelSelectedPlayerAttack();
        plateBoardView?.ResetAllPlateHighlight();
        ShowPlayerActionState();
    }

    private void ShowTargetSelection(bool targetsPlayerPlates)
    {
        string targetName = targetsPlayerPlates ? "플레이어" : "적";
        feedbackView?.Log($"{targetName} 플레이트를 선택하세요.");
        summonSelectionController?.OnDarkBackground(true);
        plateBoardView?.DownTransparencyForWhoPlate(!targetsPlayerPlates);
        feedbackView?.Log($"{targetName} 플레이트 선택을 기다립니다.");
    }

    private void HideTargetSelection()
    {
        summonSelectionController?.OnDarkBackground(false);
    }

    private bool IsOutsideTargetPlatesClick(bool targetsPlayerPlates)
    {
        IReadOnlyList<BattleBoardInputController> targetPlates = targetsPlayerPlates
            ? playerPlates
            : enemyPlates;
        if (!Input.GetMouseButtonDown(0) || targetPlates == null)
        {
            return false;
        }

        Vector2 mousePosition = Input.mousePosition;
        foreach (BattleBoardInputController plate in targetPlates)
        {
            if (plate == null)
            {
                continue;
            }

            RectTransform plateRect = plate.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(plateRect, mousePosition))
            {
                return false;
            }
        }

        return true;
    }

    private void ShowAttackFeedback(PlayerAttackResult attackResult)
    {
        if (attackResult.Messages != null)
        {
            foreach (string message in attackResult.Messages)
            {
                feedbackView?.Log(message);
            }
        }

        if (attackResult.Warnings != null)
        {
            foreach (string warning in attackResult.Warnings)
            {
                feedbackView?.LogWarning(warning);
            }
        }

        if (attackResult.Sound == PlayerAttackSound.Click)
        {
            feedbackView?.PlayClick();
        }
        else if (attackResult.Sound == PlayerAttackSound.Fail)
        {
            feedbackView?.PlayFail();
        }
    }

    private void ProcessAfterPlayerAttack()
    {
        if (plateBoardView == null)
        {
            return;
        }

        TryStopTurnForClearResult(plateBoardView.IsEnemyPlateClear());
        plateBoardView.CompactEnemyPlates();
        playerHudView?.HideStatePanel();
    }

    private void ShowPlayerActionState()
    {
        if (handlePlayerCommandUseCase == null)
        {
            return;
        }

        manaView?.ShowMana(handlePlayerCommandUseCase.GetMana());
        playerHudView?.ShowSummonAvailable(handlePlayerCommandUseCase.CanSummon());
        playerHudView?.ShowRedrawAvailable(handlePlayerCommandUseCase.CanRedraw());
    }
}
