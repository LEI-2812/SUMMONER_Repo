using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SummonController : MonoBehaviour
{
    [SerializeField] private GameObject darkBackground; // 재소환 배경 처리할 판넬 (반투명)

    public bool isSummoning = false; // 재소환 중인지 확인하는 변수

    [Header("플레이어")]
    [SerializeField] private Player player;
   //[SerializeField] private List<Plate> playerPlates; // 플레이어가 사용할 플레이트 목록

    [Header("일반 소환 관련 오브젝트")]
    public List<Summon> summons; // 인스펙터에 넣을 소환수 오브젝트들
    [FormerlySerializedAs("takeSummonPanel")]
    [SerializeField] private GameObject drawPanel;
    [FormerlySerializedAs("selectSummonPanels")]
    [SerializeField] private List<DrawOptionPanelView> drawOptionPanels; // 패널에 띄울 소환수

    [Header("재소환 관련 오브젝트")]
    [FormerlySerializedAs("reTakeSummonPanel")]
    [SerializeField] private GameObject redrawPanel;
    [FormerlySerializedAs("ReselectSummonPanels")]
    [SerializeField] private List<DrawOptionPanelView> redrawOptionPanels; // 패널에 띄울 소환수
    private int selectedPlateIndex = -1; // 소환시킬 플레이트 번호

    [Header("(외부 오브젝트)컨트롤러")]
    [SerializeField] private PlateController plateController;

    [Header("효과음")]
    [SerializeField] private AudioSource clickSound;

    [Header("소환수 프리팹 목록")]
    private Summon selectedSummon; // 선택된 소환수


    private void Awake()
    {
        ConnectOptionPanels(drawOptionPanels);
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
        isSummoning = true;

        if (isRedraw)
        {
            OpenRedrawOptions();
            StartCoroutine(RedrawSelection(plateIndex));
        }
        else
        {
            darkBackground.SetActive(true);
            OpenDrawOptions();
            StartCoroutine(DrawSelection(plateIndex));
        }
    }

    // 소환 코루틴 (일반 소환 로직)
    private IEnumerator DrawSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null) //3개중에 고른것
        {
            plateController.GetPlayerPlates()[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: false);
            player.SetHasSummonedThisTurn(true); // 플레이어가 소환했음을 알림
            Debug.Log($"플레이트 {plateIndex}에 소환 완료.");
        }

        // 재소환 진행 중 표시와 백그라운드 종료
        isSummoning = false;
        darkBackground.SetActive(false);
        plateController.ResetPlayerPlateHighlight();
       
    }

    // 재소환 코루틴 (재소환 로직)
    private IEnumerator RedrawSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null)
        {
            plateController.GetPlayerPlates()[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
            player.SetHasSummonedThisTurn(true); // 플레이어가 소환했음을 알림
            Debug.Log($"플레이트 {plateIndex}에 재소환 완료.");
        }

        // 재소환 완료 후 모든 플레이트를 다시 보이게 함
        plateController.ShowAllPlates();

        // 재소환 진행 중 표시와 백그라운드 종료
        isSummoning = false;
        darkBackground.SetActive(false);

        plateController.ResetPlayerPlateHighlight();

    }

    //차례로 재소환 로직
    public bool StartRedraw()
    {
        if (plateController.IsPlayerPlateClear())
        {
            Debug.Log("플레이트에 소환수가 없습니다.");
            return false;
        }

        OpenRedrawPlateSelection();
        return true;
    }

    //소환수가 있는 플레이트만 강조
    private void OpenRedrawPlateSelection()
    {
        // 재소환 진행 중 표시와 백그라운드 활성화
        isSummoning = true;
        darkBackground.SetActive(true);

        // PlateController에서 소환수가 있는 플레이트만 강조
        plateController.HighlightPlayerPlates();
    }

    //재소환 중일때 플레이트 클릭시 이 메소드가 호출됨. 선택한 플레이트의 번호를 가져옴
    public void SelectPlate(Plate plate)
    {
        selectedPlateIndex = plateController.GetPlayerPlateIndex(plate);
        if (selectedPlateIndex < 0)
        {
            Debug.Log("선택한 플레이트가 플레이어 플레이트가 아닙니다.");
            return;
        }

        StartRedrawOptionSelection();
        clickSound.Play();
        Debug.Log($"플레이트 {selectedPlateIndex}가 선택되었습니다.");
    }

    //재소환 시킬 플레이트에 넣을 소환수를 선택하는 오브젝트 활성화
    private void StartRedrawOptionSelection()
    {
        redrawPanel.SetActive(true); //선택할 판넬들을 활성화시킨다.
        StartSummon(selectedPlateIndex, true); //소환을 시작(선택한 인덱스와 재소환여부를 true하여 호출)
    }

    private void OpenDrawOptions()
    {
        drawPanel.SetActive(true);
        ShowDrawOptions(drawOptionPanels);
        selectedSummon = null;
    }

    //재소환시 소환시킬 소환수 선택
    private void OpenRedrawOptions()
    {
        plateController.HideAllPlates();
        redrawPanel.SetActive(true);
        ShowDrawOptions(redrawOptionPanels);
        selectedSummon = null;
    }

    private void ShowDrawOptions(List<DrawOptionPanelView> optionPanels)
    {
        List<Summon> drawOptions = CreateDrawOptions();

        for (int i = 0; i < optionPanels.Count && i < drawOptions.Count; i++)
        {
            Summon summon = drawOptions[i];
            optionPanels[i].SetAssignedSummon(summon);

            if (summon.GetImage() != null && summon.GetImage().sprite != null)
            {
                optionPanels[i].SetSummonImage(summon.GetImage());
            }
        }
    }

    // 3마리의 소환수를 확률에 따라 선택하는 메소드
    private List<Summon> CreateDrawOptions()
    {
        List<Summon> selectedSummons = new List<Summon>(); // 소환 판넬에 보이게 할 소환수들

        // 3마리의 소환수를 선택할 때까지 반복
        while (selectedSummons.Count < 3)
        {
            Summon summon = SelectDrawOptionByRank();
            if (summon != null && !selectedSummons.Contains(summon)) // 중복 방지
            {
                selectedSummons.Add(summon);
            }
        }

        return selectedSummons;
    }

    // 등급에 따른 확률로 소환수를 뽑음
    private Summon SelectDrawOptionByRank()
    {
        float randomValue = Random.Range(0f, 100f);

        Summon summon = null;
        if (randomValue <= 50) // Low 등급 (50%)
        {
            summon = SelectRandomSummonByRank(SummonRank.Low);
        }
        else if (randomValue <= 85) // Medium 등급 (35%)
        {
            summon = SelectRandomSummonByRank(SummonRank.Medium);
        }
        else // High 등급 (15%)
        {
            summon = SelectRandomSummonByRank(SummonRank.High);
        }

        // 해당 등급의 소환수가 없으면 다른 등급으로 대체
        if (summon == null)
        {
            summon = SelectRandomSummonByRank(SummonRank.Low) ?? SelectRandomSummonByRank(SummonRank.Medium) ?? SelectRandomSummonByRank(SummonRank.High);
        }

        return summon;
    }

    // 특정 등급의 소환수 중 하나를 무작위로 선택하는 메소드
    private Summon SelectRandomSummonByRank(SummonRank rank)
    {
        List<Summon> availableSummons = new List<Summon>();

        // 소환수 리스트에서 해당 등급의 소환수들만 필터링
        foreach (Summon summon in summons)
        {
            if (summon.GetSummonRank() == rank)
            {
                availableSummons.Add(summon);
            }
        }

        // 해당 등급의 소환수가 존재할 경우, 그 중에서 무작위로 하나 선택
        if (availableSummons.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSummons.Count);
            return availableSummons[randomIndex];
        }

        return null; // 해당 등급의 소환수가 없을 경우 null 반환
    }


    // 소환 중인지 확인
    public bool IsSummoning()
    {
        return isSummoning;
    }

    // 소환수 선택
    public void OnSelectSummon(Summon summon)
    {
        selectedSummon = summon;
        Debug.Log($"{selectedSummon.GetSummonName()} 소환수를 선택했습니다.");
        // 소환수가 있는 플레이트만 강조 및 투명도 되돌리기
        for (int i = 0; i < plateController.GetPlayerPlates().Count; i++)
        {
            if (plateController.GetPlayerPlates()[i].GetIsInSummon())
            {
                plateController.GetPlayerPlates()[i].Unhighlight(); //색상 되돌리기
                plateController.GetPlayerPlates()[i].SetSummonImageTransparency(1.0f); //투명도 되돌리기
            }
        }
        drawPanel.SetActive(false);
        redrawPanel.SetActive(false);
        clickSound.Play();
    }

    // 어둡게 배경 활성화
    public void OnDarkBackground(bool onOff)
    {
        darkBackground.SetActive(onOff);
    }

    public bool GetIsSummoningBackGroundActive()
    {
        return darkBackground.activeSelf;
    }

    public int GetPlayerPlateIndex(Plate selectedPlate)
    {
        return plateController.GetPlayerPlateIndex(selectedPlate);
    }

    public int GetEnermyPlateIndex(Plate selectedPlate)
    {
        return plateController.GetEnermyPlateIndex(selectedPlate);
    }

    public void SetPlayerSelectedIndex(int index)
    {
        player.SetSelectedPlateIndex(index);
    }

}
