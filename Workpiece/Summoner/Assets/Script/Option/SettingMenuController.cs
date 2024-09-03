using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuController : MonoBehaviour
{
    private static SettingMenuController instance; //destroyOnload instance

    // 패널들을 저장할 리스트
    public List<GameObject> panels;

    // 버튼들을 저장할 리스트
    public List<Button> buttons;

    // 색상을 진하게 할 정도
    public float darkenFactor = 0.8f; // 진하게 할 비율 (1.0보다 작으면 더 진해짐)

    // 각 버튼의 원래 색상을 저장할 딕셔너리
    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();

    // 현재 활성화된 버튼 인덱스
    private int activeIndex = -1;

    private void Awake()
    {
        // 싱글톤 인스턴스가 이미 존재하는지 확인
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 삭제되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 새로운 오브젝트를 삭제
        }
    }

    // 초기화 메소드
    void Start()
    {
        // 각 버튼의 원래 색상을 저장하고, 클릭 이벤트 리스너 추가
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // 로컬 변수로 인덱스 저장
            Button button = buttons[i];
            originalColors[button] = button.GetComponent<Image>().color; // 원래 색상 저장
            button.onClick.AddListener(() => OnButtonClicked(index));
        }

        // 처음에는 첫 번째 패널만 활성화
        ShowPanel(0);
    }

    // 버튼 클릭 시 호출되는 메소드
    void OnButtonClicked(int index)
    {
        // 이전에 클릭된 버튼 색상 복원
        if (activeIndex != -1)
        {
            Button previousButton = buttons[activeIndex];
            previousButton.GetComponent<Image>().color = originalColors[previousButton];
        }

        // 현재 클릭된 버튼의 색상을 진하게 변경
        activeIndex = index;
        ShowPanel(index);
        UpdateButtonColors(index);
    }

    // 특정 패널을 활성화하고 나머지는 비활성화하는 메소드
    void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    // 버튼 색상을 업데이트하는 메소드
    void UpdateButtonColors(int activeIndex)
    {
        Image buttonImage = buttons[activeIndex].GetComponent<Image>();
        Color originalColor = originalColors[buttons[activeIndex]];
        Color darkenedColor = originalColor * darkenFactor;
        darkenedColor.a = originalColor.a; // 알파값 유지
        buttonImage.color = darkenedColor;
    }
}
