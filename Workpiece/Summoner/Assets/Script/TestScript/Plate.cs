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
    public StatePanel onMousePlateScript; // 상태 패널에 소환수 정보를 업데이트하는 스크립트
    public Image summonImg;

    private Image plateImage; // 자기 자신의 Image 컴포넌트
    private Color originalColor;

    [Header("컨트롤러들")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private BattleController battleController;

    void Start()
    {
        statePanel.SetActive(false);
        plateImage = GetComponent<Image>(); // 자신의 Image 컴포넌트 가져오기
        originalColor = plateImage.color; // 원래 색상 저장
    }

    // 소환수를 플레이트에 배치
    public void SummonPlaceOnPlate(Summon summon, bool isResummon = false)
    {
        // 이미 소환수가 있어도 재소환이면 진행
        if (!isInSummon || isResummon)
        {
            currentSummon = summon;
            isInSummon = true;

            // 소환수 이미지 셋팅
            summonImg.sprite = summon.image.sprite;
            // 투명도를 255로 설정하여 완전히 불투명하게 만들기
            Color color = summonImg.color;
            color.a = 1.0f; // 알파 값을 1로 설정 (255/255)
            summonImg.color = color;

            Debug.Log($"소환수 {summon.SummonName} 을 {(isResummon ? "재소환" : "소환")}했습니다.");
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

    // 플레이트 강조 (색상 변경)
    public void Highlight()
    {
        plateImage.color = Color.yellow; // 이미지의 색상을 노란색으로 변경
    }

    // 강조 해제
    public void Unhighlight()
    {
        plateImage.color = originalColor; // 이미지의 색상을 원래대로 복원
    }


    //마우스 올렸을때 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSummon != null) //소환수가 있으면
        {
            SetSummonImageTransparency(1.0f); // 투명도를 높여 더 진하게 보이게
        }
        if (isInSummon && summonController.IsReSummoning()) // 재소환 중이고 소환수가 있는 경우
        {
            Highlight(); // 플레이트 강조
            SetSummonImageTransparency(1.0f); // 투명도 높이기
        }
    }


    //마우스가 벗어날때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSummon != null && summonController.IsReSummoning()) //재소환중일때 더 진하게
        {
            Unhighlight(); // 강조 해제
            SetSummonImageTransparency(0.5f); // 다시 흐리게
        }

    }

    //해당 플레이트 클릭시 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        // 플레이어가 재소환 중이라면 상태 패널을 뜨지 않도록 함
        if (summonController.IsReSummoning() && isInSummon)
        {
                summonController.SelectPlate(this);
                Unhighlight(); // 강조 해제
                SetSummonImageTransparency(1.0f); //투명도 되돌리기
        }

        //상태창 활성화
        else if (currentSummon != null && !summonController.IsReSummoning())
        {
            Debug.Log("클릭된 플레이트의 소환수:" + currentSummon.name);
            statePanel.SetActive(true); //상태 패널 활성화
            onMousePlateScript.setStatePanel(currentSummon); // 패널에 소환수 정보 전달 
        }
    }

    // 소환수 이미지 투명도 설정
    public void SetSummonImageTransparency(float alpha)
    {
        if (summonImg != null)
        {
            Color color = summonImg.color;
            color.a = alpha; // 투명도 설정
            summonImg.color = color;
        }
    }

}
