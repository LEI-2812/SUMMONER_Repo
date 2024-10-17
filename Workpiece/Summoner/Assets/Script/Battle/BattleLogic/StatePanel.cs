using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel: MonoBehaviour
{
    [SerializeField] private Summon stateSummon; //Summon
    [SerializeField] private Image summonImage; //image
    [SerializeField] private Slider HPSlider;

    [Header("���� ��ư��")]
    [SerializeField] private Button NormalAttackButton;
    [SerializeField] private Button SpecialAttackButton;


    //���� �ǳڿ� ��ȯ������ ����
    public void setStatePanel(Summon stateSummon, bool isEnemyPlate)
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

        //  �� �÷���Ʈ Ŭ�� �� ���� ��ư ��Ȱ��ȭ
        if (isEnemyPlate)
        {
            NormalAttackButton.gameObject.SetActive(false);
            SpecialAttackButton.gameObject.SetActive(false);
        }
        else
        {
            NormalAttackButton.gameObject.SetActive(true);
            SpecialAttackButton.gameObject.SetActive(true);

            NormalAttackButton.image.sprite = stateSummon.normalAttackSprite;   // �Ϲ� ���� ��������Ʈ ����
            SpecialAttackButton.image.sprite = stateSummon.specialAttackSprite; // Ư�� ���� ��������Ʈ ����
        }
    }


    public Summon getStatePanelSummon()
    {
        return stateSummon;
    }

}
