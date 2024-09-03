using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    public Button button; // 버튼 참조
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f); // 기본 색상 (연한 색)
    public Color pressedColor = new Color(0.3f, 0.3f, 0.3f); // 눌렸을 때 색상 (진한 색)

    private Image buttonImage;

    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        buttonImage = button.GetComponent<Image>();

        // 버튼의 초기 색상을 기본 색상으로 설정
        buttonImage.color = normalColor;

        // 버튼 클릭 이벤트 등록
        button.onClick.AddListener(OnButtonClicked);
    }

    // 버튼 클릭 시 호출될 메소드
    void OnButtonClicked()
    {
        // 눌렸을 때 색상으로 변경
        buttonImage.color = pressedColor;
    }

    // 버튼 색상을 기본 색상으로 돌리는 메소드
    void ResetColor()
    {
        buttonImage.color = normalColor;
    }
}
