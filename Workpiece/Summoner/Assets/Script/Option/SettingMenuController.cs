using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuController : MonoBehaviour
{
    public static SettingMenuController instance; // �̱��� �ν��Ͻ� ��� �������� �� ��ũ��Ʈ�� �ϳ��� ���Ǿ����.
    public GameObject option;

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
            //Debug.Log("�� ������Ʈ ����");
            instance = this; // ���� ������Ʈ�� �ν��Ͻ��� ����
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ������Ʈ�� �ı����� �ʵ��� ����
            InitializeMenu(); // �޴� �ʱ�ȭ
        }
        else
        {
            //Debug.Log("������Ʈ �ı�");
            Destroy(gameObject); // �ߺ��� ������Ʈ�� �ı�
            return; // �ı� �� ���� ���� ������ ����
        }
    }

    // �޴� �ʱ�ȭ �۾�
    private void InitializeMenu()
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

    // ����â ����/�ݱ�
    public void openOption()
    {
        if (option.activeSelf)
            option.SetActive(false);
        else
            option.SetActive(true);
    }
}
