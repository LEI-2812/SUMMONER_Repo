using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 역할: DrawOptionPanelView의 책임을 정의한다.
public class DrawOptionPanelView : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("참조")]
    [SerializeField] private Summon assignedSummon;
    [Header("참조")] 
    [SerializeField] private Image summonImage;

    private Action<Summon> selectSummon;

    public void SetSelectionHandler(Action<Summon> selectSummon)
    {
        this.selectSummon = selectSummon;
    }


    public void SetSummonImage(Image image)
    {
        if (summonImage != null && image != null)
        {
            summonImage.sprite = image.sprite;
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
        if (assignedSummon == null) return;

        selectSummon?.Invoke(assignedSummon);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (summonImage != null)
        {
            summonImage.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (summonImage != null)
        {
            summonImage.color = Color.white;
        }
    }

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
