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

    void Start()
    {
        statePanel.SetActive(false);
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

    //���콺 �÷����� �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
       // Debug.Log("�÷���Ʈ�� ���콺 �ö��");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        /*if (currentSummon != null && statePanel.activeSelf == true)
        {
            // �г��� ��Ȱ��ȭ
            statePanel.SetActive(false);
        }*/
    }

    //�ش� �÷���Ʈ Ŭ���� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentSummon != null)
        {
            Debug.Log("Ŭ���� �÷���Ʈ�� ��ȯ��:" + currentSummon.name);
            CheckHealth();
            statePanel.SetActive(true);
            onMousePlateScript.setStatePanel(currentSummon); // �гο� ��ȯ�� ���� ���� 
        }
    }

}
