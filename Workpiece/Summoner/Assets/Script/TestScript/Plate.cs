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
    public OnMousePlate onMousePlateScript; // ���� �гο� ��ȯ�� ������ ������Ʈ�ϴ� ��ũ��Ʈ
    public Image SummonImg;


    // ��ȯ���� �÷���Ʈ�� ��ġ
    public void SummonPlaceOnPlate(Summon summon)
    {
        if (!isInSummon)
        {
            currentSummon = summon;
            isInSummon = true;
            Debug.Log($"��ȯ�� {summon.summonName} �� �÷���Ʈ�� ��ȯ");
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
        if (currentSummon != null && statePanel.activeSelf==false)
        {
            CheckHealth();
            statePanel.SetActive(true);
            onMousePlateScript.setStatePanel(currentSummon); // �гο� ��ȯ�� ���� ���� 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSummon != null && statePanel.activeSelf == true)
        {
            // �г��� ��Ȱ��ȭ
            statePanel.SetActive(false);
        }
    }

    //�ش� �÷���Ʈ Ŭ���� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("�÷���Ʈ Ŭ����");
    }

}
