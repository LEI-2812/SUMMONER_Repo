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

    [Header("���� ��ư��")]
    [SerializeField] private Button NormalAttackButton;
    [SerializeField] private Button SpecialAttackButton;


    //���� �ǳڿ� ��ȯ������ ����
    public void setStatePanel(Summon stateSummon, bool isEnemyPlate)
    {
        this.stateSummon = stateSummon;
        stateSummon.AddObserver(this); // �������� ���

        // ��ȯ���� �̹����� �гο� ����
        if (summonImage != null && stateSummon != null && stateSummon.getImage() != null)
        {
            summonImage.sprite = stateSummon.getImage().sprite; // ��ȯ�� �̹����� �гη� ����
        }

        // ��ȯ���� HP�� �гο� ǥ��
        if (HPSlider != null && stateSummon != null)
        {
            float sliderHP = (float) (stateSummon.getNowHP() / stateSummon.getMaxHP());  // ü�� ���� ���
            HPSlider.value = sliderHP;  // ü�� ������ ���� �����̴� �� ����

            // ���� ���� ���
            float shieldAmount = (float) stateSummon.getShield(); // ���� ���差
            float shieldRatio = (float) (shieldAmount / stateSummon.getMaxHP()); // �ִ� ü�� �������� ���� ���� ���

            if (shieldFillImage != null)
            {
                shieldFillImage.fillAmount = shieldRatio; // ���� ������ ���� �����̴��� fill amount ����
            }

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
            // ��ȯ���� ��� �������� üũ
            if (stateSummon.getNowHP() <= 0)
            {
                // ��� �� StatePanel ��Ȱ��ȭ
                gameObject.SetActive(false);
                return;
            }

            // HP �����̴� ������Ʈ
            float sliderHP = (float)(stateSummon.getNowHP() / stateSummon.getMaxHP());
            HPSlider.value = sliderHP;
        }
    }
}
