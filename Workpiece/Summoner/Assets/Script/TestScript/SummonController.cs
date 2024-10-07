using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour
{
    public static SummonController Instance; // �̱���

    [Header("�÷��̾� ���� ������Ʈ")]
    [SerializeField] private Player player;
    [SerializeField] private List<Plate> playerPlates; // �÷��̾ ����� �÷���Ʈ ���

    [Header("�Ϲ� ��ȯ ���� ������Ʈ")]
    public List<Summon> summons; // �ν����Ϳ� ���� ��ȯ�� ������Ʈ��
    public GameObject takeSummonPanel;

    [Header("���ȯ ���� ������Ʈ")]
    public GameObject reTakeSummonPanel;
    [SerializeField] private GameObject darkenBackground; // ���ȯ ��� ó���� �ǳ� (������)
    private bool isResummoning = false; // ���ȯ ������ Ȯ���ϴ� ����
    private int selectedPlateIndex = -1; // ��ȯ��ų �÷���Ʈ ��ȣ
    private bool successResummon = false;

    private Summon selectedSummon; // ���õ� ��ȯ��

    [Header("��ȯ�� ���� �г� (+��ư���� �ø� �� ����)")]
    [SerializeField] private List<PickSummonPanel> selectSummonPanels; // �гο� ��� ��ȯ��

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �Ϲ� �� ���ȯ�� ó���ϴ� �޼���
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

    // ��ȯ �ڷ�ƾ (�Ϲ� ��ȯ ����)
    private IEnumerator TakeSummonSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon);
            player.SetHasSummonedThisTurn(true); // �÷��̾ ��ȯ������ �˸�
            Debug.Log($"�÷���Ʈ {plateIndex}�� ��ȯ �Ϸ�.");
        }
    }

    // ���ȯ �ڷ�ƾ
    private IEnumerator ReSummonSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
            Debug.Log($"�÷���Ʈ {plateIndex}�� ���ȯ �Ϸ�.");
        }
    }

    // ���ȯ ����
    public void StartResummon()
    {
        if (!player.HasSummonedThisTurn())
        {
            Debug.Log("�� �Ͽ� ��ȯ�� ���� �ʾҽ��ϴ�.");
            return;
        }

        StartCoroutine(SelectPlateAndResummon());
    }

    // ���ȯ ���� �ڷ�ƾ
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

        // ���콺 Ŭ���� ��ٸ�
        yield return StartCoroutine(CheckPlates());

        // CheckPlates()���� ��ҵǾ����� Ȯ��
        if (selectedPlateIndex == -1)
        {
            Debug.Log("���ȯ�� ��ҵǾ����ϴ�.");
            isResummoning = false;
            darkenBackground.SetActive(false);
            successResummon = false;
            for (int i = 0; i < playerPlates.Count; i++)
            {
                // ��ȯ���� ���� �÷���Ʈ�� �̹����� �����ϰ� ����
                if (!playerPlates[i].isInSummon)
                {
                    playerPlates[i].SetSummonImageTransparency(0.0f);
                }
                else
                {
                    // ��ȯ���� �ִ� �÷���Ʈ�� ���� ���·� ����
                    playerPlates[i].SetSummonImageTransparency(1.0f);
                }
            }
            yield break; // �ڷ�ƾ ����
        }

        // ���ȯ ����
        StartSummon(selectedPlateIndex, true);

        // ȿ�� ����
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

    // �÷���Ʈ ���� �ڷ�ƾ
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
                        // Ŭ���� �÷���Ʈ�� �ε����� ã��
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
                        // �ٸ� ���� Ŭ���� ��� �ڷ�ƾ ����
                        successResummon = false;
                        selectedPlateIndex = -1;
                        yield break;
                    }
                }
                else
                {
                    // Ŭ���� ���� �ƹ� ������Ʈ�� �ƴ� ��� ���ȯ ���
                    successResummon = false;
                    selectedPlateIndex = -1;
                    yield break;
                }
            }

            yield return null;
        }

        Debug.Log($"�÷���Ʈ {selectedPlateIndex}�� ���õǾ����ϴ�.");
    }


    // ��ȯ�� ���� ����
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

    // 3������ ��ȯ���� Ȯ���� ���� �����ϴ� �޼ҵ�
    private List<Summon> SummonRandomly()
    {
        List<Summon> selectedSummons = new List<Summon>(); // ��ȯ �ǳڿ� ���̰� �� ��ȯ����

        // Low, Medium, High Ȯ���� ���� 3������ ��ȯ�� ����
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

    // ��޿� ���� Ȯ���� ��ȯ���� ����
    private Summon SelectSummonByRank()
    {
        float randomValue = Random.Range(0f, 100f); // 0���� 100 ������ ������ ��

        if (randomValue <= 50) // Low ��� (50%)
        {
            selectedSummon = GetSummonByRank(SummonRank.Low);
        }
        else if (randomValue <= 85) // Medium ��� (35%)
        {
            selectedSummon = GetSummonByRank(SummonRank.Medium);
        }
        else // High ��� (15%)
        {
            selectedSummon = GetSummonByRank(SummonRank.High);
        }

        return selectedSummon;
    }

    // Ư�� ����� ��ȯ�� �� �ϳ��� �������� �����ϴ� �޼ҵ�
    private Summon GetSummonByRank(SummonRank rank)
    {
        List<Summon> availableSummons = new List<Summon>();

        // ��ȯ�� ����Ʈ���� �ش� ����� ��ȯ���鸸 ���͸�
        foreach (Summon summon in summons)
        {
            if (summon.summonRank == rank)
            {
                availableSummons.Add(summon);
            }
        }

        // �ش� ����� ��ȯ���� ������ ���, �� �߿��� �������� �ϳ� ����
        if (availableSummons.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSummons.Count);
            return availableSummons[randomIndex];
        }

        return null; // �ش� ����� ��ȯ���� ���� ��� null ��ȯ
    }


    // ��ȯ ������ Ȯ��
    public bool IsResummoning()
    {
        return isResummoning;
    }

    // ��ȯ�� ����
    public void OnSelectSummon(Summon summon)
    {
        selectedSummon = summon;
        Debug.Log($"{selectedSummon.summonName} ��ȯ���� �����߽��ϴ�.");
        takeSummonPanel.SetActive(false);
    }

    // ���õ� ��ȯ�� ��ȯ
    public Summon GetSelectedSummon()
    {
        return selectedSummon;
    }

    // ��Ӱ� ��� Ȱ��ȭ
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

        Debug.Log($"�÷���Ʈ {selectedPlateIndex}�� ���õǾ����ϴ�.");
    }

    public bool getIsSuccessSummon()
    {
        return successResummon;
    }
}
