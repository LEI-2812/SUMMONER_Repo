using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 타겟형 특수공격의 대상 선택 대기, 취소, 선택 완료 흐름을 처리한다.
public class PlayerTargetSelectionActions
{
    private readonly PlayerController player;
    private readonly SummonController summonController;
    private readonly BattleController battleController;
    private readonly PlateController plateController;
    private readonly PlayerFeedbackView feedbackView;

    public PlayerTargetSelectionActions(
        PlayerController player,
        SummonController summonController,
        BattleController battleController,
        PlateController plateController,
        PlayerFeedbackView feedbackView)
    {
        this.player = player;
        this.summonController = summonController;
        this.battleController = battleController;
        this.plateController = plateController;
        this.feedbackView = feedbackView;
    }

    public void StartTargetSelection(Action<int> onTargetSelected)
    {
        if (battleController.DoesCurrentSpecialAttackTargetPlayerPlate())
        {
            StartPlayerPlateSelection(onTargetSelected);
            return;
        }

        StartEnermyPlateSelection(onTargetSelected);
    }

    private void StartPlayerPlateSelection(Action<int> onTargetSelected)
    {
        feedbackView.Log("아군의 플레이트를 선택하세요.");
        player.StartCoroutine(WaitForPlayerPlateSelection(onTargetSelected));
    }

    private void StartEnermyPlateSelection(Action<int> onTargetSelected)
    {
        feedbackView.Log("적의 플레이트를 선택하세요.");
        player.StartCoroutine(WaitForEnermyPlateSelection(onTargetSelected));
    }

    private IEnumerator WaitForEnermyPlateSelection(Action<int> onTargetSelected)
    {
        return WaitForTargetPlateSelection(
            plateController.GetEnermyPlates(),
            true,
            "적의 플레이트를 선택하는 중입니다...",
            "적 플레이트 밖 클릭으로 선택 취소",
            "공격을 준비 중입니다. 선택한 플레이트 인덱스: {0}",
            "공격할 적의 플레이트 인덱스가 유효하지 않습니다.",
            onTargetSelected);
    }

    private IEnumerator WaitForPlayerPlateSelection(Action<int> onTargetSelected)
    {
        return WaitForTargetPlateSelection(
            plateController.GetPlayerPlates(),
            false,
            "아군의 플레이트를 선택하는 중입니다...",
            "플레이트 밖 클릭으로 선택 취소",
            "아군에게 버프를 준비 중입니다. 선택한 플레이트 인덱스: {0}",
            "아군의 플레이트 인덱스가 유효하지 않습니다.",
            onTargetSelected);
    }

    private IEnumerator WaitForTargetPlateSelection(
        IReadOnlyList<Plate> targetPlates,
        bool downTransparencyForPlayerPlate,
        string waitLog,
        string outsideClickLog,
        string executeLogFormat,
        string invalidLog,
        Action<int> onTargetSelected)
    {
        battleController.StartSpecialAttackTargetSelection();
        summonController.OnDarkBackground(true);
        plateController.DownTransparencyForWhoPlate(downTransparencyForPlayerPlate);
        battleController.ClearSpecialAttackTargetSelection();
        feedbackView.Log(waitLog);

        while (battleController.GetSelectedSpecialAttackTargetPlateIndex() < 0)
        {
            if (battleController.IsSpecialAttackTargetSelectionActive() &&
                Input.GetMouseButtonDown(0) &&
                !MousePositionInsidePlates(targetPlates))
            {
                feedbackView.Log(outsideClickLog);
                summonController.OnDarkBackground(false);
                battleController.CancelSpecialAttackTargetSelection();
                yield break;
            }

            yield return null;
        }

        int selectedTargetPlateIndex = battleController.GetSelectedSpecialAttackTargetPlateIndex();
        if (selectedTargetPlateIndex >= 0)
        {
            feedbackView.Log(string.Format(executeLogFormat, selectedTargetPlateIndex));
            summonController.OnDarkBackground(false);
            battleController.ClearSpecialAttackTargetSelection();
            onTargetSelected?.Invoke(selectedTargetPlateIndex);
            yield break;
        }

        feedbackView.LogError(invalidLog);
    }

    private bool MousePositionInsidePlates(IReadOnlyList<Plate> targetPlates)
    {
        Vector2 mousePosition = Input.mousePosition;

        foreach (Plate plate in targetPlates)
        {
            RectTransform plateRect = plate.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(plateRect, mousePosition))
            {
                return true;
            }
        }

        return false;
    }
}
