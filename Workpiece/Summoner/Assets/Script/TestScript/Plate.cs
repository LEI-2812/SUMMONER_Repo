using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    //plate�� �����ս��Ѽ� ������.
    public bool isInSummon = false; // ���� ��ȯ���� �ִ��� ����
    public Summon CurrentSummon;   // �÷���Ʈ ���� �ִ� ��ȯ��

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
            Debug.Log($"��ȯ�� {CurrentSummon.summonName} �� ü��: {CurrentSummon.health}");
        }
    }
}
