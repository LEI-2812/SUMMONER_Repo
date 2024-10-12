using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class getParenSummontImage : MonoBehaviour
{
    private Image summonImage;

    [Header("적 소환수 넣기")]
    [SerializeField] private Summon summon; // 적 소환수 직접 할당
    private Plate plate; // 부모 Plate 컴포넌트를 받기 위한 변수

    void Start()
    {
        // 부모 오브젝트에 붙어 있는 Plate 컴포넌트를 가져옴
        plate = GetComponentInParent<Plate>();

        // 소환수가 제대로 할당되었는지 확인
        if (summon != null)
        {
            // 부모 Plate에 소환수를 설정
            if (plate != null)
            {
                plate.SummonPlaceOnPlate(summon, isResummon: false); // 부모 Plate에 소환수 배치
            }

            // 소환수에서 Image 컴포넌트를 가져옴
            summonImage = summon.GetComponent<Image>();

            if (summonImage != null)
            {
                // 부모 오브젝트의 자식 이미지 컴포넌트를 가져옴
                Image childImage = GetComponent<Image>();
                if (childImage != null)
                {
                    // 소환수의 이미지를 자식 이미지로 설정
                    childImage.sprite = summonImage.sprite;

                    // 투명도를 1로 설정 (완전 불투명하게)
                    Color newColor = childImage.color;
                    newColor.a = 1f; // 알파 값(투명도)을 1로 설정
                    childImage.color = newColor;
                }
                else
                {
                    Debug.Log("자식 오브젝트에 Image 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.Log("소환수에 Image 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.Log("할당된 소환수가 없습니다.");
        }
    }

}
