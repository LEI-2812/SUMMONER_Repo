using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuController : MonoBehaviour
{
    private static SettingMenuController instance; //destroyOnload instance

    // �гε��� ������ ����Ʈ
    public List<GameObject> panels;

    // ��ư���� ������ ����Ʈ
    public List<Button> buttons;

    // ������ ���ϰ� �� ����
    public float darkenFactor = 0.8f; // ���ϰ� �� ���� (1.0���� ������ �� ������)

    // �� ��ư�� ���� ������ ������ ��ųʸ�
    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();

    // ���� Ȱ��ȭ�� ��ư �ε���
    private int activeIndex = -1;

    private void Awake()
    {
        // �̱��� �ν��Ͻ��� �̹� �����ϴ��� Ȯ��
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �������� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� ���ο� ������Ʈ�� ����
        }
    }

    // �ʱ�ȭ �޼ҵ�
    void Start()
    {
        // �� ��ư�� ���� ������ �����ϰ�, Ŭ�� �̺�Ʈ ������ �߰�
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // ���� ������ �ε��� ����
            Button button = buttons[i];
            originalColors[button] = button.GetComponent<Image>().color; // ���� ���� ����
            button.onClick.AddListener(() => OnButtonClicked(index));
        }

        // ó������ ù ��° �гθ� Ȱ��ȭ
        ShowPanel(0);
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �޼ҵ�
    void OnButtonClicked(int index)
    {
        // ������ Ŭ���� ��ư ���� ����
        if (activeIndex != -1)
        {
            Button previousButton = buttons[activeIndex];
            previousButton.GetComponent<Image>().color = originalColors[previousButton];
        }

        // ���� Ŭ���� ��ư�� ������ ���ϰ� ����
        activeIndex = index;
        ShowPanel(index);
        UpdateButtonColors(index);
    }

    // Ư�� �г��� Ȱ��ȭ�ϰ� �������� ��Ȱ��ȭ�ϴ� �޼ҵ�
    void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    // ��ư ������ ������Ʈ�ϴ� �޼ҵ�
    void UpdateButtonColors(int activeIndex)
    {
        Image buttonImage = buttons[activeIndex].GetComponent<Image>();
        Color originalColor = originalColors[buttons[activeIndex]];
        Color darkenedColor = originalColor * darkenFactor;
        darkenedColor.a = originalColor.a; // ���İ� ����
        buttonImage.color = darkenedColor;
    }
}
