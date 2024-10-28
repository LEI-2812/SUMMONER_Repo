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
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image statePanel;

    [Header("공격 버튼들")]
    [SerializeField] private Button NormalAttackButton;
    [SerializeField] private Button SpecialAttackButton;



    //상태 판넬에 소환수정보 설정
    public void setStatePanel(Summon stateSummon, bool isEnemyPlate)
    {
        statePanel.gameObject.SetActive(true);
        this.stateSummon = stateSummon;
        stateSummon.AddObserver(this); // 옵저버로 등록

        // 체력바를 초기 설정
        UpdateHealthSlider();

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
        Color color = shieldImage.color;
        color.a = 0.7f;
        shieldImage.color = color;

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
            float currentHP = (float)stateSummon.getNowHP();
            float maxHP = (float)stateSummon.getMaxHP();

            // 체력 비율을 0과 1 사이로 설정
            float sliderHP = currentHP / maxHP;
            HPSlider.value = Mathf.Clamp(sliderHP, 0f, 1f);
            //Debug.Log("체력 슬라이더 최종 값: " + HPSlider.value);

            // 쉴드 비율 계산
            float shieldAmount = (float)stateSummon.getShield(); // 현재 쉴드량
            float initialShieldAmount = (float)stateSummon.GetInitialShield();
            //Debug.Log("현재 쉴드량 : " + shieldAmount + ", 초기 쉴드량 : " + initialShieldAmount);

            if (shieldImage != null)
            {
                if (shieldAmount > 0)
                {
                    // 쉴드가 있을 때는 체력바를 가득 채운 뒤 쉴드를 표시
                    shieldImage.gameObject.SetActive(true); // 쉴드 이미지 활성화
                    shieldImage.fillAmount = (float)(shieldAmount / initialShieldAmount); // 남은 쉴드 / 처음 받아온 쉴드량

                    if (shieldImage.fillAmount > 1) // 쉴드가 최대 체력 이상일 때 처리
                    {
                        shieldImage.fillAmount = 1; // 체력바를 넘지 않도록 설정
                    }
                }
                else
                {
                    // 쉴드가 없으면 이미지 비활성화
                    shieldImage.gameObject.SetActive(false);
                }
            }
        }
    }

    private void UpdateHealthSlider()
    {
        if (HPSlider != null && stateSummon != null)
        {
            float sliderHP = (float)(stateSummon.getNowHP() / stateSummon.getMaxHP());
            HPSlider.value = Mathf.Clamp(sliderHP, 0f, 1f);
        }
    }

}
