using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: 옵션 패널 전환과 선택된 설정 탭의 버튼 표시를 관리한다.
public class SettingPanelView : MonoBehaviour
{
    [Header("옵션 패널")]
    public GameObject settingPanel;

    [Header("설정 탭 패널")]
    public List<GameObject> panels;

    [Header("설정 탭 버튼")]
    public List<Button> buttons;

    [Header("설정 화면")]
    [SerializeField] private AudioSettingView audioController;
    [SerializeField] private VideoSettingView videoController;
    [SerializeField] private GameplaySettingView gamePlayController;

    [Header("효과음")]
    [SerializeField] private AudioSource audioSource;

    private float darkenFactor = 0.8f;

    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();

    private int activeIndex = -1;

    public static SettingPanelView instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            Button button = buttons[i];
            originalColors[button] = button.GetComponent<Image>().color;
            button.onClick.AddListener(() => OnButtonClicked(index));
        }

        ShowPanel(0);
    }

    void OnButtonClicked(int index)
    {
        if (activeIndex != -1)
        {
            Button previousButton = buttons[activeIndex];
            previousButton.GetComponent<Image>().color = originalColors[previousButton];
        }

        audioSource.Play();
        activeIndex = index;
        ShowPanel(index);
        UpdateButtonColors(index);
    }

    void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    void UpdateButtonColors(int activeIndex)
    {
        Image buttonImage = buttons[activeIndex].GetComponent<Image>();
        Color originalColor = originalColors[buttons[activeIndex]];
        Color darkenedColor = originalColor * darkenFactor;
        darkenedColor.a = originalColor.a;
        buttonImage.color = darkenedColor;
    }

    public void OpenOption()
    {
        if (settingPanel == null)
        {
            Debug.LogError("SettingPanel이 할당되지 않았습니다.");
            return;
        }

        if (settingPanel.activeSelf)
        {
            audioSource?.Play();
            settingPanel.SetActive(false);
        }
        else
        {
            settingPanel.SetActive(true);
        }
    }
}
