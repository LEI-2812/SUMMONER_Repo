using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{

    [SerializeField] private List<Plate> playerPlates;
    [SerializeField] private List<Plate> enermyPlates;

    private List<Plate> plates = new List<Plate>();

    // [Header("(외부 오브젝트)컨트롤러")]

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
                    enermyPlates[i].SetSummonImageTransparency(1.0f); // 노란색으로 강조
                }
            }
        }


   
   }

   private void SelectLightPlayer()
    {

    }



    // 플레이어의 소환수가 있는 플레이트만 강조 및 투명도 설정
    public void HighlightPlayerPlates()
    {
        // 플레이어 플레이트만 강조
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].currentSummon != null)
            {
                playerPlates[i].Highlight(); // 노란색으로 강조
                playerPlates[i].SetSummonImageTransparency(0.5f); // 소환수는 투명도 조절
            }
        }

        // 적 플레이트는 숨기기
        HideEnemyPlates();
    }

    // 강조를 해제하고 투명도를 기본값으로 되돌리기
    public void ResetPlayerPlateHighlight()
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].currentSummon != null)
            {
                playerPlates[i].Unhighlight(); // 강조 해제
                playerPlates[i].SetSummonImageTransparency(1.0f); // 투명도 기본값으로 되돌리기
            }
        }

        // 적 플레이트를 다시 보이게 하기
        ShowEnemyPlates();
    }

    // 적의 플레이트를 숨기는 메서드
    public void HideEnemyPlates()
    {
        foreach (Plate plate in enermyPlates)
        {
            plate.gameObject.SetActive(false); // 적 플레이트를 비활성화
        }
    }

    // 적의 플레이트를 다시 보이게 하는 메서드
    private void ShowEnemyPlates()
    {
        foreach (Plate plate in enermyPlates)
        {
            plate.gameObject.SetActive(true); // 적 플레이트를 활성화
        }
    }

    // 플레이어의 소환수가 있는 플레이트만 강조 및 투명도 설정
    public void HighlightEnermyPlates()
    {
        // 플레이어 플레이트만 강조
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (enermyPlates[i].currentSummon != null)
            {
                enermyPlates[i].Highlight(); // 노란색으로 강조
                enermyPlates[i].SetSummonImageTransparency(0.5f); // 소환수는 투명도 조절
            }
        }

        // 적 플레이트는 숨기기
        HidePlayerPlates();
    }

    // 강조를 해제하고 투명도를 기본값으로 되돌리기
    public void ResetEnermyPlateHighlight()
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (enermyPlates[i].currentSummon != null)
            {
                enermyPlates[i].Unhighlight(); // 강조 해제
                enermyPlates[i].SetSummonImageTransparency(1.0f); // 투명도 기본값으로 되돌리기
            }
        }

        // 적 플레이트를 다시 보이게 하기
        ShowPlayerPlates();
    }

    // 적의 플레이트를 숨기는 메서드
    public void HidePlayerPlates()
    {
        foreach (Plate plate in playerPlates)
        {
            plate.gameObject.SetActive(false); // 적 플레이트를 비활성화
        }
    }

    // 적의 플레이트를 다시 보이게 하는 메서드
    private void ShowPlayerPlates()
    {
        foreach (Plate plate in playerPlates)
        {
            plate.gameObject.SetActive(true); // 적 플레이트를 활성화
        }
    }

    public void HideAllPlates()
    {
        foreach (Plate plate in plates)
        {
            plate.gameObject.SetActive(false); // 적 플레이트를 비활성화
        }
    }

    public void ShowAllPlates()
    {
        foreach (Plate plate in plates)
        {
            plate.gameObject.SetActive(true); // 적 플레이트를 비활성화
        }
    }















    public int GetPlateIndex(Plate selectedPlate)
    {
        return plates.IndexOf(selectedPlate);  // 플레이어 플레이트 리스트에서 인덱스 찾기
    }



    private void InitializePlates()
    {
        // playerPlates와 EnermyPlates의 원본 리스트의 내용을 plates 리스트에 추가
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
    // 플레이어 플레이트 리스트를 설정하는 메서드
    public void setPlayerPlates(List<Plate> plates)
    {
        playerPlates = plates;
    }

    // 적 플레이트 리스트를 설정하는 메서드
    public void setEnermyPlates(List<Plate> plates)
    {
        enermyPlates = plates;
    }
}
