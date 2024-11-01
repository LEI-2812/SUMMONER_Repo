using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class getParenSummontImage : MonoBehaviour
{
    [Header("�� ��ȯ�� �ֱ�")]
    [SerializeField] private Summon summon; // �� ��ȯ�� ���� �Ҵ�
    private Plate plate; // �θ� Plate ������Ʈ�� �ޱ� ���� ����

    void Awake()
    {
        // �θ� ������Ʈ�� �پ� �ִ� Plate ������Ʈ�� ������
        plate = GetComponent<Plate>();

        // ��ȯ���� ����� �Ҵ�Ǿ����� Ȯ��
        if (summon != null)
        {
            // �θ� Plate�� ��ȯ���� ����
            if (plate != null)
            {
                plate.SummonPlaceOnPlate(summon, isResummon: false); // �θ� Plate�� ��ȯ�� ��ġ
            }
        }
        else
        {
            Debug.Log("�Ҵ�� ��ȯ���� �����ϴ�.");
        }
    }

}
