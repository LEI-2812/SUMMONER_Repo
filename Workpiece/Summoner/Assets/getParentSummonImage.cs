using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class getParenSummontImage : MonoBehaviour
{
    private Image summonImage;

    [Header("�� ��ȯ�� �ֱ�")]
    [SerializeField] private Summon summon; // �� ��ȯ�� ���� �Ҵ�
    private Plate plate; // �θ� Plate ������Ʈ�� �ޱ� ���� ����

    void Start()
    {
        // �θ� ������Ʈ�� �پ� �ִ� Plate ������Ʈ�� ������
        plate = GetComponentInParent<Plate>();

        // ��ȯ���� ����� �Ҵ�Ǿ����� Ȯ��
        if (summon != null)
        {
            // �θ� Plate�� ��ȯ���� ����
            if (plate != null)
            {
                plate.SummonPlaceOnPlate(summon, isResummon: false); // �θ� Plate�� ��ȯ�� ��ġ
            }

            // ��ȯ������ Image ������Ʈ�� ������
            summonImage = summon.GetComponent<Image>();

            if (summonImage != null)
            {
                // �θ� ������Ʈ�� �ڽ� �̹��� ������Ʈ�� ������
                Image childImage = GetComponent<Image>();
                if (childImage != null)
                {
                    // ��ȯ���� �̹����� �ڽ� �̹����� ����
                    childImage.sprite = summonImage.sprite;

                    // ������ 1�� ���� (���� �������ϰ�)
                    Color newColor = childImage.color;
                    newColor.a = 1f; // ���� ��(����)�� 1�� ����
                    childImage.color = newColor;
                }
                else
                {
                    Debug.Log("�ڽ� ������Ʈ�� Image ������Ʈ�� �����ϴ�.");
                }
            }
            else
            {
                Debug.Log("��ȯ���� Image ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.Log("�Ҵ�� ��ȯ���� �����ϴ�.");
        }
    }

}
