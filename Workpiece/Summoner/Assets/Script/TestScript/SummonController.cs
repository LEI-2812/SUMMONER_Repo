using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Burst.Intrinsics;

public class SummonController : MonoBehaviour
{
    public static SummonController Instance; //�̱���
    public List<Summon> summons; //�ν����Ϳ� ���� ��ȯ�� ������Ʈ��
    public GameObject TakeSummonPanel;

    private Summon selectedSummon; // ���õ� ��ȯ��

    public List<PickSummonPanel> SelectSummonPanels; //�ǳڿ� ��� ��ȯ��
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

    public void randomTakeSummon()
    {

        // 50%, 35%, 15% Ȯ���� ��ȯ�� ���� �� �гο� �Ҵ�
        List<Summon> randomSelectedSummons = SummonRandomly();

        // ���õ� ��ȯ���� �� �гο� �Ҵ�
        for (int i = 0; i < SelectSummonPanels.Count && i < randomSelectedSummons.Count; i++)
        {
            Summon summon = randomSelectedSummons[i];
            SelectSummonPanels[i].assignedSummon = summon; // ��ȯ���� �гο� �Ҵ�
                                                           // �̹����� null���� üũ
            if (summon.image != null && summon.image.sprite != null)
            {
                SelectSummonPanels[i].SetSummonImage(summon.image); // ��ȯ���� ��������Ʈ�� �гο� �Ҵ�
            }
            else
            {
                Debug.LogWarning($"��ȯ�� {summon.summonName}�� �̹����� �������� �ʾҽ��ϴ�.");
            }
        }
        // ���� ���õ� ��ȯ���� �ʱ�ȭ
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

    // ��ȯ���� �����ϴ� �޼ҵ�
    public void OnSelectSummon(Summon summon)
    {
        selectedSummon = summon; // ���õ� ��ȯ���� ����
        Debug.Log($"{selectedSummon.summonName} ��ȯ���� �����߽��ϴ�.");
        TakeSummonPanel.SetActive(false);
    }

    // ���õ� ��ȯ���� ��ȯ�ϴ� �޼ҵ� (Player Ŭ�������� ���)
    public Summon GetSelectedSummon()
    {
        return selectedSummon;
    }
}
