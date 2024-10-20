using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour
{
    public static SummonController Instance; // �̱���

    [SerializeField] private GameObject darkBackground; // ���ȯ ��� ó���� �ǳ� (������)

    private bool isSummoning = false; // ���ȯ ������ Ȯ���ϴ� ����

    [Header("�÷��̾�")]
    [SerializeField] private Player player;
   //[SerializeField] private List<Plate> playerPlates; // �÷��̾ ����� �÷���Ʈ ���

    [Header("�Ϲ� ��ȯ ���� ������Ʈ")]
    public List<Summon> summons; // �ν����Ϳ� ���� ��ȯ�� ������Ʈ��
    public GameObject takeSummonPanel;
    [SerializeField] private List<PickSummonPanel> selectSummonPanels; // �гο� ��� ��ȯ��

    [Header("���ȯ ���� ������Ʈ")]
    public GameObject reTakeSummonPanel;
    [SerializeField] private List<PickSummonPanel> ReselectSummonPanels; // �гο� ��� ��ȯ��
    private int selectedPlateIndex = -1; // ��ȯ��ų �÷���Ʈ ��ȣ

    [Header("(�ܺ� ������Ʈ)��Ʈ�ѷ�")]
    [SerializeField] private PlateController plateController;

    [Header("��ȯ�� ������ ���")]
    private Summon selectedSummon; // ���õ� ��ȯ��

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
            darkBackground.SetActive(true);
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

        if (selectedSummon != null) //3���߿� ����
        {
            plateController.getPlayerPlates()[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: false);
            player.SetHasSummonedThisTurn(true); // �÷��̾ ��ȯ������ �˸�
            Debug.Log($"�÷���Ʈ {plateIndex}�� ��ȯ �Ϸ�.");
        }

        // ���ȯ ���� �� ǥ�ÿ� ��׶��� ����
        isSummoning = false;
        darkBackground.SetActive(false);
        plateController.ResetPlayerPlateHighlight();
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
            plateController.getPlayerPlates()[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
            player.SetHasSummonedThisTurn(true); // �÷��̾ ��ȯ������ �˸�
            Debug.Log($"�÷���Ʈ {plateIndex}�� ���ȯ �Ϸ�.");
        }

        // ���ȯ �Ϸ� �� ��� �÷���Ʈ�� �ٽ� ���̰� ��
        plateController.ShowAllPlates();

        // ���ȯ ���� �� ǥ�ÿ� ��׶��� ����
        isSummoning = false;
        darkBackground.SetActive(false);

        plateController.ResetPlayerPlateHighlight();
    }

    //���ʷ� ���ȯ ����
    public bool StartResummon()
    {
        if (plateController.getPlayerPlates()[0].getCurrentSummon() == null && plateController.getPlayerPlates()[1].getCurrentSummon() == null && plateController.getPlayerPlates()[2].getCurrentSummon() == null)
        {
            Debug.Log("�÷���Ʈ�� ��ȯ���� �����ϴ�.");
            return false;
        }

        ReSummonPanelOpenAndHighlight();
        return true;
    }

    //��ȯ���� �ִ� �÷���Ʈ�� ����
    private void ReSummonPanelOpenAndHighlight()
    {
        // ���ȯ ���� �� ǥ�ÿ� ��׶��� Ȱ��ȭ
        isSummoning = true;
        darkBackground.SetActive(true);

        // PlateController���� ��ȯ���� �ִ� �÷���Ʈ�� ����
        plateController.HighlightPlayerPlates();
    }

    //���ȯ ���϶� �÷���Ʈ Ŭ���� �� �޼ҵ尡 ȣ���. ������ �÷���Ʈ�� ��ȣ�� ������
    public void SelectPlate(Plate plate)
    {
        for (int i = 0; i < plateController.getPlayerPlates().Count; i++)
        {
            if (plateController.getPlayerPlates()[i] == plate)
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

            if (summon.getImage() != null && summon.getImage().sprite != null)
            {
                selectSummonPanels[i].SetSummonImage(summon.getImage());
            }
        }

        selectedSummon = null;
    }

    //���ȯ�� ��ȯ��ų ��ȯ�� ����
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
            if (summon.getSummonRank() == rank)
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
    public bool IsSummoning()
    {
        return isSummoning;
    }

    // ��ȯ�� ����
    public void OnSelectSummon(Summon summon)
    {
        selectedSummon = summon;
        Debug.Log($"{selectedSummon.getSummonName()} ��ȯ���� �����߽��ϴ�.");
        // ��ȯ���� �ִ� �÷���Ʈ�� ���� �� ���� �ǵ�����
        for (int i = 0; i < plateController.getPlayerPlates().Count; i++)
        {
            if (plateController.getPlayerPlates()[i].getIsInSummon())
            {
                plateController.getPlayerPlates()[i].Unhighlight(); //���� �ǵ�����
                plateController.getPlayerPlates()[i].SetSummonImageTransparency(1.0f); //���� �ǵ�����
            }
        }
        takeSummonPanel.SetActive(false);
        reTakeSummonPanel.SetActive(false);
    }

    // ��Ӱ� ��� Ȱ��ȭ
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
        return plateController.getPlayerPlates().IndexOf(selectedPlate);  // �÷��̾� �÷���Ʈ ����Ʈ���� �ε��� ã��
    }

    public int GetEnermyPlateIndex(Plate selectedPlate)
    {
        return plateController.getEnermyPlates().IndexOf(selectedPlate);  // �÷��̾� �÷���Ʈ ����Ʈ���� �ε��� ã��
    }

    public void setPlayerSelectedIndex(int index)
    {
        player.setSelectedPlateIndex(index);
    }
}