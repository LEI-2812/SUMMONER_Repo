using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InteractionController : MonoBehaviour, IPointerClickHandler
{
    [Header("캐릭터이름 텍스트")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueContext;

    [SerializeField] private InteractionEvent interactionEvent; // InteractionEvent 연결

    private Dialogue[] currentDialogues; // 현재 진행 중인 대화
    private int currentDialogueIndex = 0; // 현재 진행 중인 Dialogue 인덱스
    private int currentDialogueLineIndex = 0; // 현재 진행 중인 Dialogue의 대사 인덱스
    private bool isDialogueActive = false; // 대화 진행 상태 체크

    void Start()
    {
        // 초기화
        characterName.text = "";
        dialogueContext.text = "";

        // 시작하자마자 대화를 시작
        StartDialogue();
    }

    // 대화 시작 메서드
    public void StartDialogue()
    {
        if (isDialogueActive) return; // 이미 대화 중이면 중복 실행 방지

        currentDialogues = interactionEvent.getDialogue(); // 대사를 가져온다.
        if (currentDialogues == null || currentDialogues.Length == 0) // 대사가 없으면 종료
        {
            Debug.LogWarning("대화 내용이 없습니다.");
            return;
        }

        currentDialogueIndex = 0; // 캐릭터 대사 초기화
        currentDialogueLineIndex = 0; // 대사 인덱스 초기화
        isDialogueActive = true;
        ShowNextLine(); // 첫 번째 대사 출력
    }

    // 다음 대사를 보여주는 메서드
    public void ShowNextLine()
    {
        if (!isDialogueActive) return; // 대화가 진행 중이 아니면 실행 안 함

        // 현재 캐릭터의 대사 출력
        if (currentDialogueIndex < currentDialogues.Length)
        {
            Dialogue currentDialogue = currentDialogues[currentDialogueIndex]; //읽어들일 Dialogue를 가져온다.

            // 현재 캐릭터의 모든 대사를 출력했다면 다음 캐릭터로 넘어감
            if (currentDialogueLineIndex >= currentDialogue.context.Length)
            {
                currentDialogueLineIndex = 0; // 대사 인덱스 초기화
                currentDialogueIndex++; // 다음 캐릭터로 이동
                ShowNextLine(); // 다음 캐릭터 대사 출력
                return;
            }

            // 현재 캐릭터의 대사 출력
            characterName.text = currentDialogue.name;
            dialogueContext.text = currentDialogue.context[currentDialogueLineIndex];
            currentDialogueLineIndex++; // 다음 대사로 이동
        }
        else
        {
            EndDialogue(); // 모든 대사가 끝나면 대화 종료
        }
    }

    // 대화 종료 메서드
    public void EndDialogue()
    {
        //characterName.text = "";  공백 대화창이 하나 나와서 없애고 바로 전투 씬으로 보냄
        //dialogueContext.text = "";
        isDialogueActive = false;
        Debug.Log("대화가 종료되었습니다.");
        SceneManager.LoadScene("Fight Screen"); // 대화가 끝나면 전투 씬으로 변경
    }

    void Update()
    {
        // 유저의 입력으로 대사 넘기기 (예: 스페이스바)
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
