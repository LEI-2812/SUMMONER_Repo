using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel: MonoBehaviour
{
    public Summon StateSummon; //Summon
    public Image summonImage; //image
    public Slider HPSlider;

    //상태 판넬에 소환수정보 설정
    public void setStatePanel(Summon stateSummon)
    {
        this.StateSummon = stateSummon;

        // 소환수의 이미지를 패널에 설정
        if (summonImage != null && StateSummon != null && StateSummon.image != null)
        {
            summonImage.sprite = StateSummon.image.sprite; // 소환수 이미지를 패널로 전달
        }

        // 소환수의 HP를 패널에 표시
        if (HPSlider != null && StateSummon != null)
        {
            float sliderHP = (float) (StateSummon.nowHP / StateSummon.maxHP);  // 체력 비율 계산
            HPSlider.value = sliderHP;  // 체력 비율에 따른 슬라이더 값 설정
        }
    }

}
