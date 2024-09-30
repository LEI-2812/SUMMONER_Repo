using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PickSummonPanel : MonoBehaviour,
    IPointerEnterHandler, //플레이트에 마우스 올렸을때 이벤트 인터페이스
    IPointerExitHandler,  //플레이트에 마우스가 벗어낫을때 이벤트 인터페이스
    IPointerClickHandler //플레이트 클릭시 상태창 
{
    [Header("소환이벤트 소환수")] 
    [SerializeField] private Summon assignedSummon; // 패널에 할당된 소환수
    [Header("소환이벤트 소환수 이미지")] 
    [SerializeField] private Image summonImage;


    // 패널의 이미지를 설정하는 메소드
    public void SetSummonImage(Image image)
    {
        if (summonImage != null && summonImage != null)
        {
            summonImage.sprite = image.sprite; // 패널에 소환수 이미지 할당
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 패널 클릭 시 해당 소환수 반환
        SummonController.Instance.OnSelectSummon(assignedSummon);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (summonImage != null)
        {
            summonImage.color = Color.yellow; // 마우스를 올리면 색상을 노란색으로 변경
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (summonImage != null)
        {
            summonImage.color = Color.white; // 마우스가 벗어나면 원래 색상으로 복구
        }
    }

    // 패널이 닫힐 때 설정을 초기화하는 메소드
    private void OnDisable()
    {
        // 초기 상태로 설정: 색상을 원래대로, 이미지 비우기
        if (summonImage != null)
        {
            summonImage.sprite = null; // 이미지 초기화
            summonImage.color = Color.white; // 색상을 기본값으로 복구
        }

        // 소환수 할당도 초기화
        assignedSummon = null;
    }




    public Summon getAssignedSummon()
    {
        return assignedSummon;
    }

    public void setAssignedSummon(Summon summon)
    {
        assignedSummon = summon;
    }
}
