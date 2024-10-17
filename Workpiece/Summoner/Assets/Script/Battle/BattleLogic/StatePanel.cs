using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel: MonoBehaviour
{
    [SerializeField] private Summon stateSummon; //Summon
    [SerializeField] private Image summonImage; //image
    [SerializeField] private Slider HPSlider;

    [Header("공격 버튼들")]
    [SerializeField] private Button NormalAttackButton;
    [SerializeField] private Button SpecialAttackButton;


    //상태 판넬에 소환수정보 설정
    public void setStatePanel(Summon stateSummon, bool isEnemyPlate)
    {
        this.stateSummon = stateSummon;

        // 소환수의 이미지를 패널에 설정
        if (summonImage != null && stateSummon != null && stateSummon.image != null)
        {
            summonImage.sprite = stateSummon.image.sprite; // 소환수 이미지를 패널로 전달
        }

        // 소환수의 HP를 패널에 표시
        if (HPSlider != null && stateSummon != null)
        {
            float sliderHP = (float) (stateSummon.getNowHP() / stateSummon.getMaxHP());  // 체력 비율 계산
            HPSlider.value = sliderHP;  // 체력 비율에 따른 슬라이더 값 설정
        }

        //  적 플레이트 클릭 시 공격 버튼 비활성화
        if (isEnemyPlate)
        {
            NormalAttackButton.gameObject.SetActive(false);
            SpecialAttackButton.gameObject.SetActive(false);
        }
        else
        {
            NormalAttackButton.gameObject.SetActive(true);
            SpecialAttackButton.gameObject.SetActive(true);

            NormalAttackButton.image.sprite = stateSummon.normalAttackSprite;   // 일반 공격 스프라이트 설정
            SpecialAttackButton.image.sprite = stateSummon.specialAttackSprite; // 특수 공격 스프라이트 설정
        }
    }


    public Summon getStatePanelSummon()
    {
        return stateSummon;
    }

}
