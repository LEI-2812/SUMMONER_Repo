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
    public Summon CurrentSummon;   // �÷���Ʈ ���� �ִ� ��ȯ��
    public GameObject statePanel;  // ���� �г� (On/Off)
    public OnMousePlate onMousePlateScript; // ���� �гο� ��ȯ�� ������ ������Ʈ�ϴ� ��ũ��Ʈ


    // ��ȯ���� �÷���Ʈ�� ��ġ
    public void SummonPlaceOnPlate(Summon summon)
    {
        if (!isInSummon)
        {
            CurrentSummon = summon;
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
            CurrentSummon = null;
            isInSummon = false;
            Debug.Log("��ȯ�� ����.");
        }
    }

    // ü�� üũ (�÷���Ʈ �� ��ȯ���� ü�� Ȯ��)
    public void CheckHealth()
    {
        if (CurrentSummon != null)
        {
            Debug.Log($"��ȯ�� {CurrentSummon.summonName} �� ü��: {CurrentSummon.nowHP}");
        }
    }

    //���콺 �÷����� �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CurrentSummon != null && statePanel.activeSelf==false)
        {
            CheckHealth();
            statePanel.SetActive(true);
            onMousePlateScript.setStatePanel(CurrentSummon); // �гο� ��ȯ�� ���� ���� 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CurrentSummon != null && statePanel.activeSelf == true)
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
