using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour
{
    public static SummonController Instance; // 싱글톤

    [Header("플레이어 관련 오브젝트")]
    [SerializeField] private Player player;
    [SerializeField] private List<Plate> playerPlates; // 플레이어가 사용할 플레이트 목록

    [Header("일반 소환 관련 오브젝트")]
    public List<Summon> summons; // 인스펙터에 넣을 소환수 오브젝트들
    public GameObject takeSummonPanel;

    [Header("재소환 관련 오브젝트")]
    public GameObject reTakeSummonPanel;
    [SerializeField] private GameObject darkenBackground; // 재소환 배경 처리할 판넬 (반투명)
    private bool isResummoning = false; // 재소환 중인지 확인하는 변수
    private int selectedPlateIndex = -1; // 소환시킬 플레이트 번호
    private bool successResummon = false;

    private Summon selectedSummon; // 선택된 소환수

    [Header("소환수 선택 패널 (+버튼으로 늘릴 수 있음)")]
    [SerializeField] private List<PickSummonPanel> selectSummonPanels; // 패널에 띄울 소환수

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
        takeSummonPanel.SetActive(true);
        RandomTakeSummon();

        if (isResummon)
        {
            StartCoroutine(ReSummonSelection(plateIndex));
        }
        else
        {
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

        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon);
            player.SetHasSummonedThisTurn(true); // 플레이어가 소환했음을 알림
            Debug.Log($"플레이트 {plateIndex}에 소환 완료.");
        }
    }

    // 재소환 코루틴
    private IEnumerator ReSummonSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
            Debug.Log($"플레이트 {plateIndex}에 재소환 완료.");
        }
    }

    // 재소환 시작
    public void StartResummon()
    {
        if (!player.HasSummonedThisTurn())
        {
            Debug.Log("이 턴에 소환을 하지 않았습니다.");
            return;
        }

        StartCoroutine(SelectPlateAndResummon());
    }

    // 재소환 선택 코루틴
    private IEnumerator SelectPlateAndResummon()
    {
        isResummoning = true;
        darkenBackground.SetActive(true);

        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].isInSummon)
            {
                playerPlates[i].Highlight();
                playerPlates[i].SetSummonImageTransparency(0.5f);
            }
        }

        // 마우스 클릭을 기다림
        yield return StartCoroutine(CheckPlates());

        // CheckPlates()에서 취소되었는지 확인
        if (selectedPlateIndex == -1)
        {
            Debug.Log("재소환이 취소되었습니다.");
            isResummoning = false;
            darkenBackground.SetActive(false);
            successResummon = false;
            for (int i = 0; i < playerPlates.Count; i++)
            {
                // 소환수가 없는 플레이트의 이미지를 투명하게 설정
                if (!playerPlates[i].isInSummon)
                {
                    playerPlates[i].SetSummonImageTransparency(0.0f);
                }
                else
                {
                    // 소환수가 있는 플레이트는 원래 상태로 복원
                    playerPlates[i].SetSummonImageTransparency(1.0f);
                }
            }
            yield break; // 코루틴 종료
        }

        // 재소환 진행
        StartSummon(selectedPlateIndex, true);

        // 효과 복원
        darkenBackground.SetActive(false);
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].isInSummon)
            {
                playerPlates[i].Unhighlight();
                playerPlates[i].SetSummonImageTransparency(1.0f);
            }
        }

        selectedPlateIndex = -1;
        isResummoning = false;
    }

    // 플레이트 선택 코루틴
    private IEnumerator CheckPlates()
    {
        selectedPlateIndex = -1;

        while (selectedPlateIndex == -1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Plate clickedPlate = hit.collider.GetComponent<Plate>();

                    if (clickedPlate != null && clickedPlate.isInSummon)
                    {
                        // 클릭된 플레이트의 인덱스를 찾음
                        for (int i = 0; i < playerPlates.Count; i++)
                        {
                            if (playerPlates[i] == clickedPlate)
                            {
                                successResummon = true;
                                selectedPlateIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // 다른 곳을 클릭한 경우 코루틴 종료
                        successResummon = false;
                        selectedPlateIndex = -1;
                        yield break;
                    }
                }
                else
                {
                    // 클릭한 곳이 아무 오브젝트가 아닌 경우 재소환 취소
                    successResummon = false;
                    selectedPlateIndex = -1;
                    yield break;
                }
            }

            yield return null;
        }

        Debug.Log($"플레이트 {selectedPlateIndex}가 선택되었습니다.");
    }


    // 소환수 랜덤 선택
    public void RandomTakeSummon()
    {
        takeSummonPanel.SetActive(true);
        List<Summon> randomSelectedSummons = SummonRandomly();

        for (int i = 0; i < selectSummonPanels.Count && i < randomSelectedSummons.Count; i++)
        {
            Summon summon = randomSelectedSummons[i];
            selectSummonPanels[i].setAssignedSummon(summon);

            if (summon.image != null && summon.image.sprite != null)
            {
                selectSummonPanels[i].SetSummonImage(summon.image);
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
            if (summon.summonRank == rank)
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
    public bool IsResummoning()
    {
        return isResummoning;
    }

    // 소환수 선택
    public void OnSelectSummon(Summon summon)
    {
        selectedSummon = summon;
        Debug.Log($"{selectedSummon.summonName} 소환수를 선택했습니다.");
        takeSummonPanel.SetActive(false);
    }

    // 선택된 소환수 반환
    public Summon GetSelectedSummon()
    {
        return selectedSummon;
    }

    // 어둡게 배경 활성화
    public void OnResummonBackground()
    {
        darkenBackground.SetActive(true);
    }

    public List<Plate> getPlayerPlate()
    {
        return playerPlates;
    }

    // SummonController.cs
    public void SelectPlate(Plate plate)
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i] == plate)
            {
                selectedPlateIndex = i;
                break;
            }
        }

        Debug.Log($"플레이트 {selectedPlateIndex}가 선택되었습니다.");
    }

    public bool getIsSuccessSummon()
    {
        return successResummon;
    }
}
