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

    private Summon selectedSummon; // ���õ� ��ȯ��

    [Header("��ȯ�� ���� �г� (+��ư���� �ø� �� ����)")]
    [SerializeField] private List<PickSummonPanel> selectSummonPanels; // �гο� ��� ��ȯ��
    [SerializeField] private List<PickSummonPanel> ReselectSummonPanels; // �гο� ��� ��ȯ��

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
        if (isResummon)
        {
            randomReTakeSummon();
            StartCoroutine(ReSummonSelection(plateIndex));
        }
        else
        {
            randomTakeSummon();
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

    // ���ȯ �ڷ�ƾ (���ȯ ����)
    private IEnumerator ReSummonSelection(int plateIndex)
    {
        while (selectedSummon == null)
        {
            yield return null;
        }

        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
            player.SetHasSummonedThisTurn(true); // �÷��̾ ��ȯ������ �˸�
            Debug.Log($"�÷���Ʈ {plateIndex}�� ���ȯ �Ϸ�.");
        }
        // ���ȯ ���� �� ǥ�ÿ� ��׶��� ����
        isResummoning = false;
        darkenBackground.SetActive(false);
    }

    //���ʷ� ���ȯ ����
    public bool StartResummon()
    {
        if (playerPlates[0].currentSummon==null && playerPlates[1].currentSummon == null && playerPlates[2].currentSummon == null)
        {
            Debug.Log("�÷���Ʈ�� ��ȯ���� �����ϴ�.");
            return false;
        }

        SelectPlateAndResummon();
        return true;
    }

    //��ȯ���� �ִ� �÷���Ʈ�� ����
    private void SelectPlateAndResummon()
    {
        // ���ȯ ���� �� ǥ�ÿ� ��׶��� Ȱ��ȭ
        isResummoning = true;
        darkenBackground.SetActive(true);

        // ��ȯ���� �ִ� �÷���Ʈ�� ���� �� ���� ����
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].isInSummon)
            {
                playerPlates[i].Highlight(); //������ �����
                playerPlates[i].SetSummonImageTransparency(0.5f); //��ȯ���� ���� �����ϰ�
            }
        }
    }

    //���ȯ ���϶� �÷���Ʈ Ŭ���� �� �޼ҵ尡 ȣ���. ������ �÷���Ʈ�� ��ȣ�� ������
    public void SelectPlate(Plate plate)
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i] == plate)
            {
                selectedPlateIndex = i; //������ �÷���Ʈ�� ��ȣ�� �ִ´�.
                ResummonSelectStart();
                break;
            }
        }

        Debug.Log($"�÷���Ʈ {selectedPlateIndex}�� ���õǾ����ϴ�.");
    }

    //���ȯ ��ų �÷���Ʈ�� ���� ��ȯ���� �����ϴ� ������Ʈ Ȱ��ȭ
    public void ResummonSelectStart() //���ȯ ��ȯ�� ���ý���
    {
        reTakeSummonPanel.SetActive(true); //������ �ǳڵ��� Ȱ��ȭ��Ų��.
        StartSummon(selectedPlateIndex, true); //��ȯ�� ����(������ �ε����� ���ȯ���θ� true�Ͽ� ȣ��)
    }

    //�Ϲݽ� ��ȯ�� ��ȯ�� ����
    public void randomTakeSummon()
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

    //���ȯ�� ��ȯ��ų ��ȯ�� ����
    private void randomReTakeSummon()
    {
        reTakeSummonPanel.SetActive(true);
        List<Summon> randomSelectedSummons = SummonRandomly();

        for (int i = 0; i < ReselectSummonPanels.Count && i < randomSelectedSummons.Count; i++)
        {
            Summon summon = randomSelectedSummons[i];
            ReselectSummonPanels[i].setAssignedSummon(summon);

            if (summon.image != null && summon.image.sprite != null)
            {
                ReselectSummonPanels[i].SetSummonImage(summon.image);
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
            if (summon.SummonRank == rank)
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
    public bool IsReSummoning()
    {
        return isResummoning;
    }

    // ��ȯ�� ����
    public void OnSelectSummon(Summon summon)
    {
        selectedSummon = summon;
        Debug.Log($"{selectedSummon.SummonName} ��ȯ���� �����߽��ϴ�.");
        // ��ȯ���� �ִ� �÷���Ʈ�� ���� �� ���� �ǵ�����
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].isInSummon)
            {
                playerPlates[i].Unhighlight(); //���� �ǵ�����
                playerPlates[i].SetSummonImageTransparency(1.0f); //���� �ǵ�����
            }
        }
        takeSummonPanel.SetActive(false);
        reTakeSummonPanel.SetActive(false);
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

    // Ŭ���� plate�� �ε����� ��ȯ�ϴ� �޼ҵ�
    public int GetPlateIndex(Plate selectedPlate)
    {
        return playerPlates.IndexOf(selectedPlate);  // �÷��̾� �÷���Ʈ ����Ʈ���� �ε��� ã��
    }

    public void setPlayerSelectedIndex(int index)
    {
        player.setSelectedPlateIndex(index);
    }

}
