using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// 역할: 스토리 오브젝트 클릭 입력을 받아 상호작용 이벤트를 실행한다.
public class InteractionController : MonoBehaviour, IPointerClickHandler
{
    [Header("캐릭터이름 텍스트")]
    public Text characterName;
    public Text dialogueContext;

    [SerializeField] private InteractionEvent interactionEvent; // InteractionEvent 연결

    private readonly StoryProgressAdvance storyProgressAdvance = new StoryProgressAdvance();
    private bool isDialogueActive = false; // 대화 진행 상태 체크

    private bool isStory; //스토리가 진행중인지 확인
    private StoryStage storyStage;
    [FormerlySerializedAs("fadeController")]
    [SerializeField] private FadePanelView fadePanelView;

    private StoryImageView storyImageView;

    void Awake()
    {
        storyImageView = GetComponent<StoryImageView>();
        storyStage = GetComponent<StoryStage>();
    }

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
        Dialogue[] currentDialogues = interactionEvent.GetDialogue(); // 대사를 가져온다.
        if (currentDialogues == null || currentDialogues.Length == 0) // 대사가 없으면 종료
        {
            Debug.LogWarning("대화 내용이 없습니다.");
            return;
        }

        storyProgressAdvance.Start(currentDialogues);
        isDialogueActive = true;
        ShowNextLine(); // 첫 번째 대사 출력
    }

    // 다음 대사를 보여주는 메서드
    public void ShowNextLine()
    {

        if (!isDialogueActive || isStory) return; // 대화가 진행 중이 아니면 실행 안 함

        if (storyProgressAdvance.TryGetNextLine(out Dialogue currentDialogue, out int dialogueLineIndex))
        {
            DialogueLineShow.Show(characterName, dialogueContext, currentDialogue, dialogueLineIndex);
        }
        else
        {
            EndDialogue(); // 모든 대사가 끝나면 대화 종료
        }
    }

    // 대화 종료 메서드
    public void EndDialogue()
    {
        isDialogueActive = false;
        Debug.Log("대화가 종료되었습니다.");

        fadePanelView.RegisterCallback(() =>
        {
            StorySceneFlow.LoadNextScene(storyStage.GetStoryNum());
        });

        // 페이드 아웃 실행
        fadePanelView.FadeOut();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 현재 씬 이름이 "Pro_Epi Screen"일 때만 클릭 이벤트 처리
        if ((SceneManager.GetActiveScene().name == "Prologue Screen") || (SceneManager.GetActiveScene().name == "Epilogue Screen"))
        {
            Debug.Log("클릭 이벤트 발생");
            if (isDialogueActive && !isStory)
            {
                ShowNextLine();
                storyImageView.ShowImage();
            }
        }
    }

    public int GetCurrentDialogueIndex()
    {
        return storyProgressAdvance.CurrentDialogueIndex;
    }

    public int GetCurrentDialogueLineIndex()
    {
        return storyProgressAdvance.CurrentDialogueLineIndex;
    }

    public bool GetIsStory()
    {
        return isStory;
    }

    public void StartNextDialogue()
    {
        isStory = false; // 이동이 끝나면 InteractionController에서 대사 진행을 허용
    }

    public void StopNextDialogue()
    {
        isStory = true; // 이동이 끝나면 InteractionController에서 대사 진행을 허용
    }

}
