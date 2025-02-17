using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InteractionController : MonoBehaviour, IPointerClickHandler
{
    [Header("캐릭터이름 텍스트")]
    public Text characterName;
    public Text dialogueContext;

    [SerializeField] private InteractionEvent interactionEvent; // InteractionEvent 연결

    private Dialogue[] currentDialogues; // 현재 진행 중인 대화
    private int currentDialogueIndex = 0; // 현재 진행 중인 Dialogue 인덱스(CSV의 ID순서)
    private int currentDialogueLineIndex = 0; // 현재 진행 중인 Dialogue의 대사 인덱스
    private bool isDialogueActive = false; // 대화 진행 상태 체크

    private bool isStory; //스토리가 진행중인지 확인
    private StoryStage storyStage;
    public FadeController fadeController;

    private StoryChangeImage changeImage;

    void Awake()
    {
        changeImage = GetComponent<StoryChangeImage>();
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

        if (!isDialogueActive || isStory) return; // 대화가 진행 중이 아니면 실행 안 함

        // 현재 캐릭터의 대사 출력
        if (currentDialogueIndex < currentDialogues.Length)
        {
            Dialogue currentDialogue = currentDialogues[currentDialogueIndex]; //읽어들일 Dialogue를 가져온다.

            // 현재 캐릭터의 모든 대사를 출력했다면 다음 캐릭터(CSV의 ID)로 넘어감
            if (currentDialogueLineIndex >= currentDialogue.context.Length)
            {
                currentDialogueLineIndex = 0; // 대사 인덱스 초기화
                currentDialogueIndex++; // 다음 캐릭터로 이동
                ShowNextLine(); // 다음 캐릭터 대사 출력
                return;
            }

            // 현재 캐릭터의 대사 출력
            characterName.text = currentDialogue.name;

            // 현재 대사에 이어붙이기 기능 추가
            string combinedDialogue = "";
            
            for (int i = 0; i <= currentDialogueLineIndex; i++)
            {
                if (i % 2 == 0) //3번째 대사마다 이어붙일 대사를 초기화시킨다.
                    combinedDialogue = "";

                combinedDialogue += currentDialogue.context[i];
               
                if (i < currentDialogueLineIndex)
                {
                    combinedDialogue += "\n"; // 대사를 합칠 때 들여쓰기로 추가
                }
            }

            dialogueContext.text = combinedDialogue; // 합쳐진 대사 출력
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
        isDialogueActive = false;
        Debug.Log("대화가 종료되었습니다.");

        fadeController.RegisterCallback(() =>
        {
            switch (storyStage.getStoryNum())
            {
                case 0:
                    Debug.Log("프롤로그");
                    SceneManager.LoadScene("Stage Select Screen");
                    break;
                case 8:
                    Debug.Log("에필로그");
                    SceneManager.LoadScene("Thank Screen");
                    break;
                default:
                    Debug.Log("이도저도 아닌");
                    goToFightScreen();
                    break;
            }
        });

        // 페이드 아웃 실행
        fadeController.FadeOut();
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
                changeImage.ShowImage();
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
        isStory = false; // 이동이 끝나면 InteractionController에서 대사 진행을 허용
    }

    public void stopNextDialogue()
    {
        isStory = true; // 이동이 끝나면 InteractionController에서 대사 진행을 허용
    }

    private void goToFightScreen()
    {
        SceneManager.LoadScene("Fight Screen_"+storyStage.getStoryNum()+"Stage");
    }
}
