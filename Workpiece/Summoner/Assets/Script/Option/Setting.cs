using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [Header("�ɼ�(����)â")]
    public GameObject settingPanel;

    // �гε��� ������ ����Ʈ
    [Header("����â(����,�����,�����÷���) �ǳ�")]
    public List<GameObject> panels;

    // ��ư���� ������ ����Ʈ
    [Header("����â(����,�����,�����÷���) ��ư")]
    public List<Button> buttons;


    [Header("��ư Ŭ����")]
    [SerializeField] private AudioSource audioSource;

    // ������ ���ϰ� �� ����
    private float darkenFactor = 0.8f; // ���ϰ� �� ���� (1.0���� ������ �� ������)

    // �� ��ư�� ���� ������ ������ ��ųʸ�
    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();

    // ���� Ȱ��ȭ�� ��ư �ε���
    private int activeIndex = -1;

    // �̱��� �ν��Ͻ�
    public static Setting instance;

    // �̱��� �ν��Ͻ� �ʱ�ȭ
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����Ѵٸ� ���ο� �ν��Ͻ��� �ı�
        }
    }

    void Start()
    {
        InitializeMenu();
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

        audioSource.Play();
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
        if (settingPanel.activeSelf)
        {
            audioSource.Play();
            settingPanel.SetActive(false);
        }
        else
            settingPanel.SetActive(true);
    }


}
