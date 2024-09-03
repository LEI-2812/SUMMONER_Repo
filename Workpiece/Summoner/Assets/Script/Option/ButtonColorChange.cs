using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    public Button button; // ��ư ����
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f); // �⺻ ���� (���� ��)
    public Color pressedColor = new Color(0.3f, 0.3f, 0.3f); // ������ �� ���� (���� ��)

    private Image buttonImage;

    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        buttonImage = button.GetComponent<Image>();

        // ��ư�� �ʱ� ������ �⺻ �������� ����
        buttonImage.color = normalColor;

        // ��ư Ŭ�� �̺�Ʈ ���
        button.onClick.AddListener(OnButtonClicked);
    }

    // ��ư Ŭ�� �� ȣ��� �޼ҵ�
    void OnButtonClicked()
    {
        // ������ �� �������� ����
        buttonImage.color = pressedColor;
    }

    // ��ư ������ �⺻ �������� ������ �޼ҵ�
    void ResetColor()
    {
        buttonImage.color = normalColor;
    }
}
