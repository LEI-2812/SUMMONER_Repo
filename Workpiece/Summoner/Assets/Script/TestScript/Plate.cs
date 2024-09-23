using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Plate : MonoBehaviour, 
    IPointerEnterHandler, //플레이트에 마우스 올렸을때 이벤트 인터페이스
    IPointerExitHandler,  //플레이트에 마우스가 벗어낫을때 이벤트 인터페이스
    IPointerClickHandler //플레이트 클릭시 상태창 
{
    //plate를 프리팹시켜서 넣을것.
    public bool isInSummon = false; // 현재 소환수가 있는지 여부
    public Summon currentSummon;   // 플레이트 위에 있는 소환수
    public GameObject statePanel;  // 상태 패널 (On/Off)
    public OnMousePlate onMousePlateScript; // 상태 패널에 소환수 정보를 업데이트하는 스크립트
    public Image SummonImg;


    // 소환수를 플레이트에 배치
    public void SummonPlaceOnPlate(Summon summon)
    {
        if (!isInSummon)
        {
            currentSummon = summon;
            isInSummon = true;
            Debug.Log($"소환수 {summon.summonName} 을 플레이트에 소환");
        }
        else
        {
            Debug.Log("이미 이 플레이트에 소환수가 있습니다.");
        }
    }

    // 소환수가 사망하거나 플레이트에서 떠날 때
    public void RemoveSummon()
    {
        if (isInSummon)
        {
            currentSummon = null;
            isInSummon = false;
            Debug.Log("소환수 제거.");
        }
    }

    // 체력 체크 (플레이트 위 소환수의 체력 확인)
    public void CheckHealth()
    {
        if (currentSummon != null)
        {
            Debug.Log($"소환수 {currentSummon.summonName} 의 체력: {currentSummon.nowHP}");
        }
    }

    //마우스 올렸을때 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSummon != null && statePanel.activeSelf==false)
        {
            CheckHealth();
            statePanel.SetActive(true);
            onMousePlateScript.setStatePanel(currentSummon); // 패널에 소환수 정보 전달 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSummon != null && statePanel.activeSelf == true)
        {
            // 패널을 비활성화
            statePanel.SetActive(false);
        }
    }

    //해당 플레이트 클릭시 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("플레이트 클릭됨");
    }

}
