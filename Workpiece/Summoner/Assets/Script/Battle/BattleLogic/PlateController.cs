using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{

    [SerializeField] private List<Plate> playerPlates;
    [SerializeField] private List<Plate> enermyPlates;

    private List<Plate> plates = new List<Plate>();

    // [Header("(�ܺ� ������Ʈ)��Ʈ�ѷ�")]

    private void Awake()
    {
        InitializePlates();
    }

    // �÷��̾��� �÷���Ʈ�� �ִ� ��� ��ȯ������ ��ȯ�ϴ� �޼ҵ�
    public List<Summon> getPlayerSummons()
    {
        List<Summon> playerSummons = new List<Summon>();
        foreach (Plate plate in playerPlates)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null)
            {
                playerSummons.Add(summon);
            }
        }
        return playerSummons;
    }

    // ���� �÷���Ʈ�� �ִ� ��� ��ȯ������ ��ȯ�ϴ� �޼ҵ�
    public List<Summon> getEnermySummons()
    {
        List<Summon> enermySummons = new List<Summon>();
        foreach (Plate plate in enermyPlates)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null)
            {
                enermySummons.Add(summon);
            }
        }
        return enermySummons;
    }

    //�� �÷���Ʈ�� ��ȯ���� �����ϴ���
    public bool IsEnermyPlateClear()
    {
        foreach (Plate plate in playerPlates) //�÷���Ʈ�� ��ȯ
        {
            Summon summon = plate.getCurrentSummon(); //�÷���Ʈ���� ��ȯ���� �����´�
            if (summon != null) //���� ��ȯ���� �ϳ��� �ִٸ� true�� ��ȯ
            {
                return false;
            }
        }

        return true;
    }

    public bool IsPlayerPlateClear()
    {
        foreach (Plate plate in enermyPlates) //�÷���Ʈ�� ��ȯ
        {
            Summon summon = plate.getCurrentSummon(); //�÷���Ʈ���� ��ȯ���� �����´�
            if (summon != null) //���� ��ȯ���� �ϳ��� �ִٸ� true�� ��ȯ
            {
                return false;
            }
        }

        return true;
    }

    // �� ��ȯ���� �� �÷���Ʈ�� �մ��� ����
    public void CompactEnermyPlates()
    {
        int nextAvailableIndex = 0; // ä���� �� �ε��� ��ġ

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon summon = enermyPlates[i].getCurrentSummon();
            if (summon != null)
            {
                // ���� ���� �ε����� nextAvailableIndex�� �ٸ��� ��ȯ���� ������ �ű��.
                if (i != nextAvailableIndex)
                {
                    // ���� ��ȯ���� nextAvailableIndex ��ġ�� ���� �̵�
                    enermyPlates[nextAvailableIndex].DirectMoveSummon(summon);
                    enermyPlates[i].RemoveSummon(); // ���� ��ġ�� ��ȯ���� ����
                }
                nextAvailableIndex++; // ���� ��ġ�� �̵�
            }
        }
    }



    public void DownTransparencyForWhoPlate(bool isPlayer)
   {
        if (isPlayer)
        {
            for (int i = 0; i < playerPlates.Count; i++)
            {
                if (playerPlates[i].getCurrentSummon() != null)
                {
                    playerPlates[i].SetSummonImageTransparency(0.5f);
                }
            }
        }
        else
        {
            for (int i = 0; i < enermyPlates.Count; i++)
            {
                if (enermyPlates[i].getCurrentSummon() != null)
                {
                    enermyPlates[i].SetSummonImageTransparency(0.5f);
                }
            }
        }


   
   }



    // �÷��̾��� ��ȯ���� �ִ� �÷���Ʈ�� ���� �� ���� ����
    public void HighlightPlayerPlates()
    {
        // �÷��̾� �÷���Ʈ�� ����
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].getCurrentSummon() != null)
            {
                playerPlates[i].Highlight(); // ��������� ����
                playerPlates[i].SetSummonImageTransparency(0.5f); // ��ȯ���� ���� ����
            }
        }

        // �� �÷���Ʈ�� �����
        HideEnemyPlates();
    }

    // ������ �����ϰ� ������ �⺻������ �ǵ�����
    public void ResetPlayerPlateHighlight()
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].getCurrentSummon() != null)
            {
                playerPlates[i].Unhighlight(); // ���� ����
                playerPlates[i].SetSummonImageTransparency(1.0f); // ���� �⺻������ �ǵ�����
            }
        }

        // �� �÷���Ʈ�� �ٽ� ���̰� �ϱ�
        ShowEnemyPlates();
    }

    // ���� �÷���Ʈ�� ����� �޼���
    public void HideEnemyPlates()
    {
        foreach (Plate plate in enermyPlates)
        {
            plate.gameObject.SetActive(false); // �� �÷���Ʈ�� ��Ȱ��ȭ
        }
    }

    // ���� �÷���Ʈ�� �ٽ� ���̰� �ϴ� �޼���
    private void ShowEnemyPlates()
    {
        foreach (Plate plate in enermyPlates)
        {
            plate.gameObject.SetActive(true); // �� �÷���Ʈ�� Ȱ��ȭ
        }
    }

    // �÷��̾��� ��ȯ���� �ִ� �÷���Ʈ�� ���� �� ���� ����
    public void HighlightEnermyPlates()
    {
        // �÷��̾� �÷���Ʈ�� ����
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (enermyPlates[i].getCurrentSummon() != null)
            {
                enermyPlates[i].Highlight(); // ��������� ����
                enermyPlates[i].SetSummonImageTransparency(0.5f); // ��ȯ���� ���� ����
            }
        }

        // �� �÷���Ʈ�� �����
        HidePlayerPlates();
    }

    // ������ �����ϰ� ������ �⺻������ �ǵ�����
    public void ResetEnermyPlateHighlight()
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (enermyPlates[i].getCurrentSummon() != null)
            {
                enermyPlates[i].Unhighlight(); // ���� ����
                enermyPlates[i].SetSummonImageTransparency(1.0f); // ���� �⺻������ �ǵ�����
            }
        }

        // �� �÷���Ʈ�� �ٽ� ���̰� �ϱ�
        ShowPlayerPlates();
    }

    public void ResetAllPlateHighlight()
    {
        ResetPlayerPlateHighlight();
        ResetEnermyPlateHighlight();
    }

    // ���� �÷���Ʈ�� ����� �޼���
    public void HidePlayerPlates()
    {
        foreach (Plate plate in playerPlates)
        {
            plate.gameObject.SetActive(false); // �� �÷���Ʈ�� ��Ȱ��ȭ
        }
    }

    // ���� �÷���Ʈ�� �ٽ� ���̰� �ϴ� �޼���
    private void ShowPlayerPlates()
    {
        foreach (Plate plate in playerPlates)
        {
            plate.gameObject.SetActive(true); // �� �÷���Ʈ�� Ȱ��ȭ
        }
    }

    public void HideAllPlates()
    {
        foreach (Plate plate in plates)
        {
            plate.gameObject.SetActive(false); // �� �÷���Ʈ�� ��Ȱ��ȭ
        }
    }

    public void ShowAllPlates()
    {
        foreach (Plate plate in plates)
        {
            plate.gameObject.SetActive(true); // �� �÷���Ʈ�� ��Ȱ��ȭ
        }
    }

    public int getClosestPlayerPlatesIndex(Summon attackingSummon) //�÷��̾� �÷���Ʈ�� ���� ������ �ִ� ��ȯ���� �ε����� ��ȯ
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon currentSummon = playerPlates[i].getCurrentSummon();
            if (currentSummon != null && currentSummon != attackingSummon) // ���� ��ȯ���� �����ϰ�, �����ϴ� ��ȯ���� ���� ���� ���
            {
                return i;
            }
        }

        return -1; // ������ ��ȯ���� ������ -1 ��ȯ
    }






    public int getPlateIndex(Plate selectedPlate)
    {
        return plates.IndexOf(selectedPlate);  // �÷��̾� �÷���Ʈ ����Ʈ���� �ε��� ã��
    }



    private void InitializePlates()
    {
        // playerPlates�� EnermyPlates�� ���� ����Ʈ�� ������ plates ����Ʈ�� �߰�
        plates.AddRange(playerPlates);
        plates.AddRange(enermyPlates);
    }

    public List<Plate> getPlayerPlates()
    {
        return playerPlates;
    }
    public List<Plate> getEnermyPlates()
    {
        return enermyPlates;
    }
    // �÷��̾� �÷���Ʈ ����Ʈ�� �����ϴ� �޼���
    public void setPlayerPlates(List<Plate> plates)
    {
        playerPlates = plates;
    }

    // �� �÷���Ʈ ����Ʈ�� �����ϴ� �޼���
    public void setEnermyPlates(List<Plate> plates)
    {
        enermyPlates = plates;
    }
}
