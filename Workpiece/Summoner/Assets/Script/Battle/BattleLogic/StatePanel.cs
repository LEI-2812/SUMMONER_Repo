using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel: MonoBehaviour
{
    [SerializeField] private Summon stateSummon; //Summon
    [SerializeField] private Image summonImage; //image
    [SerializeField] private Slider HPSlider;

    //���� �ǳڿ� ��ȯ������ ����
    public void setStatePanel(Summon stateSummon)
    {
        this.stateSummon = stateSummon;

        // ��ȯ���� �̹����� �гο� ����
        if (summonImage != null && stateSummon != null && stateSummon.image != null)
        {
            summonImage.sprite = stateSummon.image.sprite; // ��ȯ�� �̹����� �гη� ����
        }

        // ��ȯ���� HP�� �гο� ǥ��
        if (HPSlider != null && stateSummon != null)
        {
            float sliderHP = (float) (stateSummon.getNowHP() / stateSummon.getMaxHP());  // ü�� ���� ���
            HPSlider.value = sliderHP;  // ü�� ������ ���� �����̴� �� ����
        }
    }


    public Summon getStatePanelSummon()
    {
        return stateSummon;
    }

}
