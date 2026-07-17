using System.Collections.Generic;
using UnityEngine;

// 역할: 플레이어 공격 결과를 전투 화면과 입력 장치에 연결한다.
public class PlayerAttackOutput : IPlayerAttackOutput
{
    private readonly IReadOnlyList<BattleBoardInputController> playerPlates;
    private readonly IReadOnlyList<BattleBoardInputController> enemyPlates;
    private readonly SummonSelectionController summonSelectionController;
    private readonly PlateBoardView plateBoardController;
    private readonly PlayerFeedbackView feedbackView;

    public PlayerAttackOutput(
        IReadOnlyList<BattleBoardInputController> playerPlates,
        IReadOnlyList<BattleBoardInputController> enemyPlates,
        SummonSelectionController summonSelectionController,
        PlateBoardView plateBoardController,
        PlayerFeedbackView feedbackView)
    {
        this.playerPlates = playerPlates;
        this.enemyPlates = enemyPlates;
        this.summonSelectionController = summonSelectionController;
        this.plateBoardController = plateBoardController;
        this.feedbackView = feedbackView;
    }

    public void PlayClick() => feedbackView.PlayClick();
    public void PlayFail() => feedbackView.PlayFail();
    public void Log(string message) => feedbackView.Log(message);
    public void LogWarning(string message) => feedbackView.LogWarning(message);

    public void ShowTargetSelection(bool dimPlayerPlates)
    {
        summonSelectionController.OnDarkBackground(true);
        plateBoardController.DownTransparencyForWhoPlate(dimPlayerPlates);
    }

    public void HideTargetSelection() => summonSelectionController.OnDarkBackground(false);
    public void ResetTargetSelection() => plateBoardController.ResetAllPlateHighlight();

    public bool IsOutsidePlayerPlatesClick()
    {
        return IsOutsideClick(playerPlates);
    }

    public bool IsOutsideEnemyPlatesClick()
    {
        return IsOutsideClick(enemyPlates);
    }

    private static bool IsOutsideClick(IReadOnlyList<BattleBoardInputController> targetPlates)
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return false;
        }

        Vector2 mousePosition = Input.mousePosition;
        foreach (BattleBoardInputController plate in targetPlates)
        {
            RectTransform plateRect = plate.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(plateRect, mousePosition))
            {
                return false;
            }
        }

        return true;
    }
}
