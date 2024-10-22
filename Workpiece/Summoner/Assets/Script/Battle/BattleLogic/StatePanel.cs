using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public interface stateObserver
{
    void StateUpdate();
}

public interface UpdateStateObserver
{
    void AddObserver(stateObserver observer);
    void RemoveObserver(stateObserver observer);
    void NotifyObservers();
}

public class StatePanel: MonoBehaviour, stateObserver
{
    [SerializeField] private Summon stateSummon; //Summon
    [SerializeField] private Image summonImage; //image
    [SerializeField] private Slider HPSlider;
    [SerializeField] private Image shieldFillImage;

    [Header("공격 버튼들")]
    [SerializeField] private Button NormalAttackButton;
    [SerializeField] private Button SpecialAttackButton;


    //상태 판넬에 소환수정보 설정
    public void setStatePanel(Summon stateSummon, bool isEnemyPlate)
    {
        this.stateSummon = stateSummon;
        stateSummon.AddObserver(this); // 옵저버로 등록

        // 소환수의 이미지를 패널에 설정
        if (summonImage != null && stateSummon != null && stateSummon.getImage() != null)
        {
            summonImage.sprite = stateSummon.getImage().sprite; // 소환수 이미지를 패널로 전달
        }

        // 소환수의 HP를 패널에 표시
        if (HPSlider != null && stateSummon != null)
        {
            float sliderHP = (float) (stateSummon.getNowHP() / stateSummon.getMaxHP());  // 체력 비율 계산
            HPSlider.value = sliderHP;  // 체력 비율에 따른 슬라이더 값 설정

            // 쉴드 비율 계산
            float shieldAmount = (float) stateSummon.getShield(); // 현재 쉴드량
            float shieldRatio = (float) (shieldAmount / stateSummon.getMaxHP()); // 최대 체력 기준으로 쉴드 비율 계산

            if (shieldFillImage != null)
            {
                shieldFillImage.fillAmount = shieldRatio; // 쉴드 비율에 따라 슬라이더의 fill amount 설정
            }

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

        StateUpdate();
    }


    public Summon getStatePanelSummon()
    {
        return stateSummon;
    }

    public void StateUpdate()
    {
        if (stateSummon != null)
        {
            // 소환수가 사망 상태인지 체크
            if (stateSummon.getNowHP() <= 0)
            {
                // 사망 시 StatePanel 비활성화
                gameObject.SetActive(false);
                return;
            }

            // HP 슬라이더 업데이트
            float sliderHP = (float)(stateSummon.getNowHP() / stateSummon.getMaxHP());
            HPSlider.value = sliderHP;
        }
    }
}
