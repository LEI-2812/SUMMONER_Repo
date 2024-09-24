using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PickSummonPanel : MonoBehaviour,
    IPointerEnterHandler, //�÷���Ʈ�� ���콺 �÷����� �̺�Ʈ �������̽�
    IPointerExitHandler,  //�÷���Ʈ�� ���콺�� ������� �̺�Ʈ �������̽�
    IPointerClickHandler //�÷���Ʈ Ŭ���� ����â 
{
    public Summon assignedSummon; // �гο� �Ҵ�� ��ȯ��
    public Image summonImage;


    // �г��� �̹����� �����ϴ� �޼ҵ�
    public void SetSummonImage(Image image)
    {
        if (summonImage != null && summonImage != null)
        {
            summonImage.sprite = image.sprite; // �гο� ��ȯ�� �̹��� �Ҵ�
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // �г� Ŭ�� �� �ش� ��ȯ�� ��ȯ
        SummonController.Instance.OnSelectSummon(assignedSummon);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (summonImage != null)
        {
            summonImage.color = Color.yellow; // ���콺�� �ø��� ������ ��������� ����
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (summonImage != null)
        {
            summonImage.color = Color.white; // ���콺�� ����� ���� �������� ����
        }
    }

    // �г��� ���� �� ������ �ʱ�ȭ�ϴ� �޼ҵ�
    private void OnDisable()
    {
        // �ʱ� ���·� ����: ������ �������, �̹��� ����
        if (summonImage != null)
        {
            summonImage.sprite = null; // �̹��� �ʱ�ȭ
            summonImage.color = Color.white; // ������ �⺻������ ����
        }

        // ��ȯ�� �Ҵ絵 �ʱ�ȭ
        assignedSummon = null;
    }
}
