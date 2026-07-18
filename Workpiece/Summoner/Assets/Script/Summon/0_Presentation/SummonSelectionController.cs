using System;
using System.Collections.Generic;
using UnityEngine;

// 역할: SummonSelectionController의 책임을 정의한다.
public class SummonSelectionController : MonoBehaviour
{
    private readonly SummonDrawService drawService = new SummonDrawService();
    private readonly SummonPickState pickState = new SummonPickState();
    private readonly SelectSummonUseCase selectSummonUseCase = new SelectSummonUseCase();
    private SummonPickView pickView;
    private IReadOnlyList<BattleBoardInputController> playerPlates;
    private Action onSummonPlaced;
    private bool currentIsRedraw;

    [SerializeField] private GameObject darkBackground;

    [SerializeField] private bool isSummoning = false;

    [Header("참조")]
    [SerializeField] private List<Summon> summons;
    [Header("참조")]
    [SerializeField] private GameObject redrawPanel;
    [SerializeField] private List<DrawOptionPanelView> redrawOptionPanels;
    [Header("참조")]
    [SerializeField] private PlateBoardView plateBoardController;

    [Header("참조")]
    [SerializeField] private AudioSource clickSound;



    private void Awake()
    {
        pickView = new SummonPickView(
            darkBackground,
            redrawPanel,
            redrawOptionPanels);
        playerPlates = plateBoardController.GetPlayerPlates();
        ConnectOptionPanels(redrawOptionPanels);
    }

    private void ConnectOptionPanels(List<DrawOptionPanelView> optionPanels)
    {
        if (optionPanels == null) return;

        foreach (DrawOptionPanelView optionPanel in optionPanels)
        {
            if (optionPanel == null) continue;

            optionPanel.SetSelectionHandler(OnSelectSummon);
        }
    }

    public void StartSummon(int plateIndex, bool isRedraw, Action onSummonPlaced = null)
    {
        this.onSummonPlaced = onSummonPlaced;
        currentIsRedraw = isRedraw;
        pickState.SetSelectedPlateIndex(plateIndex);
        StartSummoning();

        if (isRedraw)
        {
            StartRedrawSummon(plateIndex);
        }
        else
        {
            StartNormalSummon(plateIndex);
        }
    }

    private void StartNormalSummon(int plateIndex)
    {
        pickView.SetDarkBackground(true);
        OpenDrawOptions();

    }

    private void StartRedrawSummon(int plateIndex)
    {
        OpenRedrawOptions();

    }

    public bool StartRedraw(Action onSummonPlaced = null)
    {
        if (!CanStartRedrawSelection())
        {
            return false;
        }

        this.onSummonPlaced = onSummonPlaced;
        StartSummoning();
        pickView.SetDarkBackground(true);
        ShowRedrawSelectablePlates();
        return true;
    }

    public void SelectPlate(BattleBoardInputController plate)
    {
        if (!TryGetPlayerPlateIndex(plate, out int selectedPlateIndex))
        {
            return;
        }

        pickState.SetSelectedPlateIndex(selectedPlateIndex);
        StartRedrawOptionSelection();
        clickSound.Play();
        Debug.Log($"다시 뽑기 대상 플레이트를 선택했습니다. 인덱스: {selectedPlateIndex}");
    }

    private void StartRedrawOptionSelection()
    {
        StartSummon(pickState.SelectedPlateIndex, true, onSummonPlaced);
    }

    private void OpenDrawOptions()
    {
        pickView.ShowDrawOptions(drawService.CreateDrawOptions(summons));
        pickState.ClearSelectedSummon();
    }

    private void OpenRedrawOptions()
    {
        plateBoardController.HideAllPlates();
        pickView.ShowRedrawOptions(drawService.CreateDrawOptions(summons));
        pickState.ClearSelectedSummon();
    }

    public bool IsSummoning()
    {
        return pickState.IsSummoning;
    }

    public void OnSelectSummon(Summon summon)
    {
        if (summon == null)
        {
            return;
        }

        pickState.SetSelectedSummon(summon);
        Debug.Log($"{pickState.SelectedSummon.GetSummonName()}을 선택했습니다.");
        SelectSummonResult selectResult = selectSummonUseCase.Execute(
            playerPlates == null ? 0 : playerPlates.Count,
            pickState.SelectedPlateIndex,
            summon,
            currentIsRedraw);
        if (!selectResult.DidSelect)
        {
            return;
        }

        BattleBoardInputController targetPlate = playerPlates[selectResult.PlateIndex];
        targetPlate.SummonPlaceOnPlate(selectResult.SelectedSummon, selectResult.IsRedraw);
        targetPlate.GetCurrentSummon()?.ApplyStageMultiplier(Summon.GetStatMultiplier());
        onSummonPlaced?.Invoke();

        RestorePlayerPlateSelectionView();
        pickView.HideOptionPanels();
        clickSound.Play();
        FinishPickFlow(currentIsRedraw);
    }

    private void StartSummoning()
    {
        pickState.StartSummoning();
        isSummoning = pickState.IsSummoning;
    }

    private void FinishSummoning()
    {
        pickState.FinishSummoning();
        isSummoning = pickState.IsSummoning;
    }

    private void FinishPickFlow(bool shouldShowAllPlates)
    {
        if (shouldShowAllPlates)
        {
            plateBoardController.ShowAllPlates();
        }

        FinishSummoning();
        pickView.SetDarkBackground(false);
        plateBoardController.ResetPlayerPlateHighlight();
        onSummonPlaced = null;
    }


    private bool CanStartRedrawSelection()
    {
        if (plateBoardController.IsPlayerPlateClear())
        {
            Debug.Log("다시 뽑기할 소환수가 없습니다.");
            return false;
        }

        return true;
    }

    private void ShowRedrawSelectablePlates()
    {
        plateBoardController.HighlightPlayerPlates();
    }

    private void RestorePlayerPlateSelectionView()
    {
        foreach (BattleBoardInputController plate in playerPlates)
        {
            if (!plate.GetIsInSummon())
            {
                continue;
            }

            plate.Unhighlight();
            plate.SetSummonImageTransparency(1.0f);
        }
    }

    private bool TryGetPlayerPlateIndex(BattleBoardInputController plate, out int selectedPlateIndex)
    {
        selectedPlateIndex = plateBoardController.GetPlayerPlateIndex(plate);
        if (selectedPlateIndex >= 0)
        {
            return true;
        }

        Debug.Log("선택한 플레이트가 플레이어 플레이트 목록에 없습니다.");
        return false;
    }
    public void OnDarkBackground(bool onOff)
    {
        pickView.SetDarkBackground(onOff);
    }

}
