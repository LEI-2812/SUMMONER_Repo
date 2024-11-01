using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class getParenSummontImage : MonoBehaviour
{
    [Header("적 소환수 넣기")]
    [SerializeField] private Summon summon; // 적 소환수 직접 할당
    private Plate plate; // 부모 Plate 컴포넌트를 받기 위한 변수

    void Awake()
    {
        // 부모 오브젝트에 붙어 있는 Plate 컴포넌트를 가져옴
        plate = GetComponent<Plate>();

        // 소환수가 제대로 할당되었는지 확인
        if (summon != null)
        {
            // 부모 Plate에 소환수를 설정
            if (plate != null)
            {
                plate.SummonPlaceOnPlate(summon, isResummon: false); // 부모 Plate에 소환수 배치
            }
        }
        else
        {
            Debug.Log("할당된 소환수가 없습니다.");
        }
    }

}
