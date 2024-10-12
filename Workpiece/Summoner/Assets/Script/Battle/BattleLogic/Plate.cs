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
            // 기존 소환수가 있으면 파괴 (재소환 시)
            if (currentSummon != null && isResummon)
            {
                Destroy(currentSummon.gameObject);
                Debug.Log("기존 소환수가 파괴되었습니다.");
            }

            // 소환수 프리팹을 클론하여 생성
            Summon summonClone = Instantiate(summon);

            // 클론을 현재 플레이트의 자식으로 배치
            summonClone.transform.SetParent(this.transform, false);
            summonClone.transform.localPosition = Vector3.zero;  // 필요한 경우 위치 초기화

            // 클론의 이미지 투명도 설정 (완전히 투명하게)
            if (summonClone.image != null)
            {
                Color cloneColor = summonClone.image.color;
                cloneColor.a = 0.0f;  // 클론의 알파 값을 0으로 설정하여 투명하게 만듦
                summonClone.image.color = cloneColor;
            }

            // 플레이트의 summonImg에 소환수의 이미지 설정
            if (summonImg != null && summonClone.image != null && summonClone.image.sprite != null)
            {
                summonImg.sprite = summonClone.image.sprite; // summonImg에 소환수 이미지 설정

                // summonImg의 투명도를 1로 설정하여 완전히 보이게
                Color plateColor = summonImg.color;
                plateColor.a = 1.0f;  // 알파 값을 1로 설정 (완전 불투명)
                summonImg.color = plateColor;
            }

            // 클론된 소환수를 currentSummon으로 설정
            currentSummon = summonClone;
            isInSummon = true;

            // 소환수 초기화 로직 호출 (초기 능력치나 스킬 설정)
            summonClone.summonInitialize();

            Debug.Log($"소환수 {summonClone.getSummonName()} 을 {(isResummon ? "재소환" : "소환")}했습니다.");
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
            // 소환수 이미지를 초기 설정으로 되돌리기
            if (summonImg != null)
            {
                summonImg.sprite = null; // 이미지를 비움 (또는 기본 이미지로 변경)

                // 투명도를 0으로 설정 (완전히 투명하게)
                Color color = summonImg.color;
                color.a = 0f;
                summonImg.color = color;
            }

            // 소환수 제거
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
        if (isInSummon && summonController.IsSummoning()) //소환수가 플레이트에 있고 소환중
        {
            Highlight(); // 플레이트 강조
            SetSummonImageTransparency(1.0f); // 투명도 높이기
        }
       
        if (isInSummon && battleController.getIsAttaking()){ //공격중일때
            if (battleController.getIsHeal() && IsPlayerPlate())//힐이면서 아군플레이트만 강조
            {
                Highlight(); // 플레이트 강조
            }
            else if (!battleController.getIsHeal() && IsEnemyPlate()) //힐이 아닌 상태이상중 적 플레이트만 강조
            {
                Highlight();
            }
        }

    }


    //마우스가 벗어날때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSummon != null && summonController.IsSummoning()) //소환수가 플레이트에 있고 소환중
        {
            Unhighlight(); // 강조 해제
            SetSummonImageTransparency(0.5f); // 다시 흐리게
        }
        if (isInSummon && battleController.getIsAttaking())
        { //공격중일때
            if (battleController.getIsHeal() && IsPlayerPlate())//힐이고 아군플레이트면
            {
                Unhighlight(); // 플레이트 강조
            }
            else if(!battleController.getIsHeal() && IsEnemyPlate()) //힐이아니면 공격으로 간주
            {
                Unhighlight();
            }
        }

    }

    //해당 플레이트 클릭시 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        // 플레이어가 재소환 중이라면 상태 패널을 뜨지 않도록 함
        if (summonController.IsSummoning() && isInSummon)
        {
            summonController.SelectPlate(this);
            Unhighlight(); // 강조 해제
            SetSummonImageTransparency(1.0f); //투명도 되돌리기
        }

        //상태창 활성화
        else if (currentSummon != null && !summonController.IsSummoning() && !battleController.getIsAttaking())
        {
            Debug.Log("클릭된 플레이트의 소환수:" + currentSummon.getSummonName());
            statePanel.SetActive(true); //상태 패널 활성화

            // 현재 plate의 인덱스를 설정 (플레이트 리스트에서 자신을 찾음)
            int plateIndex = summonController.GetPlayerPlateIndex(this);  // GetPlateIndex 메소드를 통해 자신이 몇 번째인지 확인
            // BattleController에 선택된 플레이트 인덱스 전달
            summonController.setPlayerSelectedIndex(plateIndex);

            onMousePlateScript.setStatePanel(currentSummon); // 패널에 소환수 정보 전달 
        }
        // 공격 중에 클릭할 경우
        if (battleController.getIsAttaking() && isInSummon && battleController)
        {
            if (!battleController.getIsHeal()) //힐이 아니면 적 플레이트
            {
                // 적의 플레이트 인덱스를 가져옴
                int plateIndex = summonController.GetEnermyPlateIndex(this);

                if (plateIndex >= 0)
                {
                    // BattleController에 선택된 플레이트 인덱스 전달
                    summonController.setPlayerSelectedIndex(plateIndex);
                    Debug.Log($"적의 플레이트 {plateIndex}가 선택되었습니다.");
                    Unhighlight(); // 강조 해제
                }
                else
                {
                    Debug.Log("유효한 적의 플레이트가 선택되지 않았습니다.");
                }
            }
            else
            {
                // 적의 플레이트 인덱스를 가져옴
                int plateIndex = summonController.GetPlayerPlateIndex(this);

                if (plateIndex >= 0)
                {
                    // BattleController에 선택된 플레이트 인덱스 전달
                    summonController.setPlayerSelectedIndex(plateIndex);
                    Debug.Log($"아군의 플레이트 {plateIndex}가 선택되었습니다.");
                    Unhighlight(); // 강조 해제
                }
                else
                {
                    Debug.Log("유효한 아군의 플레이트가 선택되지 않았습니다.");
                }
            }
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

    // 현재 플레이트가 적의 플레이트인지 검사하는 메소드
    public bool IsEnemyPlate()
    {
        PlateController plateController = battleController.GetPlateController(); // summonController를 통해 PlateController에 접근
        return plateController.getEnermyPlates().Contains(this);
    }

    // 현재 플레이트가 플레이어의 플레이트인지 검사하는 메소드
    public bool IsPlayerPlate()
    {
        PlateController plateController = battleController.GetPlateController(); // summonController를 통해 PlateController에 접근
        return plateController.getPlayerPlates().Contains(this);
    }


    public Summon getSummon() //플레이트의 소환수를 반환
    {
        return currentSummon;
    }
}
