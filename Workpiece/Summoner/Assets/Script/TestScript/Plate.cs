using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Plate : MonoBehaviour, 
    IPointerEnterHandler, //�÷���Ʈ�� ���콺 �÷����� �̺�Ʈ �������̽�
    IPointerExitHandler,  //�÷���Ʈ�� ���콺�� ������� �̺�Ʈ �������̽�
    IPointerClickHandler //�÷���Ʈ Ŭ���� ����â 
{
    //plate�� �����ս��Ѽ� ������.
    public bool isInSummon = false; // ���� ��ȯ���� �ִ��� ����
    public Summon currentSummon;   // �÷���Ʈ ���� �ִ� ��ȯ��
    public GameObject statePanel;  // ���� �г� (On/Off)
    public StatePanel onMousePlateScript; // ���� �гο� ��ȯ�� ������ ������Ʈ�ϴ� ��ũ��Ʈ
    public Image summonImg;

    private Image plateImage; // �ڱ� �ڽ��� Image ������Ʈ
    private Color originalColor;

    [SerializeField] private SummonController summonController;

    void Start()
    {
        statePanel.SetActive(false);
        plateImage = GetComponent<Image>(); // �ڽ��� Image ������Ʈ ��������
        originalColor = plateImage.color; // ���� ���� ����
    }

    // ��ȯ���� �÷���Ʈ�� ��ġ
    public void SummonPlaceOnPlate(Summon summon, bool isResummon = false)
    {
        // �̹� ��ȯ���� �־ ���ȯ�̸� ����
        if (!isInSummon || isResummon)
        {
            currentSummon = summon;
            isInSummon = true;

            // ��ȯ�� �̹��� ����
            summonImg.sprite = summon.image.sprite;
            // ������ 255�� �����Ͽ� ������ �������ϰ� �����
            Color color = summonImg.color;
            color.a = 1.0f; // ���� ���� 1�� ���� (255/255)
            summonImg.color = color;

            Debug.Log($"��ȯ�� {summon.summonName} �� {(isResummon ? "���ȯ" : "��ȯ")}�߽��ϴ�.");
        }
        else
        {
            Debug.Log("�̹� �� �÷���Ʈ�� ��ȯ���� �ֽ��ϴ�.");
        }
    }

    // ��ȯ���� ����ϰų� �÷���Ʈ���� ���� ��
    public void RemoveSummon()
    {
        if (isInSummon)
        {
            currentSummon = null;
            isInSummon = false;
            Debug.Log("��ȯ�� ����.");
        }
    }

    // ü�� üũ (�÷���Ʈ �� ��ȯ���� ü�� Ȯ��)
    public void CheckHealth()
    {
        if (currentSummon != null)
        {
            Debug.Log($"��ȯ�� {currentSummon.summonName} �� ü��: {currentSummon.nowHP}");
        }
    }

    // �÷���Ʈ ���� (���� ����)
    public void Highlight()
    {
        plateImage.color = Color.yellow; // �̹����� ������ ��������� ����
    }

    // ���� ����
    public void Unhighlight()
    {
        plateImage.color = originalColor; // �̹����� ������ ������� ����
    }


    //���콺 �÷����� �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSummon != null) //��ȯ���� ������
        {
            SetSummonImageTransparency(1.0f); // ������ ���� �� ���ϰ� ���̰�
        }
        if (isInSummon && summonController.IsResummoning()) // ���ȯ ���̰� ��ȯ���� �ִ� ���
        {
            Highlight(); // �÷���Ʈ ����
            SetSummonImageTransparency(1.0f); // ���� ���̱�
        }
    }


    //���콺�� �����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSummon != null && summonController.IsResummoning()) //���ȯ���϶� �� ���ϰ�
        {
            Unhighlight(); // ���� ����
            SetSummonImageTransparency(0.5f); // �ٽ� �帮��
        }

    }

    //�ش� �÷���Ʈ Ŭ���� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        // �÷��̾ ���ȯ ���̶�� ���� �г��� ���� �ʵ��� ��
        if (summonController.IsResummoning() && isInSummon)
        {
                summonController.SelectPlate(this);
        }

        if (currentSummon != null && !summonController.IsResummoning())
        {
            Debug.Log("Ŭ���� �÷���Ʈ�� ��ȯ��:" + currentSummon.name);
            CheckHealth();
            statePanel.SetActive(true);
            onMousePlateScript.setStatePanel(currentSummon); // �гο� ��ȯ�� ���� ���� 
        }
    }

    // ��ȯ�� �̹��� ���� ����
    public void SetSummonImageTransparency(float alpha)
    {
        if (summonImg != null)
        {
            Color color = summonImg.color;
            color.a = alpha; // ���� ����
            summonImg.color = color;
        }
    }

}
