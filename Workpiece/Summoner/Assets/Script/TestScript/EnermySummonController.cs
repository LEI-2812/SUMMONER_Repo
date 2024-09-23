using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermySummonController : MonoBehaviour
{
    private List<Summon> enermySummonList;

    public Summon slime;
    public Summon kingSlime;
    public List<Summon> EnermySummonSetting(int stage)
    {
        switch(stage){
            case 1:
                Debug.Log("슬라임, 킹슬라임이 소환됨 ");
                enermySummonList.Add(slime);
                enermySummonList.Add(kingSlime);
                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            case 5:

                break;
            case 6:

                break;
            case 7:

                break;
            //스테이지 추가시 여기에 추가
            default:
                Debug.Log("잘못된 스테이지 입력을 받았습니다.");
                break;
        }
        return enermySummonList;
    }
}
