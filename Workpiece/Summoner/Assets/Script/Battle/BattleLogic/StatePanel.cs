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

    [Header("���� ��ư��")]
    [SerializeField] private Button NormalAttackButton;
    [SerializeField] private Button SpecialAttackButton;



    //���� �ǳڿ� ��ȯ������ ����
    public void setStatePanel(Summon stateSummon, bool isEnemyPlate)
    {
        statePanel.gameObject.SetActive(true);
        this.stateSummon = stateSummon;
        stateSummon.AddObserver(this); // �������� ���

        // ü�¹ٸ� �ʱ� ����
        UpdateHealthSlider();

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
        Color color = shieldImage.color;
        color.a = 0.7f;
        shieldImage.color = color;

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
            float currentHP = (float)stateSummon.getNowHP();
            float maxHP = (float)stateSummon.getMaxHP();

            // ü�� ������ 0�� 1 ���̷� ����
            float sliderHP = currentHP / maxHP;
            HPSlider.value = Mathf.Clamp(sliderHP, 0f, 1f);
            //Debug.Log("ü�� �����̴� ���� ��: " + HPSlider.value);

            // ���� ���� ���
            float shieldAmount = (float)stateSummon.getShield(); // ���� ���差
            float initialShieldAmount = (float)stateSummon.GetInitialShield();
            //Debug.Log("���� ���差 : " + shieldAmount + ", �ʱ� ���差 : " + initialShieldAmount);

            if (shieldImage != null)
            {
                if (shieldAmount > 0)
                {
                    // ���尡 ���� ���� ü�¹ٸ� ���� ä�� �� ���带 ǥ��
                    shieldImage.gameObject.SetActive(true); // ���� �̹��� Ȱ��ȭ
                    shieldImage.fillAmount = (float)(shieldAmount / initialShieldAmount); // ���� ���� / ó�� �޾ƿ� ���差

                    if (shieldImage.fillAmount > 1) // ���尡 �ִ� ü�� �̻��� �� ó��
                    {
                        shieldImage.fillAmount = 1; // ü�¹ٸ� ���� �ʵ��� ����
                    }
                }
                else
                {
                    // ���尡 ������ �̹��� ��Ȱ��ȭ
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
