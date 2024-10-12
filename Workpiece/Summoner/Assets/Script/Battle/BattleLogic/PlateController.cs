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
    

    
   public void DownTransparencyForWhoPlate(bool isPlayer)
   {
        if (isPlayer)
        {
            for (int i = 0; i < playerPlates.Count; i++)
            {
                if (playerPlates[i].currentSummon != null)
                {
                    playerPlates[i].SetSummonImageTransparency(1.0f); //
                }
            }
        }
        else
        {
            for (int i = 0; i < enermyPlates.Count; i++)
            {
                if (enermyPlates[i].currentSummon != null)
                {
                    enermyPlates[i].SetSummonImageTransparency(1.0f); // ��������� ����
                }
            }
        }


   
   }

   private void SelectLightPlayer()
    {

    }



    // �÷��̾��� ��ȯ���� �ִ� �÷���Ʈ�� ���� �� ���� ����
    public void HighlightPlayerPlates()
    {
        // �÷��̾� �÷���Ʈ�� ����
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].currentSummon != null)
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
            if (playerPlates[i].currentSummon != null)
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
            if (enermyPlates[i].currentSummon != null)
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
            if (enermyPlates[i].currentSummon != null)
            {
                enermyPlates[i].Unhighlight(); // ���� ����
                enermyPlates[i].SetSummonImageTransparency(1.0f); // ���� �⺻������ �ǵ�����
            }
        }

        // �� �÷���Ʈ�� �ٽ� ���̰� �ϱ�
        ShowPlayerPlates();
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















    public int GetPlateIndex(Plate selectedPlate)
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
