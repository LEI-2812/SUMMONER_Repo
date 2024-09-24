using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel: MonoBehaviour
{
    public Summon StateSummon; //Summon
    public Image summonImage; //image
    public Slider HPSlider;

    //���� �ǳڿ� ��ȯ������ ����
    public void setStatePanel(Summon stateSummon)
    {
        this.StateSummon = stateSummon;

        // ��ȯ���� �̹����� �гο� ����
        if (summonImage != null && StateSummon != null && StateSummon.image != null)
        {
            summonImage.sprite = StateSummon.image.sprite; // ��ȯ�� �̹����� �гη� ����
        }

        // ��ȯ���� HP�� �гο� ǥ��
        if (HPSlider != null && StateSummon != null)
        {
            float sliderHP = (float) (StateSummon.nowHP / StateSummon.maxHP);  // ü�� ���� ���
            HPSlider.value = sliderHP;  // ü�� ������ ���� �����̴� �� ����
        }
    }

}
