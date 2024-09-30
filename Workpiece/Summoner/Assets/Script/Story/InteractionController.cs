using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InteractionController : MonoBehaviour, IPointerClickHandler
{
    [Header("ĳ�����̸� �ؽ�Ʈ")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueContext;

    [SerializeField] private InteractionEvent interactionEvent; // InteractionEvent ����

    private Dialogue[] currentDialogues; // ���� ���� ���� ��ȭ
    private int currentDialogueIndex = 0; // ���� ���� ���� Dialogue �ε���
    private int currentDialogueLineIndex = 0; // ���� ���� ���� Dialogue�� ��� �ε���
    private bool isDialogueActive = false; // ��ȭ ���� ���� üũ

    void Start()
    {
        // �ʱ�ȭ
        characterName.text = "";
        dialogueContext.text = "";

        // �������ڸ��� ��ȭ�� ����
        StartDialogue();
    }

    // ��ȭ ���� �޼���
    public void StartDialogue()
    {
        if (isDialogueActive) return; // �̹� ��ȭ ���̸� �ߺ� ���� ����

        currentDialogues = interactionEvent.getDialogue(); // ��縦 �����´�.
        if (currentDialogues == null || currentDialogues.Length == 0) // ��簡 ������ ����
        {
            Debug.LogWarning("��ȭ ������ �����ϴ�.");
            return;
        }

        currentDialogueIndex = 0; // ĳ���� ��� �ʱ�ȭ
        currentDialogueLineIndex = 0; // ��� �ε��� �ʱ�ȭ
        isDialogueActive = true;
        ShowNextLine(); // ù ��° ��� ���
    }

    // ���� ��縦 �����ִ� �޼���
    public void ShowNextLine()
    {
        if (!isDialogueActive) return; // ��ȭ�� ���� ���� �ƴϸ� ���� �� ��

        // ���� ĳ������ ��� ���
        if (currentDialogueIndex < currentDialogues.Length)
        {
            Dialogue currentDialogue = currentDialogues[currentDialogueIndex]; //�о���� Dialogue�� �����´�.

            // ���� ĳ������ ��� ��縦 ����ߴٸ� ���� ĳ���ͷ� �Ѿ
            if (currentDialogueLineIndex >= currentDialogue.context.Length)
            {
                currentDialogueLineIndex = 0; // ��� �ε��� �ʱ�ȭ
                currentDialogueIndex++; // ���� ĳ���ͷ� �̵�
                ShowNextLine(); // ���� ĳ���� ��� ���
                return;
            }

            // ���� ĳ������ ��� ���
            characterName.text = currentDialogue.name;
            dialogueContext.text = currentDialogue.context[currentDialogueLineIndex];
            currentDialogueLineIndex++; // ���� ���� �̵�
        }
        else
        {
            EndDialogue(); // ��� ��簡 ������ ��ȭ ����
        }
    }

    // ��ȭ ���� �޼���
    public void EndDialogue()
    {
        //characterName.text = "";  ���� ��ȭâ�� �ϳ� ���ͼ� ���ְ� �ٷ� ���� ������ ����
        //dialogueContext.text = "";
        isDialogueActive = false;
        Debug.Log("��ȭ�� ����Ǿ����ϴ�.");
        SceneManager.LoadScene("Fight Screen"); // ��ȭ�� ������ ���� ������ ����
    }

    void Update()
    {
        // ������ �Է����� ��� �ѱ�� (��: �����̽���)
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowNextLine();
    }
}
