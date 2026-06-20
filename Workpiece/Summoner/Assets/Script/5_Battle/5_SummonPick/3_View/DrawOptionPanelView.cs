using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 역할: 소환 후보 패널의 이미지와 선택 입력을 표시한다.
public class DrawOptionPanelView : MonoBehaviour,
    IPointerEnterHandler, //플레이트에 마우스 올렸을때 이벤트 인터페이스
    IPointerExitHandler,  //플레이트에 마우스가 벗어낫을때 이벤트 인터페이스
    IPointerClickHandler //플레이트 클릭시 상태창 
{
    [Header("소환이벤트 소환수")] 
    [SerializeField] private Summon assignedSummon; // 패널에 할당된 소환수
    [Header("소환이벤트 소환수 이미지")] 
    [SerializeField] private Image summonImage;

    private Action<Summon> selectSummon;

    public void SetSelectionHandler(Action<Summon> selectSummon)
    {
        this.selectSummon = selectSummon;
    }


    // 패널의 이미지를 설정하는 메소드
    public void SetSummonImage(Image image)
    {
        if (summonImage != null && image != null)
        {
            summonImage.sprite = image.sprite; // 패널에 소환수 이미지 할당
        }
    }

    public void ClearAssignedSummon()
    {
        assignedSummon = null;

        if (summonImage != null)
        {
            summonImage.sprite = null;
            summonImage.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 패널 클릭 시 해당 소환수 반환
        if (assignedSummon == null) return;

        selectSummon?.Invoke(assignedSummon);
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
        ClearAssignedSummon();
    }

    public Summon GetAssignedSummon()
    {
        return assignedSummon;
    }

    public void SetAssignedSummon(Summon summon)
    {
        assignedSummon = summon;
    }
}
