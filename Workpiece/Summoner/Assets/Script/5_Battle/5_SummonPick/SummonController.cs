using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 역할: 플레이어의 일반 소환과 재소환 선택 흐름을 관리한다.
public class SummonController : MonoBehaviour
{
    private readonly SummonDrawService drawService = new SummonDrawService();
    private readonly SummonPickState pickState = new SummonPickState();
    private SummonPickView pickView;

    [SerializeField] private GameObject darkBackground; // 재소환 배경 처리할 판넬 (반투명)

    public bool isSummoning = false; // 재소환 중인지 확인하는 변수

    [Header("플레이어")]
    [SerializeField] private PlayerController player;
   //[SerializeField] private IReadOnlyList<Plate> playerPlates; // 플레이어가 사용할 플레이트 목록

    [Header("일반 소환 관련 오브젝트")]
    public List<Summon> summons; // 인스펙터에 넣을 소환수 오브젝트들
    [Header("재소환 관련 오브젝트")]
    [FormerlySerializedAs("reTakeSummonPanel")]
    [SerializeField] private GameObject redrawPanel;
    [FormerlySerializedAs("ReselectSummonPanels")]
    [SerializeField] private List<DrawOptionPanelView> redrawOptionPanels; // 패널에 띄울 소환수

    [Header("(외부 오브젝트)컨트롤러")]
    [SerializeField] private PlateController plateController;

    [Header("효과음")]
    [SerializeField] private AudioSource clickSound;

    [Header("소환수 프리팹 목록")]
    private PlateSelectionController plateSelectionController;


    private void Awake()
    {
        pickView = new SummonPickView(
            darkBackground,
            redrawPanel,
            redrawOptionPanels);
        plateSelectionController = new PlateSelectionController(plateController);
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

    // 일반 및 재소환을 처리하는 메서드
    public void StartSummon(int plateIndex, bool isRedraw)
    {
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
        StartCoroutine(DrawSelection(plateIndex));
    }

    private void StartRedrawSummon(int plateIndex)
    {
        OpenRedrawOptions();
        StartCoroutine(RedrawSelection(plateIndex));
    }

    // 소환 코루틴 (일반 소환 로직)
    private IEnumerator DrawSelection(int plateIndex)
    {
        while (pickState.SelectedSummon == null)
        {
            yield return null;
        }

        if (pickState.SelectedSummon != null) //3개중에 고른것
        {
            PlaceSelectedSummon(plateIndex, pickState.SelectedSummon, isResummon: false);
            Debug.Log($"플레이트 {plateIndex}에 소환 완료.");
        }

        FinishPickFlow(shouldShowAllPlates: false);
       
    }

    // 재소환 코루틴 (재소환 로직)
    private IEnumerator RedrawSelection(int plateIndex)
    {
        while (pickState.SelectedSummon == null)
        {
            yield return null;
        }

        if (pickState.SelectedSummon != null)
        {
            PlaceSelectedSummon(plateIndex, pickState.SelectedSummon, isResummon: true);
            Debug.Log($"플레이트 {plateIndex}에 재소환 완료.");
        }

        FinishPickFlow(shouldShowAllPlates: true);

    }

    //차례로 재소환 로직
    public bool StartRedraw()
    {
        if (!plateSelectionController.CanStartRedrawSelection())
        {
            return false;
        }

        StartSummoning();
        pickView.SetDarkBackground(true);
        plateSelectionController.ShowRedrawSelectablePlates();
        return true;
    }

    //재소환 중일때 플레이트 클릭시 이 메소드가 호출됨. 선택한 플레이트의 번호를 가져옴
    public void SelectPlate(Plate plate)
    {
        if (!plateSelectionController.TryGetPlayerPlateIndex(plate, out int selectedPlateIndex))
        {
            return;
        }

        pickState.SetSelectedPlateIndex(selectedPlateIndex);
        StartRedrawOptionSelection();
        clickSound.Play();
        Debug.Log($"플레이트 {selectedPlateIndex}가 선택되었습니다.");
    }

    //재소환 시킬 플레이트에 넣을 소환수를 선택하는 오브젝트 활성화
    private void StartRedrawOptionSelection()
    {
        StartSummon(pickState.SelectedPlateIndex, true); //소환을 시작(선택한 인덱스와 재소환여부를 true하여 호출)
    }

    private void OpenDrawOptions()
    {
        pickView.ShowDrawOptions(drawService.CreateDrawOptions(summons));
        pickState.ClearSelectedSummon();
    }

    //재소환시 소환시킬 소환수 선택
    private void OpenRedrawOptions()
    {
        plateController.HideAllPlates();
        pickView.ShowRedrawOptions(drawService.CreateDrawOptions(summons));
        pickState.ClearSelectedSummon();
    }

    // 소환 중인지 확인
    public bool IsSummoning()
    {
        return pickState.IsSummoning;
    }

    // 소환수 선택
    public void OnSelectSummon(Summon summon)
    {
        if (summon == null)
        {
            return;
        }

        pickState.SetSelectedSummon(summon);
        Debug.Log($"{pickState.SelectedSummon.GetSummonName()} 소환수를 선택했습니다.");
        plateSelectionController.RestorePlayerPlateSelectionView();
        pickView.HideOptionPanels();
        clickSound.Play();
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

    private void PlaceSelectedSummon(int plateIndex, Summon summon, bool isResummon)
    {
        plateController.GetPlayerPlates()[plateIndex]
            .SummonPlaceOnPlate(summon, isResummon);
        player.SetHasSummonedThisTurn(true);
    }

    private void FinishPickFlow(bool shouldShowAllPlates)
    {
        if (shouldShowAllPlates)
        {
            plateController.ShowAllPlates();
        }

        FinishSummoning();
        pickView.SetDarkBackground(false);
        plateController.ResetPlayerPlateHighlight();
    }

    // 어둡게 배경 활성화
    public void OnDarkBackground(bool onOff)
    {
        pickView.SetDarkBackground(onOff);
    }

    public bool GetIsSummoningBackGroundActive()
    {
        return pickView.IsDarkBackgroundActive();
    }

    public int GetPlayerPlateIndex(Plate selectedPlate)
    {
        return plateController.GetPlayerPlateIndex(selectedPlate);
    }

    public int GetEnermyPlateIndex(Plate selectedPlate)
    {
        return plateController.GetEnermyPlateIndex(selectedPlate);
    }

}
