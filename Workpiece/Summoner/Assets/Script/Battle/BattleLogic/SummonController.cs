using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour
{
    public static SummonController Instance; // 싱글톤

    [SerializeField] private GameObject darkBackground; // 재소환 배경 처리할 판넬 (반투명)

    private bool isSummoning = false; // 재소환 중인지 확인하는 변수

    [Header("플레이어")]
    [SerializeField] private Player player;
   //[SerializeField] private List<Plate> playerPlates; // 플레이어가 사용할 플레이트 목록

    [Header("일반 소환 관련 오브젝트")]
    public List<Summon> summons; // 인스펙터에 넣을 소환수 오브젝트들
    public GameObject takeSummonPanel;
    [SerializeField] private List<PickSummonPanel> selectSummonPanels; // 패널에 띄울 소환수

    [Header("재소환 관련 오브젝트")]
    public GameObject reTakeSummonPanel;
    [SerializeField] private List<PickSummonPanel> ReselectSummonPanels; // 패널에 띄울 소환수
    private int selectedPlateIndex = -1; // 소환시킬 플레이트 번호

    [Header("(외부 오브젝트)컨트롤러")]
    [SerializeField] private PlateController plateController;

    [Header("소환수 프리팹 목록")]
    private Summon selectedSummon; // 선택된 소환수

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 일반 및 재소환을 처리하는 메서드
    public void StartSummon(int plateIndex, bool isResummon)
    {

        if (isResummon)
        {
            randomReTakeSummon();
            StartCoroutine(ReSummonSelection(plateIndex));
        }
        else
        {
            darkBackground.SetActive(true);
            randomTakeSummon();
            StartCoroutine(TakeSummonSelection(plateIndex));
        }
    }

    // 소환 코루틴 (일반 소환 로직)
    private IEnumerator TakeSummonSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null) //3개중에 고른것
        {
            plateController.getPlayerPlates()[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: false);
            player.SetHasSummonedThisTurn(true); // 플레이어가 소환했음을 알림
            Debug.Log($"플레이트 {plateIndex}에 소환 완료.");
        }

        // 재소환 진행 중 표시와 백그라운드 종료
        isSummoning = false;
        darkBackground.SetActive(false);
        plateController.ResetPlayerPlateHighlight();
    }

    // 재소환 코루틴 (재소환 로직)
    private IEnumerator ReSummonSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null)
        {
            plateController.getPlayerPlates()[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
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
    public bool StartResummon()
    {
        if (plateController.getPlayerPlates()[0].getCurrentSummon() == null && plateController.getPlayerPlates()[1].getCurrentSummon() == null && plateController.getPlayerPlates()[2].getCurrentSummon() == null)
        {
            Debug.Log("플레이트에 소환수가 없습니다.");
            return false;
        }

        ReSummonPanelOpenAndHighlight();
        return true;
    }

    //소환수가 있는 플레이트만 강조
    private void ReSummonPanelOpenAndHighlight()
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
        for (int i = 0; i < plateController.getPlayerPlates().Count; i++)
        {
            if (plateController.getPlayerPlates()[i] == plate)
            {
                selectedPlateIndex = i; //선택한 플레이트의 번호를 넣는다.
                ResummonSelectStart();
                break;
            }
        }

        Debug.Log($"플레이트 {selectedPlateIndex}가 선택되었습니다.");
    }

    //재소환 시킬 플레이트에 넣을 소환수를 선택하는 오브젝트 활성화
    public void ResummonSelectStart() //재소환 소환수 선택시작
    {
        reTakeSummonPanel.SetActive(true); //선택할 판넬들을 활성화시킨다.
        StartSummon(selectedPlateIndex, true); //소환을 시작(선택한 인덱스와 재소환여부를 true하여 호출)
    }

    //일반시 소환할 소환수 선택
    public void randomTakeSummon()
    {
        takeSummonPanel.SetActive(true);
        List<Summon> randomSelectedSummons = SummonRandomly();

        for (int i = 0; i < selectSummonPanels.Count && i < randomSelectedSummons.Count; i++)
        {
            Summon summon = randomSelectedSummons[i];
            selectSummonPanels[i].setAssignedSummon(summon);

            if (summon.getImage() != null && summon.getImage().sprite != null)
            {
                selectSummonPanels[i].SetSummonImage(summon.getImage());
            }
        }

        selectedSummon = null;
    }

    //재소환시 소환시킬 소환수 선택
    private void randomReTakeSummon()
    {
        plateController.HideAllPlates();
        reTakeSummonPanel.SetActive(true);
        List<Summon> randomSelectedSummons = SummonRandomly();

        for (int i = 0; i < ReselectSummonPanels.Count && i < randomSelectedSummons.Count; i++)
        {
            Summon summon = randomSelectedSummons[i];
            ReselectSummonPanels[i].setAssignedSummon(summon);

            if (summon.getImage() != null && summon.getImage().sprite != null)
            {
                ReselectSummonPanels[i].SetSummonImage(summon.getImage());
            }
        }

        selectedSummon = null;
    }

    // 3마리의 소환수를 확률에 따라 선택하는 메소드
    private List<Summon> SummonRandomly()
    {
        List<Summon> selectedSummons = new List<Summon>(); // 소환 판넬에 보이게 할 소환수들

        // Low, Medium, High 확률에 맞춰 3마리의 소환수 선택
        for (int i = 0; i < 3; i++)
        {
            Summon summon = SelectSummonByRank();
            if (summon != null)
            {
                selectedSummons.Add(summon);
            }
        }

        return selectedSummons;
    }

    // 등급에 따른 확률로 소환수를 뽑음
    private Summon SelectSummonByRank()
    {
        float randomValue = Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값

        if (randomValue <= 50) // Low 등급 (50%)
        {
            selectedSummon = GetSummonByRank(SummonRank.Low);
        }
        else if (randomValue <= 85) // Medium 등급 (35%)
        {
            selectedSummon = GetSummonByRank(SummonRank.Medium);
        }
        else // High 등급 (15%)
        {
            selectedSummon = GetSummonByRank(SummonRank.High);
        }

        return selectedSummon;
    }

    // 특정 등급의 소환수 중 하나를 무작위로 선택하는 메소드
    private Summon GetSummonByRank(SummonRank rank)
    {
        List<Summon> availableSummons = new List<Summon>();

        // 소환수 리스트에서 해당 등급의 소환수들만 필터링
        foreach (Summon summon in summons)
        {
            if (summon.getSummonRank() == rank)
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
        Debug.Log($"{selectedSummon.getSummonName()} 소환수를 선택했습니다.");
        // 소환수가 있는 플레이트만 강조 및 투명도 되돌리기
        for (int i = 0; i < plateController.getPlayerPlates().Count; i++)
        {
            if (plateController.getPlayerPlates()[i].getIsInSummon())
            {
                plateController.getPlayerPlates()[i].Unhighlight(); //색상 되돌리기
                plateController.getPlayerPlates()[i].SetSummonImageTransparency(1.0f); //투명도 되돌리기
            }
        }
        takeSummonPanel.SetActive(false);
        reTakeSummonPanel.SetActive(false);
    }

    // 어둡게 배경 활성화
    public void OnDarkBackground(bool onOff)
    {
        darkBackground.SetActive(onOff);
    }

    public bool getIsSummoningBackGroundActive()
    {
        return darkBackground.activeSelf;
    }

    public int GetPlayerPlateIndex(Plate selectedPlate)
    {
        return plateController.getPlayerPlates().IndexOf(selectedPlate);  // 플레이어 플레이트 리스트에서 인덱스 찾기
    }

    public int GetEnermyPlateIndex(Plate selectedPlate)
    {
        return plateController.getEnermyPlates().IndexOf(selectedPlate);  // 플레이어 플레이트 리스트에서 인덱스 찾기
    }

    public void setPlayerSelectedIndex(int index)
    {
        player.setSelectedPlateIndex(index);
    }
}