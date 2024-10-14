using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InteractionController : MonoBehaviour, IPointerClickHandler
{
    [Header("ĳ�����̸� �ؽ�Ʈ")]
    public Text characterName;
    public Text dialogueContext;

    [SerializeField] private InteractionEvent interactionEvent; // InteractionEvent ����

    private Dialogue[] currentDialogues; // ���� ���� ���� ��ȭ
    private int currentDialogueIndex = 0; // ���� ���� ���� Dialogue �ε���(CSV�� ID����)
    private int currentDialogueLineIndex = 0; // ���� ���� ���� Dialogue�� ��� �ε���
    private bool isDialogueActive = false; // ��ȭ ���� ���� üũ

    private bool isStory; //���丮�� ���������� Ȯ��
    private CheckStage checkStage;
    public FadeController fadeController;

    void Start()
    {
        checkStage = GetComponent<CheckStage>();

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

        if (!isDialogueActive || isStory) return; // ��ȭ�� ���� ���� �ƴϸ� ���� �� ��

        // ���� ĳ������ ��� ���
        if (currentDialogueIndex < currentDialogues.Length)
        {
            Dialogue currentDialogue = currentDialogues[currentDialogueIndex]; //�о���� Dialogue�� �����´�.

            // ���� ĳ������ ��� ��縦 ����ߴٸ� ���� ĳ����(CSV�� ID)�� �Ѿ
            if (currentDialogueLineIndex >= currentDialogue.context.Length)
            {
                currentDialogueLineIndex = 0; // ��� �ε��� �ʱ�ȭ
                currentDialogueIndex++; // ���� ĳ���ͷ� �̵�
                ShowNextLine(); // ���� ĳ���� ��� ���
                return;
            }

            // ���� ĳ������ ��� ���
            characterName.text = currentDialogue.name;

            // ���� ��翡 �̾���̱� ��� �߰�
            string combinedDialogue = "";
            
            for (int i = 0; i <= currentDialogueLineIndex; i++)
            {
                if (i % 2 == 0) //3��° ��縶�� �̾���� ��縦 �ʱ�ȭ��Ų��.
                    combinedDialogue = "";

                combinedDialogue += currentDialogue.context[i];
               
                if (i < currentDialogueLineIndex)
                {
                    combinedDialogue += "\n"; // ��縦 ��ĥ �� �鿩����� �߰�
                }
            }

            dialogueContext.text = combinedDialogue; // ������ ��� ���
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

        if (checkStage.stageNum == 0)
        {
            Debug.Log("���ѷα�");
            SceneManager.LoadScene("Stage Select Screen"); //���ѷα� ���� ���
        }
        else if (checkStage.stageNum == 8)
        {
            Debug.Log("���ʷα�");
            SceneManager.LoadScene("Thank Screen"); //���ʷα� ���� ���
        }
        else
        {
            Debug.Log("�̵����� �ƴ�");
            fadeController.FadeOut();
            Invoke("GotoFightScreen", 1f); // ���̵� �ƿ��� �ɰ� ���� ������ �̵�           
        }
    }

    void Update()
    {
        // ������ �Է����� ��� �ѱ�� (��: �����̽���)
       // if (isDialogueActive && Input.GetKeyDown(KeyCode.Space) && !isStory)
       // {
        //    ShowNextLine();
       // }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ���� �� �̸��� "Pro_Epi Screen"�� ���� Ŭ�� �̺�Ʈ ó��
        if (SceneManager.GetActiveScene().name == "Pro_Epi Screen")
        {
            Debug.Log("Ŭ�� �̺�Ʈ �߻�");
            if (isDialogueActive && !isStory)
            {
                ShowNextLine();
            }
        }
    }


    public int getCurrentDialogueIndex()
    {
        return currentDialogueIndex;
    }

    public int getCurrentDialogueLineIndex()
    {
        return currentDialogueLineIndex;
    }

    public bool getIsStory()
    {
        return isStory;
    }

    public void startNextDialogue()
    {
        isStory = false; // �̵��� ������ InteractionController���� ��� ������ ���
    }

    public void stopNextDialogue()
    {
        isStory = true; // �̵��� ������ InteractionController���� ��� ������ ���
    }

    public void GotoFightScreen()
    {
        SceneManager.LoadScene("Fight Screen"); // ��ȭ�� ������ ���� ������ ���� 
    }
}
