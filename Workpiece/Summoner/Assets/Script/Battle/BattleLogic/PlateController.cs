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

    // 플레이어의 플레이트에 있는 모든 소환수들을 반환하는 메소드
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

    // 적의 플레이트에 있는 모든 소환수들을 반환하는 메소드
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

    //적 플레이트에 소환수가 존재하는지
    public bool IsEnermyPlateClear()
    {
        foreach (Plate plate in playerPlates) //플레이트를 순환
        {
            Summon summon = plate.getCurrentSummon(); //플레이트마다 소환수를 가져온다
            if (summon != null) //만약 소환수가 하나라도 있다면 true를 반환
            {
                return false;
            }
        }

        return true;
    }

    public bool IsPlayerPlateClear()
    {
        foreach (Plate plate in enermyPlates) //플레이트를 순환
        {
            Summon summon = plate.getCurrentSummon(); //플레이트마다 소환수를 가져온다
            if (summon != null) //만약 소환수가 하나라도 있다면 true를 반환
            {
                return false;
            }
        }

        return true;
    }

    // 적 소환수의 빈 플레이트를 앞당기는 로직
    public void CompactEnermyPlates()
    {
        int nextAvailableIndex = 0; // 채워야 할 인덱스 위치

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon summon = enermyPlates[i].getCurrentSummon();
            if (summon != null)
            {
                // 만약 현재 인덱스와 nextAvailableIndex가 다르면 소환수를 앞으로 옮긴다.
                if (i != nextAvailableIndex)
                {
                    // 현재 소환수를 nextAvailableIndex 위치로 직접 이동
                    enermyPlates[nextAvailableIndex].DirectMoveSummon(summon);
                    enermyPlates[i].RemoveSummon(); // 원래 위치의 소환수를 제거
                }
                nextAvailableIndex++; // 다음 위치로 이동
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



    // 플레이어의 소환수가 있는 플레이트만 강조 및 투명도 설정
    public void HighlightPlayerPlates()
    {
        // 플레이어 플레이트만 강조
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].getCurrentSummon() != null)
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
            if (playerPlates[i].getCurrentSummon() != null)
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
            if (enermyPlates[i].getCurrentSummon() != null)
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
            if (enermyPlates[i].getCurrentSummon() != null)
            {
                enermyPlates[i].Unhighlight(); // 강조 해제
                enermyPlates[i].SetSummonImageTransparency(1.0f); // 투명도 기본값으로 되돌리기
            }
        }

        // 적 플레이트를 다시 보이게 하기
        ShowPlayerPlates();
    }

    public void ResetAllPlateHighlight()
    {
        ResetPlayerPlateHighlight();
        ResetEnermyPlateHighlight();
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

    public int getClosestPlayerPlatesIndex(Summon attackingSummon) //플레이어 플레이트중 가장 가까이 있는 소환수의 인덱스를 반환
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon currentSummon = playerPlates[i].getCurrentSummon();
            if (currentSummon != null && currentSummon != attackingSummon) // 현재 소환수가 존재하고, 공격하는 소환수와 같지 않은 경우
            {
                return i;
            }
        }

        return -1; // 공격할 소환수가 없으면 -1 반환
    }






    public int getPlateIndex(Plate selectedPlate)
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
